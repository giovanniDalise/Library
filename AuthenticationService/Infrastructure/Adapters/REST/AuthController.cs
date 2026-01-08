using Library.AuthenticationService.Core.Domain.Models;
using Library.AuthenticationService.Core.Domain.Services;
using Library.AuthenticationService.Core.Ports;
using Library.AuthenticationService.Infrastructure.Adapters.REST.DTO;
using Library.BookService.Core.Ports;
using Library.Logging.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Library.AuthenticationService.Infrastructure.Adapters.Controller
{
    [ApiController]
    [Route("auth")]
    [ApiExplorerSettings(IgnoreApi = false)]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly AuthenticationServicePort _authenticationService;
        private readonly JwtPort _jwtPort;
        private readonly ILoggerPort _logger;

        // Inject dependencies via the constructor
        public AuthController(
            AuthenticationServicePort authenticationService,
            JwtPort jwtPort,
            ILoggerPort logger)
        {
            _authenticationService = authenticationService;
            _jwtPort = jwtPort;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            // Verifica se le credenziali sono vuote
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                _logger.Warn($"Invalid login attempt: missing credentials (email={request.Email})");
                return Unauthorized(new { message = "Email o password non validi" });
            }

            _logger.Debug($"Attempting authentication for email: {request.Email}");

            try
            {
                var credentials = AuthDTOMapper.ToDomain(request);

                var authResponse = await _authenticationService.Authenticate(credentials);

                if (authResponse != null)
                {
                    _logger.Info($"Authentication successful for email: {request.Email}");
                    return Ok(authResponse);
                }

                _logger.Warn($"Authentication failed for email: {request.Email}");
                return Unauthorized(new { message = "Email o password non validi" });
            }
            catch (Exception ex)
            {
                _logger.Error(
                    $"Unexpected error during authentication for email: {request.Email}",
                    ex
                );

                return StatusCode(500, new { message = "Errore interno del server" });
            }
        }
    }
}
