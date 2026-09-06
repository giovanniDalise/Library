namespace Library.IdentityService.Core.Domain.Models.Events
{
    public class UserRegisteredEvent
    {
        public long UserId { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string ConfirmationToken { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
