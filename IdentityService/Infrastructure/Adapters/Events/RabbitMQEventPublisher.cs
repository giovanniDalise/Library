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
            await _channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false);

            var json = JsonSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(json);

            var props = new BasicProperties { Persistent = true };

            await _channel.BasicPublishAsync(
                exchange: "",
                routingKey: queueName,
                mandatory: false,
                basicProperties: props,
                body: body);

            _logger.Info($"RabbitMQEventPublisher - Published to queue: {queueName}");
        }

        public async ValueTask DisposeAsync()
        {
            await _channel.CloseAsync();
            await _connection.CloseAsync();
        }
    }
}

