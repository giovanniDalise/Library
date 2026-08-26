namespace Library.IdentityService.Infrastructure.DTO.REST.Users
{
    public class UserRequest
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public long Role { get; set; }
    }
}
