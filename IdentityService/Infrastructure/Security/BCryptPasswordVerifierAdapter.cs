using Library.IdentityService.Core.Ports;

namespace Library.IdentityService.Infrastructure.Adapters.Security
{
    public class BCryptPasswordVerifierAdapter : IPasswordVerifierPort
    {
        public bool Verify(string plainPassword, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);
        }
    }
}
