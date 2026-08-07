using Library.IdentityService.Core.Ports;

namespace Library.IdentityService.Infrastructure.Adapters
{
    public class BCryptPasswordHasherAdapter : IPasswordHasherPort
    {
        public string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
