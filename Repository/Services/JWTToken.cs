using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Repository.Services
{
    public class JWTToken
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expiryMinutes;

        public JWTToken(IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            _secretKey = jwtSettings["SecretKey"] ?? throw new ArgumentNullException(nameof(_secretKey), "JWT SecretKey is missing.");
            _issuer = jwtSettings["Issuer"] ?? string.Empty;
            _audience = jwtSettings["Audience"] ?? string.Empty;
            _expiryMinutes = int.TryParse(jwtSettings["ExpiryMinutes"], out int expiry) ? expiry : 60; // Default to 60 minutes if not specified
        }

        public string GenerateToken(string username, string role = "Admin") // Default role to Admin
        {
            var key = Encoding.UTF8.GetBytes(_secretKey);
            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_expiryMinutes),
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
