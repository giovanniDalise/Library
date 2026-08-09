using Library.IdentityService.Core.Domain.Models;

namespace Library.IdentityService.Core.Ports
{
    public interface IUserRepositoryPort
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
