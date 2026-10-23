using Library.EmailService.Core.Domain.Events;
using Library.EmailService.Core.Ports;
using Library.Logging.Abstractions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Library.EmailService.Infrastructure.Adapters.RabbitMQ
{
    public class PasswordResetConfirmConsumer : BaseRabbitMQConsumer
    {
        private readonly IEmailAppServicePort _emailAppService;
        protected override string MainQueue => "IdentityService_PasswordResetConfirm";
        protected override string RetryQueue => "IdentityService_PasswordResetConfirm_Retry";
        protected override string DlqQueue => "IdentityService_PasswordResetConfirm_DLQ";
        protected override string MainExchange => "IdentityService_PasswordResetConfirm_Exchange";
        protected override string RetryExchange => "IdentityService_PasswordResetConfirm_Retry_Exchange";
        protected override string DlqExchange => "IdentityService_PasswordResetConfirm_DLQ_Exchange";

        public PasswordResetConfirmConsumer(
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
            _logger.Info("PasswordResetConfirmConsumer - Starting");

            await InitializeConnectionAsync();
            await SetupQueuesAsync();

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (_, ea) =>
            {
                var retryCount = GetRetryCount(ea.BasicProperties);
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());

                try
                {
                    var @event = JsonSerializer.Deserialize<PasswordResetConfirmEvent>(json);

                    if (@event != null)
                    {
                        await _emailAppService.SendPasswordResetConfirmEmailAsync(@event);

                        await _channel!.BasicAckAsync(ea.DeliveryTag, false);
                        _logger.Info($"PasswordResetConfirmConsumer - Processed | Email: {@event.Email}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error($"PasswordResetConfirmConsumer - Error | Attempt: {retryCount + 1}", ex);

                    await _channel!.BasicAckAsync(ea.DeliveryTag, false);

                    if (retryCount < MaxRetries)
                    {
                        await PublishToRetryAsync(json, retryCount + 1);
                        _logger.Warn($"PasswordResetConfirmConsumer - Sent to Retry | Attempt: {retryCount + 1}/{MaxRetries}");
                    }
                    else
                    {
                        await PublishToDlqAsync(json, ex.Message);
                        _logger.Error("PasswordResetConfirmConsumer - Max retries reached, sent to DLQ");
                    }
                }
            };
            await _channel!.BasicConsumeAsync(MainQueue, autoAck: false, consumer);

            _logger.Info($"PasswordResetConfirmConsumer - Listening on queue: {MainQueue}");

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}
