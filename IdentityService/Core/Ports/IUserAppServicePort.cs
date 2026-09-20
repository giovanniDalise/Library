using Library.IdentityService.Core.Domain.Models;

namespace Library.IdentityService.Core.Ports
{
    public interface IUserAppServicePort
    {
        Task<List<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(long id);
        Task<long> CreateUserAsync(User user);
        Task<long> UpdateUserAsync(long id, User user);
        Task<long> DeleteUserAsync(long id);
        Task<List<User>> GetUsersByTextAsync(string searchText);
    }
}