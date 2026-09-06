using Library.EmailService.Core.Ports;
using Library.Logging.Abstractions;
using MailKit.Net.Smtp;
using MimeKit;

namespace Library.MailService.Infrastructure.Adapters.SMTP
{
    public class GmailEmailSender : IEmailSenderPort
    {
        private readonly string _fromEmail;
        private readonly string _appPassword;
        private readonly ILoggerPort _logger;

        public GmailEmailSender(string fromEmail, string appPassword, ILoggerPort logger)
        {
            _fromEmail = fromEmail;
            _appPassword = appPassword;
            _logger = logger;
        }

        public async Task SendAsync(string to, string subject, string htmlBody)
        {
            _logger.Info($"GmailEmailSender - Sending to: {to}");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Library", _fromEmail));
            message.To.Add(new MailboxAddress("", to));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = htmlBody };

            using var client = new SmtpClient(); 
            await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_fromEmail, _appPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.Info($"GmailEmailSender - Sent successfully to: {to}");
        }
    }
}