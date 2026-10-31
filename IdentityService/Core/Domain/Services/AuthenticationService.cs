
using Library.IdentityService.Core.Domain.Models;
using Library.IdentityService.Core.Domain.Models.Events;
using Library.IdentityService.Core.Exceptions;
using Library.IdentityService.Core.Ports;

namespace Library.IdentityService.Core.Domain.Services
{
    public class AuthenticationService: IAuthenticationServicePort
    {
        private readonly IJwtPort _jwtPort;
        private readonly IAuthenticationRepositoryPort _repositoryPort;
        private readonly IEventPublisherPort _eventPublisher;
        private readonly IPasswordHasherPort _passwordHasherPort;
        private readonly IPasswordVerifierPort _passwordVerifier;
        private static string? _dummyHash;
        //Il dummy hash è facoltativo e riguarda un problema molto specifico.
        //Verificare una password con bcrypt è lento di proposito, qualche decina
        //o centinaio di millisecondi.Senza il dummy, succede questo:
        //Email inesistente: il metodo ritorna subito, risposta velocissima.
        //Email esistente con password sbagliata: il metodo fa il verify bcrypt, risposta più lenta.
        //Chi vuole sapere se un’email è registrata può misurare i tempi di risposta
        //e capire la differenza, anche se il messaggio d’errore è identico.
        //Con il dummy, se l’utente non esiste si fa comunque un verify contro
        //un hash finto, quindi i due casi impiegano più o meno lo stesso tempo.

        public AuthenticationService(
            IJwtPort jwtPort,
            IAuthenticationRepositoryPort authenticationRepositoryPort,
            IEventPublisherPort eventPublisher,
            IPasswordHasherPort passwordHasherPort,
            IPasswordVerifierPort passwordVerifier
        )
        {
            _jwtPort = jwtPort;
            _repositoryPort = authenticationRepositoryPort;
            _eventPublisher = eventPublisher;
            _passwordHasherPort = passwordHasherPort;
            _passwordVerifier = passwordVerifier;
        }

        public async Task<AuthResult?> Authenticate(Credentials loginRequest)
        {
            var user = await _repositoryPort.GetLoginInfoByEmailAsync(loginRequest.Email);

            // Verifica sempre un hash, anche se l'utente non esiste,
            // così i tempi di risposta non rivelano se l'email è registrata.
            _dummyHash ??= _passwordHasherPort.Hash("dummy-password");
            bool passwordValid = _passwordVerifier.Verify(
                loginRequest.Password, user?.PasswordHash ?? _dummyHash);

            if (user is null || !passwordValid) return null;

            if (!user.IsConfirmed)
                await HandleUnconfirmedUserAsync(user);

            string jwtToken = await _jwtPort.GenerateJwtToken(
                new JwtUserClaims(user.Id, user.Email, user.RoleName));

            return new AuthResult { Token = jwtToken };
        }
        private async Task HandleUnconfirmedUserAsync(UserLoginInfo user)
        {
            if (user.TokenExpiresAt is null || user.TokenExpiresAt < DateTime.UtcNow)
            {
                var newToken = Guid.NewGuid().ToString("N");
                var newExpiry = DateTime.UtcNow.AddHours(24);

                await _repositoryPort.UpdateConfirmationTokenAsync(user.Email, newToken, newExpiry);

                await _eventPublisher.PublishAsync(new UserRegisteredEvent
                {
                    UserId = user.Id,
                    Email = user.Email,
                    FullName = $"{user.Name} {user.Surname}",
                    ConfirmationToken = newToken,
                    ExpiresAt = newExpiry
                }, "IdentityService_UserRegistered");

                throw new EmailNotConfirmedException(
                    "Email not confirmed. We sent you a new confirmation link.");
            }

            throw new EmailNotConfirmedException(
                "Email not confirmed. Please check your inbox.");
        }
        public async Task<bool> ConfirmUserAsync(string token)
        {
            return await _repositoryPort.ConfirmUserAsync(token);
        }

        public async Task ForgotPasswordAsync(string email)
        {
            var user = await _repositoryPort.GetLoginInfoByEmailAsync(email);
            if (user == null) return;

            if (!user.IsConfirmed)
                await HandleUnconfirmedUserAsync(user);

            var resetToken = Guid.NewGuid().ToString("N");
            var resetTokenExpiry = DateTime.UtcNow.AddHours(24);

            await _repositoryPort.UpdateResetTokenAsync(user.Email, resetToken, resetTokenExpiry);
            // Token di conferma e di reset su colonne separate: cicli di vita indipendenti.

            await _eventPublisher.PublishAsync(new PasswordResetEvent
            {
                Email = user.Email,
                FullName = $"{user.Name} {user.Surname}",
                ResetToken = resetToken,
                ExpiresAt = resetTokenExpiry
            }, "IdentityService_ResetPassword");
            // Coda separata da UserRegistered: isolamento dei fallimenti, DLQ distinte, monitoring chiaro.
        }
        public async Task ResetPasswordAsync(string token, string newPassword)
        {
            var (user, resetTokenExpiresAt) = await _repositoryPort.GetUserByResetTokenAsync(token);

            if (user == null)
                throw new InvalidResetTokenException("Token not valid");
            if(resetTokenExpiresAt < DateTime.UtcNow)
                throw new InvalidResetTokenException("Token expired");
            //ora corrente in UTC (Coordinated Universal Time) — il fuso orario standard internazionale senza offset.
            //Lo usi perché quando hai salvato reset_token_expires_at nel DB hai usato DateTime.UtcNow.AddHours(24)
            //quindi il confronto deve essere fatto con lo stesso riferimento temporale.
            //Se usassi DateTime.Now(ora locale) e il server fosse in un fuso orario diverso dal client avresti
            //un confronto sbagliato.Quindi se la data expired è più piccola di quella di ora allora il token è scaduto.

            var hashedPassword = _passwordHasherPort.Hash(newPassword);

            await _repositoryPort.UpdateForgottenPasswordAsync(user.Id, hashedPassword);

            await _eventPublisher.PublishAsync(new PasswordResetConfirmEvent
            {
                Email = user.Email,
                FullName = $"{user.Name} {user.Surname},",
                ChangedAt = DateTime.UtcNow
            }, "IdentityService_PasswordResetConfirm");
        }

    }
}
