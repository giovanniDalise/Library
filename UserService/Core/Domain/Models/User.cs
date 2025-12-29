namespace Library.UserService.Core.Domain.Models
{
    public class User
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public long Role { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is User user &&
                   Name == user.Name &&
                   Surname == user.Surname &&
                   Email == user.Email &&
                   Password == user.Password &&
                   Role == user.Role;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Surname, Email, Password, Role);
        }

        public override string ToString()
        {
            return $"{Name} {Surname} {Email} {Password} {Role}";
        }
    }
}
