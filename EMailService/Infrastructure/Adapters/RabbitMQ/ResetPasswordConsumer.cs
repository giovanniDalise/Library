using Library.EmailService.Core.Ports;
using Library.Logging.Abstractions;
using RabbitMQ.Client;
using System.Text;

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
        }
    }
}
