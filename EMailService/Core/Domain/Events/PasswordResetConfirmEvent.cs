namespace Library.EmailService.Core.Domain.Events
{
    public class PasswordResetConfirmEvent
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public DateTime ChangedAt { get; set; }
    }
}
