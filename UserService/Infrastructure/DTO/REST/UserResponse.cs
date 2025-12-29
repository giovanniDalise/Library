namespace Library.UserService.Infrastructure.DTO.REST
{
    public class UserResponse
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public long Role { get; set; }
    }
}
