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

        public async Task SendResetPasswordEmailAsync(string to, string fullName, string resetUrl, DateTime expiresAt)
        {
            _logger.Info($"EmailService - SendResetPasswordEmailAsync | To: {to}");

            var templatePath = Path.Combine(AppContext.BaseDirectory, "Templates", "ResetPasswordEmail.html");
            var template = await File.ReadAllTextAsync(templatePath);

            var body = template
                .Replace("{{FullName}}", fullName)
                .Replace("{{ResetUrl}}", resetUrl)
                .Replace("{{ExpiresAt}}", expiresAt.ToString("dd/MM/yyyy HH:mm"));

            await _emailSender.SendAsync(to, "Reset your password", body);
            _logger.Info($"EmailService - Completed | To: {to}");
        }
        public async Task SendPasswordResetConfirmEmailAsync(string to, string fullName, DateTime changedAt)
        {
            _logger.Info($"EmailService - SendPasswordResetConfirmEmailAsync | To: {to}");

            var templatePath = Path.Combine(AppContext.BaseDirectory, "Templates", "PasswordResetConfirmEmail.html");
            var template = await File.ReadAllTextAsync(templatePath);

            var body = template
                .Replace("{{FullName}}", fullName)
                .Replace("{{ChangedAt}}", changedAt.ToString("dd/MM/yyyy HH:mm"));

            await _emailSender.SendAsync(to, "Your password has been changed", body);
            _logger.Info($"EmailService - Completed | To: {to}");
        }
    }
}