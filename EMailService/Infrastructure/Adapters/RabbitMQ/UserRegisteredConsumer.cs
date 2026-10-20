using Library.EmailService.Core.Domain.Events;
using Library.EmailService.Core.Ports;
using Library.Logging.Abstractions;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.Json;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace Library.EmailService.Infrastructure.Adapters.RabbitMQ
{
    public class UserRegisteredConsumer : BaseRabbitMQConsumer
    {
        private readonly IEmailAppServicePort _emailAppService;

        protected override string MainQueue => "IdentityService_UserRegistered";
        protected override string RetryQueue => "IdentityService_UserRegistered_Retry";
        protected override string DlqQueue => "IdentityService_UserRegistered_DLQ";
        protected override string MainExchange => "IdentityService_UserRegistered_Exchange";
        protected override string RetryExchange => "IdentityService_UserRegistered_Retry_Exchange";
        protected override string DlqExchange => "IdentityService_UserRegistered_DLQ_Exchange";

        public UserRegisteredConsumer(
            IEmailAppServicePort emailAppService,
            ILoggerPort logger,
            string hostName,
            string userName,
            string password) : base(logger, hostName, userName, password)
        {
            _emailAppService = emailAppService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.Info("UserRegisteredConsumer - Starting");

            // Crea la connessione e il channel per comunicare con RabbitMQ.
            await InitializeConnectionAsync();

            // Crea e configura gli Exchange, le Queue e i relativi Binding.
            await SetupQueuesAsync();

            // Crea un consumer asincrono che riceverà i messaggi da RabbitMQ.
            var consumer = new AsyncEventingBasicConsumer(_channel);

            // Registra una funzione che verrà eseguita ogni volta
            // che RabbitMQ consegna un messaggio al consumer.
            // "_" rappresenta il mittente, mentre "ea" contiene i dati del messaggio.
            consumer.ReceivedAsync += async (_, ea) =>
            {
                // Legge dall'header il numero di retry già effettuati.
                var retryCount = GetRetryCount(ea.BasicProperties);

                // Converte il corpo del messaggio da byte a stringa JSON.
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());

                try
                {
                    // Deserializza il JSON trasformandolo in un oggetto UserRegisteredEvent.
                    var @event = JsonSerializer.Deserialize<UserRegisteredEvent>(json);

                    // Verifica che la deserializzazione abbia prodotto un oggetto valido.
                    if (@event != null)
                    {
                        // Invia l'email di conferma all'utente registrato.
                        // Se l'operazione fallisce con un'eccezione, passa al catch.
                        await _emailAppService.SendConfirmationEmailAsync(@event);

                        // Conferma a RabbitMQ che il messaggio è stato elaborato.
                        // false significa che viene confermato solo questo messaggio.
                        await _channel!.BasicAckAsync(ea.DeliveryTag, false);
                        _logger.Info($"UserRegisteredConsumer - Processed | Email: {@event.Email}");
                    }
                }
                catch (Exception ex)
                {
                    // Registra l'errore e il numero del tentativo corrente.
                    _logger.Error($"UserRegisteredConsumer - Error | Attempt: {retryCount + 1}", ex);

                    // Conferma il messaggio originale, evitando che RabbitMQ
                    // lo riconsegni automaticamente alla Main Queue.
                    await _channel!.BasicAckAsync(ea.DeliveryTag, false);

                    // Se non abbiamo ancora raggiunto il numero massimo di retry...
                    if (retryCount < MaxRetries)
                    {
                        // Pubblica una nuova copia del messaggio nella Retry Queue,
                        // incrementando il contatore dei tentativi.
                        await PublishToRetryAsync(json, retryCount + 1);
                        _logger.Warn($"UserRegisteredConsumer - Sent to Retry | Attempt: {retryCount + 1}/{MaxRetries}");
                    }
                    else
                    {
                        // Se i retry sono esauriti, pubblica il messaggio nella DLQ
                        // aggiungendo le informazioni relative all'errore.
                        await PublishToDlqAsync(json, ex.Message);
                        _logger.Error("UserRegisteredConsumer - Max retries reached, sent to DLQ");
                    }
                }
            };

            // Avvia il consumer sulla Main Queue.
            // autoAck: false significa che il messaggio deve essere confermato
            // esplicitamente dal codice tramite BasicAckAsync.
            await _channel!.BasicConsumeAsync(MainQueue, autoAck: false, consumer);

            // Registra nei log che il consumer è in ascolto sulla Main Queue.
            _logger.Info($"UserRegisteredConsumer - Listening on queue: {MainQueue}");

            // Mantiene attivo il servizio senza occupare inutilmente la CPU.
            // L'attesa termina quando stoppingToken viene cancellato durante lo shutdown.
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}