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
            _jwtExpirationMs = jwtExpirationMs;
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

            var tokenDescriptor = new JwtSecurityToken(
                claims: claims, // Claims
                expires: DateTime.Now.AddMilliseconds(_jwtExpirationMs), // Set the expiration
                signingCredentials: credentials); // Use the signing credentials

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.WriteToken(tokenDescriptor); // Generate the JWT token

            return await Task.FromResult(token); // Return the token asynchronously
        }
    }
}
