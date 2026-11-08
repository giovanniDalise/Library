using Library.IdentityService.Core.Domain.Models;
using Library.IdentityService.Core.Exceptions;
using Library.IdentityService.Core.Ports;

namespace Library.IdentityService.Core.Domain.Services
{
    public class UserService:IUserServicePort
    {
        private readonly IUserRepositoryPort _userRepositoryPort;
        private readonly IPasswordHasherPort _passwordHasherPort;
        private readonly IProfileImageStoragePort _imageStorage;

        private static readonly Dictionary<string, string> AllowedImageTypes =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ["image/jpeg"] = ".jpg",
                ["image/png"] = ".png",
                ["image/webp"] = ".webp"
            };
        public UserService(IUserRepositoryPort userRepositoryPort, IPasswordHasherPort passwordHasherPort, IProfileImageStoragePort imageStorage)
        {
            _userRepositoryPort = userRepositoryPort ?? throw new ArgumentNullException(nameof(userRepositoryPort));
            _passwordHasherPort = passwordHasherPort ?? throw new ArgumentNullException(nameof(passwordHasherPort));
            _imageStorage = imageStorage ?? throw new ArgumentNullException(nameof(imageStorage));
        }
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userRepositoryPort.ReadAsync();
        }
        public async Task<User?> GetUserByIdAsync(long id)
        {
            var user = await _userRepositoryPort.GetByIdAsync(id);
            if (user?.ProfileImageUrl is { Length: > 0 } key)
                user.ProfileImageUrl = _imageStorage.GetImageUrl(key);
            return user;
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

        public async Task<User?> UpdateProfileImageAsync(long userId, Stream content, string contentType)
        {
            if (!AllowedImageTypes.TryGetValue(contentType, out var ext))
                throw new ProfileImageStorageException("Only JPEG, PNG or WebP images are allowed.");

            var current = await _userRepositoryPort.GetByIdAsync(userId);
            if (current is null) return null;

            var newKey = $"profile-images/{userId}/{Guid.NewGuid():N}{ext}";
            await _imageStorage.SaveAsync(newKey, content, contentType);

            try
            {
                await _userRepositoryPort.UpdateProfileImageAsync(userId, newKey);
            }
            catch
            {
                await _imageStorage.DeleteAsync(newKey);   // niente file orfani
                throw;
            }

            if (!string.IsNullOrEmpty(current.ProfileImageUrl))
                await _imageStorage.DeleteAsync(current.ProfileImageUrl);   // qui è ancora la chiave grezza

            return await GetUserByIdAsync(userId);
        }
    }
}
