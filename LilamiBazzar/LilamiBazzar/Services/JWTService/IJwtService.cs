using LilamiBazzar.Models.Models;
using System.Security.Claims;

namespace LilamiBazzar.Services
{
    public interface IJwtService
    {
        public string AuthClaim(User user);
    }
}
