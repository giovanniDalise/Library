using Library.Logging.Abstractions;
using RabbitMQ.Client;
using System.Text;

namespace Library.EmailService.Infrastructure.Adapters.RabbitMQ
{
    public abstract class BaseRabbitMQConsumer : BackgroundService
    {
        protected readonly ILoggerPort _logger;
        protected IConnection? _connection;
        protected IChannel? _channel;

        private readonly string _hostName;
        private readonly string _userName;
        private readonly string _password;

        protected const int MaxRetries = 3;
        protected const int RetryDelayMs = 30000;

        // ogni consumer definisce i propri nomi di coda
        protected abstract string MainQueue { get; }
        protected abstract string RetryQueue { get; }
        protected abstract string DlqQueue { get; }
        // Pattern di messaggiastica Dead Letter Queue(DLQ).
        // Ad esempio per la coda UserRegistered IdentityServiceUtente registrato → pubblica un evento
        // RabbitMQ — Main Exchange → Main Queue Il messaggio viene instradato al consumer
        // EmailService Consuma l'evento e invia l'email
        // Se l'invio fallisce: Retry Queue Attende 30 secondi e riprova
        // DLQ Conserva i messaggi falliti definitivamente
        protected abstract string MainExchange { get; }
        protected abstract string RetryExchange { get; }
        protected abstract string DlqExchange { get; }
        // Exchange = smistatore | Riceve i messaggi dal producer e decide a quali code inoltrarli, usando le regole di routing.
        // Queue = coda di attesa | Conserva i messaggi finché un consumer non li elabora e ne conferma la ricezione.
        // Binding = collegamento | Collega un Exchange a una Queue, definendo la routing key da usare per l'instradamento.
        // Nel tuo codice utilizzi ExchangeType.Direct: significa che l'Exchange inoltra un messaggio alla coda il cui binding ha una routing key corrispondente esattamente a quella usata nella pubblicazione.
        // Ma ogni coda ha il suo exchange dedicato è il pattern corretto con RabbitMQ Direct Exchange.
        // Un exchange unico richiederebbe routing key complesse e renderebbe tutto più difficile da debuggare.

        protected BaseRabbitMQConsumer(
            ILoggerPort logger,
            string hostName,
            string userName,
            string password)
        {
            _logger = logger;
            _hostName = hostName;
            _userName = userName;
            _password = password;
        }

        protected async Task InitializeConnectionAsync()
        {
            var factory = new ConnectionFactory
            {
                HostName = _hostName,
                UserName = _userName,
                Password = _password
            };
            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
        }

        protected async Task SetupQueuesAsync()
        {
            // Crea e configura gli Exchange e le Queue di RabbitMQ.
            // La Main Queue gestisce i messaggi normali, la Retry Queue
            // li trattiene per 30 secondi prima di reinviarli alla Main Queue,
            // mentre la DLQ conserva i messaggi che hanno esaurito i retry.
            // I Binding collegano ogni Exchange alla propria Queue tramite routing key.

            // ── DLQ Exchange e Queue ──────────────────────────────────────
            await _channel!.ExchangeDeclareAsync(DlqExchange, ExchangeType.Direct, durable: true);
            await _channel.QueueDeclareAsync(DlqQueue, durable: true, exclusive: false, autoDelete: false);
            await _channel.QueueBindAsync(DlqQueue, DlqExchange, DlqQueue);

            // ── Retry Exchange e Queue (con TTL e DLX verso Main) ─────────
            await _channel.ExchangeDeclareAsync(RetryExchange, ExchangeType.Direct, durable: true);
            await _channel.QueueDeclareAsync(
                queue: RetryQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: new Dictionary<string, object?>
                {
                { "x-message-ttl", RetryDelayMs },
                { "x-dead-letter-exchange", MainExchange },
                { "x-dead-letter-routing-key", MainQueue }
                });
            await _channel.QueueBindAsync(RetryQueue, RetryExchange, RetryQueue);

            // ── Main Exchange e Queue ─────────────────────────────────────
            await _channel.ExchangeDeclareAsync(MainExchange, ExchangeType.Direct, durable: true);
            await _channel.QueueDeclareAsync(MainQueue, durable: true, exclusive: false, autoDelete: false);
            await _channel.QueueBindAsync(MainQueue, MainExchange, MainQueue);
        }

        protected int GetRetryCount(IReadOnlyBasicProperties props)
        {
            // Legge dall'header "x-retry-count" il numero di retry già effettuati.
            // Se l'header non esiste, restituisce 0 perché il messaggio
            // non è ancora stato ritentato.
            if (props.Headers != null &&
                props.Headers.TryGetValue("x-retry-count", out var value))
                return Convert.ToInt32(value);
            return 0;
        }

        protected async Task PublishToRetryAsync(string json, int retryCount)
        {
            // Ripubblica il messaggio nella Retry Queue, incrementando il contatore
            // dei retry negli header. Il messaggio attende 30 secondi (TTL),
            // poi RabbitMQ lo reinoltra automaticamente alla Main Queue.
            var body = Encoding.UTF8.GetBytes(json);
            var props = new BasicProperties
            {
                Persistent = true,
                Headers = new Dictionary<string, object?> { { "x-retry-count", retryCount } }
            };
            await _channel!.BasicPublishAsync(RetryExchange, RetryQueue, false, props, body);
        }

        protected async Task PublishToDlqAsync(string json, string errorMessage)
        {
            // Pubblica il messaggio nella Dead Letter Queue (DLQ) quando
            // vengono esauriti i retry. Aggiunge agli header il numero di retry,
            // il messaggio di errore e la data/ora del fallimento,
            // per facilitare l'analisi e l'eventuale recupero del messaggio.
            var body = Encoding.UTF8.GetBytes(json);
            var props = new BasicProperties
            {
                Persistent = true,
                Headers = new Dictionary<string, object?>
            {
                { "x-retry-count", MaxRetries },
                { "x-error-message", errorMessage },
                { "x-failed-at", DateTime.UtcNow.ToString("o") }
            }
            };
            await _channel!.BasicPublishAsync(DlqExchange, DlqQueue, false, props, body);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            // Gestisce l'arresto del servizio, chiudendo il channel RabbitMQ
            // e la connessione, se inizializzati, e richiamando il metodo
            // della classe base per completare lo shutdown del BackgroundService.
            if (_channel != null) await _channel.CloseAsync();
            if (_connection != null) await _connection.CloseAsync();
            await base.StopAsync(cancellationToken);
        }
    }
}
