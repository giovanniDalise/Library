namespace Library.EmailService.Core.Ports
{
    public interface IEmailServicePort
    {
        Task SendConfirmationEmailAsync(string to, string fullName, string confirmUrl, DateTime expiresAt);
    }
}