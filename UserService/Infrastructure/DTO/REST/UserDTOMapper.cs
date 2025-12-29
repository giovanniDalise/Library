using Library.UserService.Core.Domain.Models;

namespace Library.UserService.Infrastructure.DTO.REST
{
    public static class UserDTOMapper
    {
        public static User ToDomain(UserRequest dto)
            => new User
            {
                Name = dto.Name,
                Surname = dto.Surname,
                Email = dto.Email,
                Password = dto.Password,
                Role = dto.Role
            };

        public static UserResponse ToResponse(User user)
            => new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                Role = user.Role
            };

        public static List<UserResponse> ToResponseList(IEnumerable<User> users)
            => users.Select(ToResponse).ToList();
    }
}
