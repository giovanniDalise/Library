using Library.UserService.Core.Ports;
using BCrypt.Net;
using Org.BouncyCastle.Crypto.Generators;

namespace Library.UserService.Infrastructure.Adapters
{
    public class BCryptPasswordHasherAdapter : IPasswordHasherPort
    {
        public string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
