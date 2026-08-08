namespace Library.IdentityService.Core.Ports
{
    public interface IAuthenticationRepositoryPort
    {
        Task<bool> CheckUserCredentials(string email, string password);
        Task<string> GetUserRole(string email);
    }
}
