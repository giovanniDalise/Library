using Library.IdentityService.Core.Domain.Models;
using System.Net;

namespace Library.IdentityService.Infrastructure.DTO.REST.Auth
{
    public static class AuthDTOMapper
    {
        public static Credentials ToDomain(LoginRequestDTO dto)
            => new Credentials
            {
                Email = dto.Email,
                Password = dto.Password
            };


        public static AuthResponseDTO ToResponse(AuthResult result)
            => new() { Token = result.Token };
    }

}
