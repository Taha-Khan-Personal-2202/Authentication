using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Authentication.BackendHost.CustomServices
{
    public class JwtService
    {
        public readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(string userId,string email, string userName, string role, List<string>? permissions)
        {
            var jwtSettings = _configuration.GetSection("JwtSetting");
            var secretKey = Encoding.UTF8.GetBytes(jwtSettings["Secret"]);


            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Role, role)
            };


            // Add permissions as claims
            foreach (var permission in permissions)
            {
                claims.Add(new Claim("Permission", permission));
            }

            //  new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) Unique token ID
            var key = new SymmetricSecurityKey(secretKey);
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    
            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"], // Now properly set
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpirationMinutes"])),
                signingCredentials: credentials
            );


            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}
