using Library.IdentityService.Core.Domain.Models;

namespace Library.IdentityService.Infrastructure.DTO.REST.Auth
{
    public static class AuthDTOMapper
    {
        public static Credentials ToDomain(LoginRequest dto)
            => new Credentials
            {
                Email = dto.Email,
                Password = dto.Password
            };
    }

}
