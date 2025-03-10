namespace Library.AuthenticationService.Infrastructure.Adapters.Jwt
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using Library.AuthenticationService.Core.Ports;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.Extensions.Configuration;  // Aggiungi questa direttiva per accedere alla configurazione

    public class JwtAdapter : JwtPort
    {
        private readonly string _jwtSecret;
        private readonly double _jwtExpirationMs;

        // Costruttore che accetta IConfiguration per caricare i valori dalla configurazione
        public JwtAdapter(IConfiguration configuration, double jwtExpirationMs)
        {
            _jwtSecret = configuration["Jwt:Secret"] ?? throw new ArgumentNullException("Jwt Secret not configured");
            _jwtExpirationMs = double.Parse(configuration["Jwt:ExpirationMs"]);
        }

        public async Task<string> GenerateJwtToken(string email, string role)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException(nameof(email), "Email cannot be null or empty");
            }

            if (string.IsNullOrEmpty(role))
            {
                throw new ArgumentNullException(nameof(role), "Role cannot be null or empty");
            }

            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_jwtSecret)); // Key for signing the token
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature); // Set the signing algorithm

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, email), // Subject: email
                new Claim(ClaimTypes.Role, role)   // Add role as a claim
            };

            // Recupera il fuso orario desiderato (ad esempio, per l'Italia, che è UTC+1 o UTC+2 con l'ora legale)
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");
            var expirationTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow.AddMilliseconds(_jwtExpirationMs), timeZoneInfo);


            // Log della data di scadenza
            Console.WriteLine($"Token expiration time (Local Time Zone): {expirationTime}");

            var tokenDescriptor = new JwtSecurityToken(
                claims: claims, // Claims
                expires: expirationTime, // Imposta la scadenza nel fuso orario locale
                signingCredentials: credentials); // Usa le credenziali di firma

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.WriteToken(tokenDescriptor); // Genera il token JWT

            return await Task.FromResult(token); // Restituisci il token in modo asincrono
        }
    }
}
