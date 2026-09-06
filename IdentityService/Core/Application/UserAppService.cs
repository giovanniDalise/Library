using Library.IdentityService.Core.Domain.Models;
using Library.IdentityService.Core.Ports;

namespace Library.IdentityService.Core.Application
{
    public class UserAppService : IUserAppServicePort
    {
        private readonly IUserServicePort _userDomainService;
        private readonly IEventPublisherPort _eventPublisher;

        public UserAppService(
            IUserServicePort userDomainService,
            IEventPublisherPort eventPublisher)
        {
            _userDomainService = userDomainService;
            _eventPublisher = eventPublisher;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userDomainService.GetAllUsersAsync();
        }

        public async Task<User> GetUserByIdAsync(long id)
        {
            return await _userDomainService.GetUserByIdAsync(id);
        }

        public async Task<long> CreateUserAsync(User user)
        {
            // genera token di conferma
            user.ConfirmationToken = Guid.NewGuid().ToString("N");
            user.TokenExpiresAt = DateTime.UtcNow.AddHours(24);
            user.IsConfirmed = false;

            var id = await _userDomainService.CreateUserAsync(user);

            // pubblica evento per il MailService
            await _eventPublisher.PublishAsync(new UserRegisteredEvent
            {
                UserId = id,
                Email = user.Email,
                FullName = $"{user.Name} {user.Surname}",
                ConfirmationToken = user.ConfirmationToken,
                ExpiresAt = user.TokenExpiresAt.Value
            }, "user.registered");

            return id;
        }

        public async Task<long> UpdateUserAsync(long id, User user)
        {
            return await _userDomainService.UpdateUserAsync(id, user);
        }

        public async Task<long> DeleteUserAsync(long id)
        {
            return await _userDomainService.DeleteUserAsync(id);
        }

        public async Task<List<User>> GetUsersByTextAsync(string searchText)
        {
            return await _userDomainService.GetUsersByTextAsync(searchText);
        }
    }
}