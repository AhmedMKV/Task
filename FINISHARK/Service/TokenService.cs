using FINISHARK.Interfaces;
using FINISHARK.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FINISHARK.Service
{
    public class TokenService : ITokenService
    {

        private readonly IConfiguration _configuration;
        private readonly SymmetricSecurityKey _key;
        private readonly UserManager<AppUser> _userManager;

        public TokenService(IConfiguration configuration, UserManager<AppUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SigningKey"]));
            
        }
        public string CreateToken(AppUser user)
        {
            var Claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.GivenName, user.UserName)
            };

            // Add role claims to the token
            var roles = _userManager.GetRolesAsync(user).GetAwaiter().GetResult();
            foreach (var role in roles)
            {
                Claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var credentials = new SigningCredentials(_key , SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(Claims),
                Expires = DateTime.Now.AddDays(7),
                
                SigningCredentials = credentials,
                Issuer = _configuration["JWT:Issuer"],
                Audience = _configuration["JWT:Audience"]
            };

            var TokenHandler = new JwtSecurityTokenHandler();

            var Token = TokenHandler.CreateToken(tokenDescriptor);
            return TokenHandler.WriteToken(Token);
        }
    }
}
