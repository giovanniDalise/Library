using Library.IdentityService.Core.Domain.Models;

namespace Library.IdentityService.Core.Ports
{
    public interface IAuthenticationRepositoryPort
    {
        Task UpdateConfirmationTokenAsync(string email, string token, DateTime expiresAt);
        Task UpdateResetTokenAsync(string email, string resetToken, DateTime resetTokenExpiry);
        Task<bool> ConfirmUserAsync(string token);
        Task<(User? User, DateTime? ResetTokenExpiresAt)> GetUserByResetTokenAsync(string token);
        Task UpdateForgottenPasswordAsync(long? userId, string hashedPassword);
        Task<UserLoginInfo?> GetLoginInfoByEmailAsync(string email);

    }
}
