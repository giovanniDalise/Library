namespace Library.AuthenticationService.Core.Ports
{
    public interface IAuthenticationRepositoryPort
    {
        Task<bool> CheckUserCredentials(string email, string password);
        Task<string> GetUserRole(string email);
    }
}
