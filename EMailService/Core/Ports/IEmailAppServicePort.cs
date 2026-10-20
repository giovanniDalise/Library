using Library.EmailService.Core.Domain.Events;

namespace Library.EmailService.Core.Ports
{
    public interface IEmailAppServicePort
    {
        Task SendConfirmationEmailAsync(UserRegisteredEvent @event);
        Task SendResetPasswordEmailAsync(PasswordResetEvent @event);

    }
}