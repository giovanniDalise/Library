namespace Library.AuthenticationService.Core.Ports
{
    public interface AuthenticationRepositoryPort
    {
        Task<bool> CheckUserCredentials(string email, string password);
        Task<string> GetUserRole(string email);
    }
}
