using Library.UserService.Core.Domain.Models;
using Library.UserService.Core.Ports;

namespace Library.UserService.Core.Domain.Services
{
    public class UserService:UserServicePort
    {
        private readonly UserRepositoryPort _userRepositoryPort;
        private readonly IPasswordHasherPort _passwordHasherPort;

        public UserService(UserRepositoryPort userRepositoryPort, IPasswordHasherPort passwordHasherPort)
        {
            _userRepositoryPort = userRepositoryPort ?? throw new ArgumentNullException(nameof(userRepositoryPort));
            _passwordHasherPort = passwordHasherPort ?? throw new ArgumentNullException(nameof(passwordHasherPort));
        }
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userRepositoryPort.ReadAsync();
        }
        public async Task<User> GetUserByIdAsync(long id)
        {
            return await _userRepositoryPort.GetByIdAsync(id);
        }
        public async Task<long> CreateUserAsync(User user)
        {
            user.Password = _passwordHasherPort.Hash(user.Password);
            return await _userRepositoryPort.CreateAsync(user);
        }
        public async Task<long> UpdateUserAsync(long id, User user)
        {
            return await _userRepositoryPort.UpdateAsync(id, user);
        }
        public async Task<long> DeleteUserAsync(long id)
        {
            return await _userRepositoryPort.DeleteAsync(id);
        }
        public async Task<List<User>> GetUsersByTextAsync(string searchText)
        {
            return await _userRepositoryPort.FindByTextAsync(searchText);
        }
        //public async Task<List<User>> GetUsersByObjectAsync(User user)
        //{
        //    return await _userRepositoryPort.FindByObjectAsync(user);
        //}
    }
}
