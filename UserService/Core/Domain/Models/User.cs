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


        public User() { }

        public User(long? id, string name, string surname, string email, string password, long role)
        {
            Id = id;
            Name = name;
            Surname = surname;
            Email = email;
            Password = password;
            Role = role;
        }

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
