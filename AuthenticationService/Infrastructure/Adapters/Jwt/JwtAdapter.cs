using Library.AuthenticationService.Core.Ports;
using Library.Logging.Abstractions;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Library.AuthenticationService.Infrastructure.Adapters.Jwt
{
    public class JwtAdapter : JwtPort
    {
        private readonly string _jwtSecret;
        private readonly double _jwtExpirationMs;
        private readonly ILoggerPort _logger;

        public JwtAdapter(
            IConfiguration configuration,
            double jwtExpirationMs,
            ILoggerPort logger)
        {
            _jwtSecret = configuration["Jwt:Secret"]
                ?? throw new ArgumentNullException("Jwt Secret not configured");

            _jwtExpirationMs = double.Parse(configuration["Jwt:ExpirationMs"]);
            _logger = logger;
        }

        public async Task<string> GenerateJwtToken(string email, string role)
        {
            _logger.Debug($"Generating JWT token for email: {email}, role: {role}");

            var securityKey = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(_jwtSecret));

            var credentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha512Signature);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, email),
                new Claim(ClaimTypes.Role, role)
            };

            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");
            var expirationTime = TimeZoneInfo.ConvertTime(
                DateTime.UtcNow.AddMilliseconds(_jwtExpirationMs),
                timeZoneInfo);

            _logger.Info($"JWT token expiration time set to: {expirationTime}");

            var tokenDescriptor = new JwtSecurityToken(
                claims: claims,
                expires: expirationTime,
                signingCredentials: credentials);

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.WriteToken(tokenDescriptor);

            _logger.Info($"JWT token successfully generated for email: {email}");

            return await Task.FromResult(token);
        }
    }
}
