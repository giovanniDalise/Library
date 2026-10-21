using Library.IdentityService.Core.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Library.IdentityService.Core.Ports
{
    public interface IAuthenticationServicePort
    {
        Task<AuthResult> Authenticate(Credentials loginRequest);
        Task<bool> ConfirmUserAsync(string token);
        Task ForgotPasswordAsync(string email);
        Task ResetPasswordAsync(string token, string newPassword);
    }
}
