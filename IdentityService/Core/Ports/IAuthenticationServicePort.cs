using Library.IdentityService.Core.Domain.Models;

namespace Library.IdentityService.Core.Ports
{
    public interface IAuthenticationServicePort
    {
        Task<AuthResult> Authenticate(Credentials loginRequest);
    }
}
