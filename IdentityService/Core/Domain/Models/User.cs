namespace Library.IdentityService.Core.Domain.Models
{
    public class User
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public long Role { get; set; }
        public string? ConfirmationToken { get; set; }
        public DateTime? TokenExpiresAt { get; set; }
        public bool IsConfirmed { get; set; }
    }
}
