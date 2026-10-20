using Library.EmailService.Core.Domain.Events;
using Library.EmailService.Core.Ports;
using Library.Logging.Abstractions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Library.EmailService.Infrastructure.Adapters.RabbitMQ
{
    public class ResetPasswordConsumer : BaseRabbitMQConsumer
    {
        private readonly IEmailAppServicePort _emailAppService;
        protected override string MainQueue => "IdentityService_ResetPassword";
        protected override string RetryQueue  => "IdentityService_ResetPassword_Retry";
        protected override string DlqQueue  => "IdentityService_ResetPassword_DLQ";
        protected override string MainExchange => "IdentityService_ResetPassword_Exchange";
        protected override string RetryExchange => "IdentityService_ResetPassword_Retry_Exchange";
        protected override string DlqExchange  => "IdentityService_ResetPassword_DLQ_Exchange";

        public ResetPasswordConsumer(
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
            _logger.Info("ResetPasswordConsumer - Starting");

            await InitializeConnectionAsync();
            await SetupQueuesAsync();

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (_, ea) =>
            {
                var retryCount = GetRetryCount(ea.BasicProperties);
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());

                try
                {
                    var @event = JsonSerializer.Deserialize<PasswordResetEvent>(json);

                    if (@event != null)
                    {
                        await _emailAppService.SendResetPasswordEmailAsync(@event);

                        await _channel!.BasicAckAsync(ea.DeliveryTag, false);
                        _logger.Info($"ResetPasswordConsumer - Processed | Email: {@event.Email}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error($"ResetPasswordConsumer - Error | Attempt: {retryCount + 1}", ex);

                    await _channel!.BasicAckAsync(ea.DeliveryTag, false);

                    if (retryCount < MaxRetries)
                    {
                        await PublishToRetryAsync(json, retryCount + 1);
                        _logger.Warn($"ResetPasswordConsumer - Sent to Retry | Attempt: {retryCount + 1}/{MaxRetries}");
                    }
                    else
                    {
                        await PublishToDlqAsync(json, ex.Message);
                        _logger.Error("ResetPasswordConsumer - Max retries reached, sent to DLQ");
                    }
                }
            };
            await _channel!.BasicConsumeAsync(MainQueue, autoAck: false, consumer);

            _logger.Info($"ResetPasswordConsumer - Listening on queue: {MainQueue}");

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}
