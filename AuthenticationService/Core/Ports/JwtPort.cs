namespace Library.AuthenticationService.Core.Ports
{
    public interface JwtPort
    {
        Task<string> GenerateJwtToken(string email, string role);
    }
}
