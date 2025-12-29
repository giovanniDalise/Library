using Library.AuthenticationService.Core.Domain.Models;
using Library.AuthenticationService.Core.Ports;
using Library.BookService.Core.Ports;

namespace Library.AuthenticationService.Core.Domain.Services
{
    public class AuthenticationService: AuthenticationServicePort
    {
        private readonly JwtPort _jwtPort;
        private readonly AuthenticationRepositoryPort _repositoryPort;

        public AuthenticationService(JwtPort jwtPort, AuthenticationRepositoryPort authenticationRepositoryPort)
        {
            _jwtPort = jwtPort;
            _repositoryPort = authenticationRepositoryPort;
        }

        public async Task<AuthResult> Authenticate (Credentials loginRequest)
        {
            bool authenticated = await _repositoryPort.CheckUserCredentials(loginRequest.Email, loginRequest.Password);
            if (authenticated)
            {
                string role = await _repositoryPort.GetUserRole(loginRequest.Email);
                string token = await _jwtPort.GenerateJwtToken(loginRequest.Email, role);
                return new AuthResult() { Token = token };

            }
            return null;
        }
    }
}
