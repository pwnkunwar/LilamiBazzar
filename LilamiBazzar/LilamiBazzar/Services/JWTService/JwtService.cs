using LilamiBazzar.DataAccess.Database;
using LilamiBazzar.Models.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LilamiBazzar.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _dbcontext;
        public JwtService(IConfiguration configuration, ApplicationDbContext dbContext)
        {
            _configuration = configuration;
            _dbcontext = dbContext;
        }

        public string AuthClaim(User user)
        {

            Role roleName = new Role();
            var roleId = _dbcontext.UserRoles.FirstOrDefault(r => r.UserId == user.UserId);
            if (roleId == null)
            {
                return "Error in roleId";
            }
            roleName = _dbcontext.Roles.FirstOrDefault(r => r.RoleId == roleId.RoleId);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.GivenName, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, roleName.Name),
                new Claim(ClaimTypes.StreetAddress, user.Address)
            };

            // Generate and return the token
            return GenerateNewJsonWebToken(authClaims);
        }

        private string GenerateNewJsonWebToken(List<Claim> claims)
        {
            var authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var tokenObject = new JwtSecurityToken(
                expires: DateTime.Now.AddHours(1),
                claims: claims,
                signingCredentials: new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenObject);
        }
    }
}
