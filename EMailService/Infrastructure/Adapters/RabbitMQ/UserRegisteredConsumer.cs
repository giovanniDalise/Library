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

            await InitializeConnectionAsync();
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
                        await _channel!.BasicAckAsync(ea.DeliveryTag, false);
                        _logger.Info($"UserRegisteredConsumer - Processed | Email: {@event.Email}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error($"UserRegisteredConsumer - Error | Attempt: {retryCount + 1}", ex);
                    await _channel!.BasicAckAsync(ea.DeliveryTag, false);

                    if (retryCount < MaxRetries)
                    {
                        await PublishToRetryAsync(json, retryCount + 1);
                        _logger.Warn($"UserRegisteredConsumer - Sent to Retry | Attempt: {retryCount + 1}/{MaxRetries}");
                    }
                    else
                    {
                        await PublishToDlqAsync(json, ex.Message);
                        _logger.Error("UserRegisteredConsumer - Max retries reached, sent to DLQ");
                    }
                }
            };

            await _channel!.BasicConsumeAsync(MainQueue, autoAck: false, consumer);
            _logger.Info($"UserRegisteredConsumer - Listening on queue: {MainQueue}");
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}