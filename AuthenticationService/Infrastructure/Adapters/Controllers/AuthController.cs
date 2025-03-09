﻿using Library.AuthenticationService.Core.Domain.Models;
using Library.AuthenticationService.Core.Ports;
using Library.AuthenticationService.Core.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Library.BookService.Core.Ports;

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

        // Inject dependencies via the constructor
        public AuthController(AuthenticationServicePort authenticationService, JwtPort jwtPort)
        {
            _authenticationService = authenticationService;
            _jwtPort = jwtPort;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Verifica se le credenziali sono vuote
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return Unauthorized(new { message = "Email o password non validi" });
            }

            // Usa il metodo authenticate del servizio in modo asincrono
            var authResponse = await _authenticationService.Authenticate(request);

            if (authResponse != null)
            {
                return Ok(authResponse); // Risponde con i dati di autenticazione
            }

            return Unauthorized(new { message = "Email o password non validi" }); // Risponde con errore di autenticazione
        }
    }
}
