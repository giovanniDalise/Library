using Library.Logging.Abstractions;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Library.BookService.Security
{

    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _secretKey;
        private readonly ILoggerPort _logger;

        public JwtMiddleware(RequestDelegate next, IConfiguration configuration, ILoggerPort logger)
        {
            _next = next;
            _secretKey = configuration["Jwt:Secret"]; // Leggiamo la chiave dal file di configurazione (appsettings.json)
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                _logger.Debug($"JWT token found for request {context.Request.Method} {context.Request.Path}");
                await AttachUserToContext(context, token);
            }
            else
            {
                _logger.Debug($"JWT token NOT present for request {context.Request.Method} {context.Request.Path}");
            }
            await _next(context);
        }

        private async Task AttachUserToContext(HttpContext context, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_secretKey);

                var parameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, parameters, out SecurityToken validatedToken);
                var identity = principal.Identity as ClaimsIdentity;

                if (identity != null)
                {
                    var username = identity.FindFirst(ClaimTypes.Name)?.Value;
                    var role = identity.FindFirst(ClaimTypes.Role)?.Value ?? "USER"; // Default USER

                    _logger.Info($"JWT validated | User={username} | Role={role}");

                    if (!string.IsNullOrEmpty(username))
                    {
                        var claims = new[]
                        {
                            new Claim(ClaimTypes.Name, username),
                            new Claim(ClaimTypes.Role, role)
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, "jwt");
                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                        context.User = claimsPrincipal;
                    }
                }
            }
            catch (SecurityTokenException ex)
            {
                _logger.Warn($"Invalid JWT token | Path={context.Request.Path}");

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized: Invalid token.");
                return;
            }
        }
    }
}
