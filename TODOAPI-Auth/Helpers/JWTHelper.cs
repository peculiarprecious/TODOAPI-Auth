using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TODOAPI_Auth.Models;
namespace TODOAPI_Auth.Helpers
{
    public class JWTHelper : IJWTHelper
    {
        private readonly IConfiguration _config;

        public JWTHelper(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(User user)
        {
            var jwtSettings = _config.GetSection("jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);
            // 1. Create claims — info stored inside the token
            var claims = new List<Claim>
            {
                
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                 new Claim(ClaimTypes.Name, $"{user.firstName} {user.lastName}".Trim()),
                new Claim(ClaimTypes.Email, user.email)
            
            };
            // 2. Get secret key from appsettings.json
            var signingKey = new SymmetricSecurityKey(key);
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            // 3. Build the token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["DurationInMinutes"] ?? "60")),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = creds
            };

            // 4. Convert token to string
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
