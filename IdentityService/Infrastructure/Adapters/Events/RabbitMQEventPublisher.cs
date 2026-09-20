using Library.IdentityService.Core.Ports;
using Library.Logging.Abstractions;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Library.IdentityService.Infrastructure.Adapters.Events
{
    public class RabbitMQEventPublisher : IEventPublisherPort, IAsyncDisposable
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private readonly ILoggerPort _logger;
        private const string UserRegisteredQueue = "IdentityService_UserRegistered";
        private const string UserRegisteredExchange = "IdentityService_UserRegistered_Exchange";

        private RabbitMQEventPublisher(IConnection connection, IChannel channel, ILoggerPort logger)
        {
            _connection = connection;
            _channel = channel;
            _logger = logger;
        }

        public static async Task<RabbitMQEventPublisher> CreateAsync(string hostName, ILoggerPort logger)
        {
            var factory = new ConnectionFactory { HostName = hostName };
            var connection = await factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();
            return new RabbitMQEventPublisher(connection, channel, logger);
        }

        public async Task PublishAsync<T>(T @event, string queueName)
        {
            // usa l'exchange invece della coda diretta
            var exchangeName = $"{queueName}_Exchange";

            await _channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Direct, durable: true);
            await _channel.QueueDeclareAsync(queueName, durable: true, exclusive: false, autoDelete: false);
            await _channel.QueueBindAsync(queueName, exchangeName, queueName);

            var json = JsonSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(json);
            var props = new BasicProperties { Persistent = true };

            await _channel.BasicPublishAsync(
                exchange: exchangeName,
                routingKey: queueName,
                mandatory: false,
                basicProperties: props,
                body: body);

            _logger.Info($"RabbitMQEventPublisher - Published to exchange: {exchangeName}");
        }

        public async ValueTask DisposeAsync()
        {
            await _channel.CloseAsync();
            await _connection.CloseAsync();
        }
    }
}

