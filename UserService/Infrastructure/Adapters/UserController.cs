using Library.UserService.Core.Domain.Models;
using Library.UserService.Core.Ports;
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
        public async Task<ActionResult<List<User>>> GetUsers()
        {
            return Ok(await _userService.GetAllUsersAsync());
        }

        [HttpPost]
        public async Task<ActionResult<long>> AddUser([FromBody] User user)
        {
            return Ok(await _userService.CreateUserAsync(user));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(long id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            return user == null ? NotFound() : Ok(user);
        }

        [HttpGet("findByString")]
        public async Task<ActionResult<List<User>>> FindUsersByString([FromQuery] string param)
        {
            return Ok(await _userService.GetUsersByTextAsync(param));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<long>> DeleteUser(long id)
        {
            return Ok(await _userService.DeleteUserAsync(id));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<long>> UpdateUser(long id,[FromBody] User user)
        {
            return Ok(await _userService.UpdateUserAsync(id, user));
        }
    }
}
