using Library.AuthenticationService.Core.Ports;
using BCrypt.Net;

namespace Library.AuthenticationService.Infrastructure.Adapters.Security
{
    public class BCryptPasswordVerifierAdapter : IPasswordVerifierPort
    {
        public bool Verify(string plainPassword, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);
        }
    }
}
