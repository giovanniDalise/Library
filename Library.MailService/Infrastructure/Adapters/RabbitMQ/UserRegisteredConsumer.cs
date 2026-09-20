using System.Text;
using System.Text.Json;
using Library.EmailService.Core.Domain.Events;
using Library.EmailService.Core.Ports;
using Library.Logging.Abstractions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Library.EmailService.Infrastructure.Adapters.RabbitMQ
{
    public class UserRegisteredConsumer : BackgroundService
    {
        private readonly IEmailAppServicePort _emailAppService;
        private readonly ILoggerPort _logger;
        private readonly string _hostName;

        private const string MainQueue = "IdentityService_UserRegistered";
        private const string RetryQueue = "IdentityService_UserRegistered_Retry";
        private const string DlqQueue = "IdentityService_UserRegistered_DLQ";
        private const string MainExchange = "IdentityService_UserRegistered_Exchange";
        private const string RetryExchange = "IdentityService_UserRegistered_Retry_Exchange";
        private const string DlqExchange = "IdentityService_UserRegistered_DLQ_Exchange";
        private const int MaxRetries = 3;
        private const int RetryDelayMs = 30000; // 30 secondi

        private IConnection? _connection;
        private IChannel? _channel;

        public UserRegisteredConsumer(
            IEmailAppServicePort emailAppService,
            ILoggerPort logger,
            string hostName)
        {
            _emailAppService = emailAppService;
            _logger = logger;
            _hostName = hostName;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.Info("UserRegisteredConsumer - Starting");

            var factory = new ConnectionFactory { HostName = _hostName };
            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await SetupQueuesAsync();

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (_, ea) =>
            {
                var retryCount = GetRetryCount(ea.BasicProperties);
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());

                try
                {
                    var @event = JsonSerializer.Deserialize<UserRegisteredEvent>(json);
                    if (@event != null)
                    {
                        await _emailAppService.SendConfirmationEmailAsync(@event);
                        await _channel.BasicAckAsync(ea.DeliveryTag, false);
                        _logger.Info($"UserRegisteredConsumer - Processed | Email: {@event.Email}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error($"UserRegisteredConsumer - Error processing message | Attempt: {retryCount + 1}", ex);
                    await _channel.BasicAckAsync(ea.DeliveryTag, false);

                    if (retryCount < MaxRetries)
                    {
                        await PublishToRetryAsync(json, retryCount + 1);
                        _logger.Warn($"UserRegisteredConsumer - Sent to Retry | Attempt: {retryCount + 1}/{MaxRetries}");
                    }
                    else
                    {
                        await PublishToDlqAsync(json, ex.Message);
                        _logger.Error($"UserRegisteredConsumer - Max retries reached, sent to DLQ");
                    }
                }
            };

            await _channel.BasicConsumeAsync(MainQueue, autoAck: false, consumer);
            _logger.Info($"UserRegisteredConsumer - Listening on queue: {MainQueue}");

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private async Task SetupQueuesAsync()
        {
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
                    { "x-message-ttl", RetryDelayMs },           // attendi 30s
                    { "x-dead-letter-exchange", MainExchange },   // poi rimanda alla coda principale
                    { "x-dead-letter-routing-key", MainQueue }
                });
            await _channel.QueueBindAsync(RetryQueue, RetryExchange, RetryQueue);

            // ── Main Exchange e Queue ─────────────────────────────────────
            await _channel.ExchangeDeclareAsync(MainExchange, ExchangeType.Direct, durable: true);
            await _channel.QueueDeclareAsync(
                queue: MainQueue,
                durable: true,
                exclusive: false,
                autoDelete: false);
            await _channel.QueueBindAsync(MainQueue, MainExchange, MainQueue);
        }

        private int GetRetryCount(IReadOnlyBasicProperties props)
        {
            if (props.Headers != null &&
                props.Headers.TryGetValue("x-retry-count", out var value))
            {
                return Convert.ToInt32(value);
            }
            return 0;
        }

        private async Task PublishToRetryAsync(string json, int retryCount)
        {
            var body = Encoding.UTF8.GetBytes(json);
            var props = new BasicProperties
            {
                Persistent = true,
                Headers = new Dictionary<string, object?> { { "x-retry-count", retryCount } }
            };
            await _channel!.BasicPublishAsync(RetryExchange, RetryQueue, false, props, body);
        }

        private async Task PublishToDlqAsync(string json, string errorMessage)
        {
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
            if (_channel != null) await _channel.CloseAsync();
            if (_connection != null) await _connection.CloseAsync();
            await base.StopAsync(cancellationToken);
        }
    }
}