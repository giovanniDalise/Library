using Library.IdentityService.Core.Exceptions;
using Library.IdentityService.Core.Ports;
using Library.IdentityService.Infrastructure.DTO.REST.Users;
using Library.Logging.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Library.IdentityService.Infrastructure.Adapters
{
    [Route("users")]
    [ApiController]
    [Authorize]
    //[Authorize] 
    //Possiamo avere due strategie opposte, entrambe valide:
    //Controller senza authorize : pubblico per default, protetto solo dove serve (inserendo poi nella api[Authorize(Roles = "admin")])
    //[AllowAnonymous] in questo caso non serve e sarebbe ridondante
    //Controller con authorize: protetto per default, aperto solo dove serve.
    //Qui quasi tutto è sensibile(dati di altri utenti, modifica, cancellazione) e qui serve [AllowAnonymous]
    //Il vantaggio di “protetto per default” è che se un domani aggiungi un endpoint e ti dimentichi
    //l’attributo, l’endpoint resta chiuso invece che aperto.
    public class UserController : ControllerBase
    {
        private readonly IUserAppServicePort _userAppService;
        private readonly ILoggerPort _logger;
        private const long MaxPhotoBytes = 2 * 1024 * 1024;


        public UserController(IUserAppServicePort userAppService, ILoggerPort<UserController> logger)
        {
            _userAppService = userAppService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<List<UserResponse>>> GetUsers()
        {
            _logger.Info("GetUsers endpoint called");
            try
            {
                var users = await _userAppService.GetAllUsersAsync();
                _logger.Info($"GetUsers returned {users.Count} users");
                return Ok(UserDTOMapper.ToResponseList(users));
            }
            catch (Exception ex)
            {
                _logger.Error("Unexpected error while retrieving users", ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("me")]
        public async Task<ActionResult<UserResponse>> GetPersonalUser()
        {
            _logger.Info("GetPersonalUser called");
            try
            {
                var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!long.TryParse(idClaim, out var userId))
                    return Unauthorized();

                var user = await _userAppService.GetUserByIdAsync(userId);

                if(user == null)
                    return NotFound();

                return Ok(UserDTOMapper.ToResponse(user));
            }
            catch (Exception ex)
            {
                _logger.Error("Unexpected error while retrieving personal user", ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<UserResponse>> GetUserById(long id)
        {
            _logger.Info($"GetUserById called with id={id}");
            try
            {
                var user = await _userAppService.GetUserByIdAsync(id);
                if (user == null)
                {
                    _logger.Warn($"User not found with id={id}");
                    return NotFound();
                }

                _logger.Info($"User found with id={id}");
                return Ok(UserDTOMapper.ToResponse(user));
            }
            catch (Exception ex)
            {
                _logger.Error($"Unexpected error while retrieving user id={id}", ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("addUser")]
        [AllowAnonymous]
        public async Task<ActionResult<long>> AddUser([FromBody] UserRequest request)
        {
            _logger.Info($"AddUser {request.Name} {request.Surname}");
            try
            {
                var user = UserDTOMapper.ToDomain(request);
                var id = await _userAppService.CreateUserAsync(user);
                _logger.Info($"User created with id={id}");
                return Ok(id);
            }
            catch (Exception ex)
            {
                _logger.Error($"Unexpected error while creating user {request.Email}", ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<long>> UpdateUser(long id, [FromBody] UserRequest request)
        {
            _logger.Info($"UpdateUser called for id={id}");
            try
            {
                var user = UserDTOMapper.ToDomain(request);
                var updatedId = await _userAppService.UpdateUserAsync(id, user);
                _logger.Info($"User updated with id={updatedId}");
                return Ok(updatedId);
            }
            catch (Exception ex)
            {
                _logger.Error($"Unexpected error while updating user id={id}", ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<long>> DeleteUser(long id)
        {
            _logger.Info($"DeleteUser called for id={id}");
            try
            {
                var deletedId = await _userAppService.DeleteUserAsync(id);
                _logger.Info($"User deleted with id={deletedId}");
                return Ok(deletedId);
            }
            catch (Exception ex)
            {
                _logger.Error($"Unexpected error while deleting user id={id}", ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("me/photo")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<UserResponse>> UpdateMyPhoto(IFormFile photo)
        {
            _logger.Info("UpdateMyPhoto called");
            try
            {
                if (!long.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
                    return Unauthorized();
                if (photo is null || photo.Length == 0)
                    return BadRequest("Photo is required.");
                if (photo.Length > MaxPhotoBytes)
                    return BadRequest("Photo must be 2 MB or smaller.");

                await using var stream = photo.OpenReadStream();
                var user = await _userAppService.UpdateProfileImageAsync(userId, stream, photo.ContentType);
                if (user == null) return NotFound();

                return Ok(UserDTOMapper.ToResponse(user));
            }
            catch (ProfileImageStorageException ex)
            {
                _logger.Warn($"Invalid profile image: {ex.Message}");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error("Unexpected error while updating profile photo", ex);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
