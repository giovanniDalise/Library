using Library.UserService.Core.Domain.Models;
using Library.UserService.Core.Ports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.UserService.Infrastructure.Adapters
{
    [Route("users")]
    [ApiController]
    public class UserController:ControllerBase
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

    }
}
