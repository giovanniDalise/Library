using Library.EmailService.Core.Ports;
using Library.Logging.Abstractions;
using Library.EmailService.Core.Domain.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client.Events;

namespace Library.EmailService.Infrastructure.Adapters.RabbitMQ
{
    public class UserRegisteredConsumer : BackgroundService
    {
        private readonly IEmailAppServicePort _emailAppService;
        private readonly ILoggerPort _logger;
        private readonly string _hostName;
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

            await _channel.QueueDeclareAsync(
                queue: "user.registered",
                durable: true,
                exclusive: false,
                autoDelete: false);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (_, ea) =>
            {
                try
                {
                    var json = Encoding.UTF8.GetString(ea.Body.ToArray());
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
                    _logger.Error("UserRegisteredConsumer - Error processing message", ex);
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, requeue: true);
                }
            };

            await _channel.BasicConsumeAsync("user.registered", autoAck: false, consumer);
            _logger.Info("UserRegisteredConsumer - Listening on queue: user.registered");

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_channel != null) await _channel.CloseAsync();
            if (_connection != null) await _connection.CloseAsync();
            await base.StopAsync(cancellationToken);
        }
    }
}