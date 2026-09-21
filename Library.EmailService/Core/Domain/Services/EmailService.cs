using Library.EmailService.Core.Ports;
using Library.Logging.Abstractions;

namespace Library.EmailService.Core.Domain.Services
{
    public class EmailService : IEmailServicePort
    {
        private readonly IEmailSenderPort _emailSender;
        private readonly ILoggerPort _logger;

        public EmailService(IEmailSenderPort emailSender, ILoggerPort logger)
        {
            _emailSender = emailSender;
            _logger = logger;
        }

        public async Task SendConfirmationEmailAsync(string to, string fullName, string confirmUrl, DateTime expiresAt)
        {
            _logger.Info($"MailService - SendConfirmationEmailAsync | To: {to}");

            var templatePath = Path.Combine(AppContext.BaseDirectory, "Templates", "ConfirmationEmail.html");
            var template = await File.ReadAllTextAsync(templatePath);

            var body = template
                .Replace("{{FullName}}", fullName)
                .Replace("{{ConfirmUrl}}", confirmUrl)
                .Replace("{{ExpiresAt}}", expiresAt.ToString("dd/MM/yyyy HH:mm"));

            await _emailSender.SendAsync(to, "Confirm your registration", body);

            _logger.Info($"MailService - Completed | To: {to}");
        }
    }
}