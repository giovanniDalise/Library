using Library.AuthenticationService.Core.Domain.Models;

namespace Library.BookService.Core.Ports
{
    public interface AuthenticationServicePort
    {
        Task<AuthResponse> Authenticate(LoginRequest loginRequest);
    }
}
