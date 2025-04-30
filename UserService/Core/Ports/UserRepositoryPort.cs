using Library.UserService.Core.Domain.Models;

namespace Library.UserService.Core.Ports
{
    public interface UserRepositoryPort
    {
        Task<List<User>> ReadAsync();
        Task<User> GetByIdAsync(long id);
        Task<long> CreateAsync(User user);
        Task<long> UpdateAsync(long id, User user);
        Task<long> DeleteAsync(long id);
        Task<List<User>> FindByTextAsync(string searchText);
        //Task<List<User>> FindByObjectAsync(User searchUser);
    }
}
