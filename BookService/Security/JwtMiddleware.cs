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

        public JwtMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _secretKey = configuration["Jwt:Secret"]; // Leggiamo la chiave dal file di configurazione (appsettings.json)
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
                AttachUserToContext(context, token);

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
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized: Invalid token.");
                return;
            }
        }
    }
}
