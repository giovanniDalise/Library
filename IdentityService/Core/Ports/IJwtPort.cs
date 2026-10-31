using Library.IdentityService.Core.Domain.Models;

namespace Library.IdentityService.Core.Ports
{
    public interface IJwtPort
    {
        Task<string> GenerateJwtToken(JwtUserClaims user);
    }
}
