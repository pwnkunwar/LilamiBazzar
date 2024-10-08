using System.Security.Claims;

namespace LilamiBazzar.Services.JWTService
{
    public interface IJwtService
    {
        public string GenerateNewJsonWebToken(List<Claim> claims);
    }
}
