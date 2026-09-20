
using Library.IdentityService.Core.Domain.Models;
using Library.IdentityService.Core.Domain.Models.Events;
using Library.IdentityService.Core.Ports;
using Library.IdentityService.Infrastructure.Exceptions;

namespace Library.IdentityService.Core.Domain.Services
{
    public class AuthenticationService: IAuthenticationServicePort
    {
        private readonly IJwtPort _jwtPort;
        private readonly IAuthenticationRepositoryPort _repositoryPort;
        private readonly IEventPublisherPort _eventPublisher;

        public AuthenticationService(
            IJwtPort jwtPort,
            IAuthenticationRepositoryPort authenticationRepositoryPort,
            IEventPublisherPort eventPublisher
        )
        {
            _jwtPort = jwtPort;
            _repositoryPort = authenticationRepositoryPort;
            _eventPublisher = eventPublisher;
        }

        public async Task<AuthResult> Authenticate(Credentials loginRequest)
        {
            bool authenticated = await _repositoryPort.CheckUserCredentials(
                loginRequest.Email, loginRequest.Password);

            if (!authenticated) return null;

            // credenziali ok — controlla conferma email
            var (isConfirmed, token, expiresAt) =
                await _repositoryPort.GetUserConfirmationInfoAsync(loginRequest.Email);

            if (!isConfirmed)
            {
                if (expiresAt == null || expiresAt < DateTime.UtcNow)
                {
                    // token scaduto — genera nuovo e reinvia mail
                    var newToken = Guid.NewGuid().ToString("N");
                    var newExpiry = DateTime.UtcNow.AddHours(24);

                    await _repositoryPort.UpdateConfirmationTokenAsync(
                        loginRequest.Email, newToken, newExpiry);

                    var user = await _repositoryPort.GetByEmailAsync(loginRequest.Email);

                    await _eventPublisher.PublishAsync(new UserRegisteredEvent
                    {
                        UserId = user.Id ?? 0,
                        Email = user.Email,
                        FullName = $"{user.Name} {user.Surname}",
                        ConfirmationToken = newToken,
                        ExpiresAt = newExpiry
                    }, "user.registered");

                    throw new EmailNotConfirmedException(
                        "Email not confirmed. We sent you a new confirmation link.");
                }

                throw new EmailNotConfirmedException(
                    "Email not confirmed. Please check your inbox.");
            }

            string role = await _repositoryPort.GetUserRole(loginRequest.Email);
            string jwtToken = await _jwtPort.GenerateJwtToken(loginRequest.Email, role);
            return new AuthResult { Token = jwtToken };
        }
        public async Task<bool> ConfirmUserAsync(string token)
        {
            return await _repositoryPort.ConfirmUserAsync(token);
        }
    }
}
