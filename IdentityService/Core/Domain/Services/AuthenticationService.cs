
using Library.IdentityService.Core.Domain.Models;
using Library.IdentityService.Core.Domain.Models.Events;
using Library.IdentityService.Core.Exceptions;
using Library.IdentityService.Core.Ports;
using Library.IdentityService.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.VisualBasic;
using Mysqlx.Session;
using System.Runtime.ConstrainedExecution;
using System.Security.Principal;

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
                    //N formato token senza trattini
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
                    }, "IdentityService_UserRegistered");

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

        public async Task ForgotPasswordAsync(string email)
        {
            var user = await _repositoryPort.GetByEmailAsync(email);
            if (user == null)
            {
                return;
            }
            var (isConfirmed, token, expiresAt) =  await _repositoryPort.GetUserConfirmationInfoAsync(email);

            if (!isConfirmed)
            {
                if (expiresAt == null || expiresAt < DateTime.UtcNow)
                {
                    // token scaduto — genera nuovo e reinvia mail
                    var newToken = Guid.NewGuid().ToString("N");
                    //N formato token senza trattini
                    var newExpiry = DateTime.UtcNow.AddHours(24);

                    await _repositoryPort.UpdateConfirmationTokenAsync(email, newToken, newExpiry);


                    await _eventPublisher.PublishAsync(new UserRegisteredEvent
                    {
                        UserId = user.Id ?? 0,
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

            var resetToken = Guid.NewGuid().ToString("N");
            var resetTokenExpiry = DateTime.UtcNow.AddHours(24);

            await _repositoryPort.UpdateResetTokenAsync(email, resetToken, resetTokenExpiry);
            //Abbiamo due token diversi (confirmation_token e reset_token)sono due operazioni di sicurezza
            //con cicli di vita indipendenti. Se usassi la stessa colonna, un utente non ancora confermato
            //che richiede anche un reset password vedrebbe il proprio token di conferma sovrascritto da
            //quello di reset — perdendo la possibilità di confermare l’account.
            //Con colonne separate i due processi coesistono senza interferirsi.

            await _eventPublisher.PublishAsync(new PasswordResetEvent
            {
                Email = user.Email,
                FullName = $"{user.Name} {user.Surname}",
                ResetToken = resetToken,
                ExpiresAt = resetTokenExpiry
            }, "IdentityService_PasswordReset");
            //Coda separata(IdentityService_PasswordReset vs IdentityService_UserRegistered)
            //La logica è identica tra l'execute delle due code, unica cosa che cambia sono i template.
            //Il vantaggio principale è la scalabilità e l’isolamento dei fallimenti.Se il template dell’email 
            //di reset è rotto o il processo di reset ha un bug, i messaggi di registrazione continuano a 
            //essere processati normalmente — le DLQ sono separate quindi i messaggi morti di un flusso non inquinano l’altro.
            //Inoltre in futuro potresti voler gestire i due tipi di email con priorità diverse — ad esempio 
            //il reset password è più urgente della conferma registrazione, quindi potresti avere un 
            //consumer più aggressivo sul retry del reset.Con code separate puoi farlo senza toccare la logica dell’altro consumer.
            //Infine sulla management UI di RabbitMQ hai visibilità chiara su quanti messaggi di ogni tipo sono 
            //in coda, in retry o in DLQ — informazione preziosa per il monitoring e il debugging.
        }
    }
}
