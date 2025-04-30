using Library.UserService.Core.Domain.Models;

namespace Library.UserService.Core.Ports
{
    public interface UserServicePort
    {
        Task<List<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(long id);
        Task<long> CreateUserAsync(User user);
        Task<long> UpdateUserAsync(long id, User user);
        Task<long> DeleteUserAsync(long id);
        Task<List<User>> GetUsersByTextAsync(string searchText);
        //Task<List<User>> GetUsersByObjectAsync(User searchUser);
    }
}
