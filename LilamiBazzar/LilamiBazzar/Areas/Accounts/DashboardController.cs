using LilamiBazzar.DataAccess.Database;
using LilamiBazzar.Models.Models;
using LilamiBazzar.Services.JWTService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Security.Claims;

namespace LilamiBazzar.Areas.Accounts
{
    [Area("Accounts")]

    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _dbcontext;
        private readonly IJwtService _jwtService;
        public DashboardController(ApplicationDbContext context, IJwtService jwtService)
        {
            _dbcontext = context;
            _jwtService = jwtService;
        }
        [Authorize]
        public IActionResult Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var fullName = User.FindFirst(ClaimTypes.GivenName)?.Value;
            var address = User.FindFirst(ClaimTypes.StreetAddress)?.Value;

            var profileData = new
            {
                UserId = userId,
                FullName = fullName,
                Address = address,
            };
            return View(profileData);
        }
        [HttpPost]
        public IActionResult ProfileUpdate(Guid Id, string fullName, string Address)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(userId is null)
            {
                return Unauthorized();
            }
            else if(userId != Id.ToString())
            {
                return Unauthorized();
            }

            var user = _dbcontext.Users.Find(Id);
            user.FullName = fullName;
            user.Address = Address;
            _dbcontext.SaveChanges();




            Role roleName = new Role();
            var roleId = _dbcontext.UserRoles.FirstOrDefault(r => r.UserId == user.UserId);
            roleName = _dbcontext.Roles.FirstOrDefault(r => r.RoleId == roleId.RoleId);


            var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                        new Claim(ClaimTypes.GivenName, user.FullName),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, roleName.Name),
                        new Claim(ClaimTypes.StreetAddress, user.Address)
            };
            var token = _jwtService.GenerateNewJsonWebToken(authClaims);


            Response.Cookies.Append("Authorization", token, new CookieOptions
            {
                /*HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.Now.AddDays(1)*/
            });

            return RedirectToAction("Index");

        }
        public IActionResult Email()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var emailObj = new
            {
                UserId = userId,
                Email = email
            };
            return View(emailObj);
        }

        public IActionResult UpdateEmail(Guid Id, string newEmail)
        {
            var oldEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return Unauthorized();
            }
            else if (userId != Id.ToString())
            {
                return Unauthorized();
            }
            else if(newEmail == null)
            {
                return BadRequest();
            }
            else if(oldEmail == newEmail)
            {
                return BadRequest();
            }
            var user = _dbcontext.Users.Find(Id);
            bool emailExists = _dbcontext.Users.Any(u => u.Email == newEmail);
            if(emailExists)
            {
                return BadRequest();
            }

            //here send email verify token to the user email client and later save email
            user.Email = newEmail;
            _dbcontext.SaveChanges();

            return RedirectToAction("Index");




        }
        public IActionResult ChangePassword()
        {
            return View();
        }
        public IActionResult TwoFactorAuthentication()
        {
            return View();
        }
        public IActionResult PersonalData()
        {
            return View();
        }
    }
}
