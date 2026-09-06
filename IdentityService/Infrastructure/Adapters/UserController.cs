using Library.Logging.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Library.IdentityService.Core.Ports;
using Library.IdentityService.Infrastructure.DTO.REST.Users;

namespace Library.IdentityService.Infrastructure.Adapters
{
    [Route("users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserAppServicePort _userAppService;
        private readonly ILoggerPort _logger;

        public UserController(IUserAppServicePort userAppService, ILoggerPort logger)
        {
            _userAppService = userAppService;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<UserResponse>>> GetUsers()
        {
            _logger.Info("GetUsers endpoint called");
            var users = await _userAppService.GetAllUsersAsync();
            _logger.Info($"GetUsers returned {users.Count} users");
            return Ok(UserDTOMapper.ToResponseList(users));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponse>> GetUserById(long id)
        {
            _logger.Info($"GetUserById called with id={id}");
            var user = await _userAppService.GetUserByIdAsync(id);
            if (user == null)
            {
                _logger.Warn($"User not found with id={id}");
                return NotFound();
            }

            _logger.Info($"User found with id={id}");
            return Ok(UserDTOMapper.ToResponse(user));
        }

        [HttpPost("addUser")]
        [AllowAnonymous]
        public async Task<ActionResult<long>> AddUser([FromBody] UserRequest request)
        {
            _logger.Info($"AddUser {request.Name} {request.Surname}");
            var user = UserDTOMapper.ToDomain(request);
            var id = await _userAppService.CreateUserAsync(user);
            _logger.Info($"User created with id={id}");
            return Ok(id);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<long>> UpdateUser(long id, [FromBody] UserRequest request)
        {
            _logger.Info($"UpdateUser called for id={id}");
            var user = UserDTOMapper.ToDomain(request);
            var updatedId = await _userAppService.UpdateUserAsync(id, user);
            _logger.Info($"User updated with id={updatedId}");
            return Ok(updatedId);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<long>> DeleteUser(long id)
        {
            _logger.Info($"DeleteUser called for id={id}");
            var deletedId = await _userAppService.DeleteUserAsync(id);
            _logger.Info($"User deleted with id={deletedId}");
            return Ok(deletedId);
        }

        [HttpGet("findByString")]
        public async Task<ActionResult<List<UserResponse>>> FindUsersByString([FromQuery] string param)
        {
            _logger.Info($"FindUsersByString called with param='{param}'");
            var users = await _userAppService.GetUsersByTextAsync(param);
            _logger.Info($"FindUsersByString returned {users.Count} users");
            return Ok(UserDTOMapper.ToResponseList(users));
        }
    }
}
