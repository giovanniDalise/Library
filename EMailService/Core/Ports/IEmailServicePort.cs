namespace Library.EmailService.Core.Ports
{
    public interface IEmailServicePort
    {
        Task SendConfirmationEmailAsync(string to, string fullName, string confirmUrl, DateTime expiresAt);
        Task SendResetPasswordEmailAsync(string to, string fullName, string resetUrl, DateTime expiresAt);
        Task SendPasswordResetConfirmEmailAsync(string to, string fullName, DateTime changedAt);
    }
}