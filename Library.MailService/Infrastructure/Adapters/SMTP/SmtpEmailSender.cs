using Library.EmailService.Core.Ports;
using Library.MailService.Infrastructure.Options;
using Library.Logging.Abstractions;
using MailKit.Net.Smtp;
using MimeKit;

namespace Library.MailService.Infrastructure.Adapters.SMTP
{
    public class SmtpEmailSender : IEmailSenderPort
    {
        private readonly EmailOptions _options;
        private readonly ILoggerPort _logger;

        public SmtpEmailSender(EmailOptions options, ILoggerPort logger)
        {
            _options = options;
            _logger = logger;
        }

        public async Task SendAsync(string to, string subject, string htmlBody)
        {
            _logger.Info($"SmtpEmailSender - Sending to: {to}");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Library", _options.FromAddress));
            message.To.Add(new MailboxAddress("", to));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = htmlBody };

            using var client = new SmtpClient();
            await client.ConnectAsync(_options.Host, _options.Port,
                MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_options.Username, _options.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.Info($"SmtpEmailSender - Sent successfully to: {to}");
        }
    }
}