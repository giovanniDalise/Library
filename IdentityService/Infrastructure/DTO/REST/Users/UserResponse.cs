namespace Library.IdentityService.Infrastructure.DTO.REST.Users
{
    public class UserResponse
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public long Role { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ProfileImageUrl { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
