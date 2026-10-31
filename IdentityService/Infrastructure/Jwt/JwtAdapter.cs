using Library.IdentityService.Core.Domain.Models;
using Library.IdentityService.Core.Ports;
using Library.Logging.Abstractions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.ConstrainedExecution;
using System.Security.Claims;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace Library.IdentityService.Infrastructure.Adapters.Jwt
{
    public class JwtAdapter : IJwtPort
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

        public async Task<string> GenerateJwtToken(JwtUserClaims user)
        {
            _logger.Debug($"Generating JWT token for user id: {user.UserId}, role: {user.Role}");

            var securityKey = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(_jwtSecret));

            var credentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha512Signature);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString(CultureInfo.InvariantCulture)),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };
            //Sono due modi di scrivere un’etichetta sul badge.Ogni informazione nel token ha 
            //un’etichetta(si chiama “claim”) per esempio “questo è il ruolo”, “questo è il nome”.
            //ClaimTypes.Role, ClaimTypes.Name: etichette definite da Microsoft. 
            //Nel token vengono scritte come indirizzi lunghi(tipo http://schemas.microsoft.com/ws/2008/06/identity/claims/role).
            //JwtRegisteredClaimNames.Sub: etichetta definita dallo standard internazionale dei JWT.sub sta per “subject”, cioè
            //“il soggetto del token”: chi è la persona.È un’etichetta corta, uguale in tutti i linguaggi e sistemi.
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

            _logger.Info($"JWT token successfully generated for user id: {user.UserId}");

            return await Task.FromResult(token);
        }
    }
}
