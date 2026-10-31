namespace Library.IdentityService.Core.Domain.Models
{
    public record UserLoginInfo(
        long Id,
        string Email,
        string Name,
        string Surname,
        string PasswordHash,
        string RoleName,
        bool IsConfirmed,
        DateTime? TokenExpiresAt);
}
