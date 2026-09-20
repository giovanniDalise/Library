using Library.IdentityService.Core.Domain.Models;

namespace Library.IdentityService.Core.Ports
{
    public interface IAuthenticationRepositoryPort
    {
        Task<bool> CheckUserCredentials(string email, string password);
        Task<string> GetUserRole(string email);
        Task<(bool IsConfirmed, string? Token, DateTime? ExpiresAt)> GetUserConfirmationInfoAsync(string email);
        Task UpdateConfirmationTokenAsync(string email, string token, DateTime expiresAt);
        Task<bool> ConfirmUserAsync(string token);
        Task<User> GetByEmailAsync(string email);
    }
}
