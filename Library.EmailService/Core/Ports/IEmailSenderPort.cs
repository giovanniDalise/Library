namespace Library.EmailService.Core.Ports
{
    public interface IEmailSenderPort
    {
        Task SendAsync(string to, string subject, string htmlBody);
    }
}