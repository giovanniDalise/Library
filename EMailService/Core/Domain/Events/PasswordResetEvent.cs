namespace Library.EmailService.Core.Domain.Events
{
    public class PasswordResetEvent
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string ResetToken { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
