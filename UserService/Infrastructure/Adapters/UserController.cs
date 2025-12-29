using Library.UserService.Core.Domain.Models;
using Library.UserService.Core.Ports;
using Library.UserService.Infrastructure.DTO.REST;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.UserService.Infrastructure.Adapters
{
    [Route("users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserServicePort _userService;

        public UserController(UserServicePort userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<UserResponse>>> GetUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(UserDTOMapper.ToResponseList(users));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponse>> GetUserById(long id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            return Ok(UserDTOMapper.ToResponse(user));
        }

        [HttpPost]
        public async Task<ActionResult<long>> AddUser([FromBody] UserRequest request)
        {
            var user = UserDTOMapper.ToDomain(request);
            return Ok(await _userService.CreateUserAsync(user));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<long>> UpdateUser(long id, [FromBody] UserRequest request)
        {
            var user = UserDTOMapper.ToDomain(request);
            return Ok(await _userService.UpdateUserAsync(id, user));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<long>> DeleteUser(long id)
        {
            return Ok(await _userService.DeleteUserAsync(id));
        }

        [HttpGet("findByString")]
        public async Task<ActionResult<List<UserResponse>>> FindUsersByString([FromQuery] string param)
        {
            var users = await _userService.GetUsersByTextAsync(param);
            return Ok(UserDTOMapper.ToResponseList(users));
        }
    }
}
