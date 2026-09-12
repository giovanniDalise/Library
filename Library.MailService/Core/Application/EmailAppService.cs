
using Library.EmailService.Core.Domain.Events;
using Library.EmailService.Core.Ports;
using Library.Logging.Abstractions;

namespace Library.EmailService.Core.Application
{
    public class EmailAppService : IEmailAppServicePort
    {
        private readonly IEmailSenderPort _emailSender;
        private readonly ILoggerPort _logger;

        public EmailAppService(IEmailSenderPort emailSender, ILoggerPort logger)
        {
            _emailSender = emailSender;
            _logger = logger;
        }

        public async Task SendConfirmationEmailAsync(UserRegisteredEvent @event)
        {
            _logger.Info($"SendConfirmationEmailAsync - Start | UserId: {@event.UserId}");

            var confirmUrl = $"http://localhost:4200/confirm-email?token={@event.ConfirmationToken}";

            var body = await BuildEmailBodyAsync(@event.FullName, confirmUrl, @event.ExpiresAt);

            await _emailSender.SendAsync(
                to: @event.Email,
                subject: "Confirm your registration",
                htmlBody: body);

            _logger.Info($"SendConfirmationEmailAsync - Completed | Email: {@event.Email}");
        }

        private async Task<string> BuildEmailBodyAsync(string fullName, string confirmUrl, DateTime expiresAt)
        {
            var templatePath = Path.Combine(AppContext.BaseDirectory, "Templates", "ConfirmationEmail.html");
            var template = await File.ReadAllTextAsync(templatePath);

            return template
                .Replace("{{FullName}}", fullName)
                .Replace("{{ConfirmUrl}}", confirmUrl)
                .Replace("{{ExpiresAt}}", expiresAt.ToString("dd/MM/yyyy HH:mm"));
        }
    }
}