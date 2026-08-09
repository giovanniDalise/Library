namespace Library.IdentityService.Core.Ports
{
    public interface IJwtPort
    {
        Task<string> GenerateJwtToken(string email, string role);
    }
}
