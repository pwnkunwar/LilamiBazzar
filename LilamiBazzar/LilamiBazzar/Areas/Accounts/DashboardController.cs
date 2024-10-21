using LilamiBazzar.DataAccess.Database;
using LilamiBazzar.Models.Models;
using LilamiBazzar.Services.PasswordHashingService;
using LilamiBazzar.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using System.Security.Cryptography;

namespace LilamiBazzar.Areas.Accounts
{
    [Area("Accounts")]

    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _dbcontext;
        private readonly IJwtService _jwtService;
        private readonly IPasswordHashingService _passwordHashingService;
        public DashboardController(ApplicationDbContext context, IJwtService jwtService, IPasswordHashingService passwordHashingService)
        {
            _dbcontext = context;
            _jwtService = jwtService;
            _passwordHashingService = passwordHashingService;
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




            var token = _jwtService.AuthClaim(user);


            
            


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
        [HttpPost]
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

            var token = _jwtService.AuthClaim(user);

            Response.Cookies.Append("Authorization", token, new CookieOptions
            {
                /*HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.Now.AddDays(1)*/
            });

            return RedirectToAction("Index");




        }
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            if(newPassword != confirmPassword)
            {
                return BadRequest();
            }
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var guUserId = Guid.Parse(userId);
            var user = _dbcontext.Users.Find(guUserId);
            if(!_passwordHashingService.VerifyPasswordHash(oldPassword, user.PasswordHash, user.PasswordSalt))
            { 
                return BadRequest("Password not Matched");
            }
            _passwordHashingService.GeneratePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            _dbcontext.SaveChanges();

            var token = _jwtService.AuthClaim(user);

            Response.Cookies.Append("Authorization", token, new CookieOptions
            {
                /*HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.Now.AddDays(1)*/
            });

            return RedirectToAction("Index");
        }

        public IActionResult AccountVerification(string token)
        {
            if(token == null)
            {
                return BadRequest();
            }
            var isTokenValid = _dbcontext.Users.FirstOrDefault(t => t.VerificationToken == token);
            if(isTokenValid == null)
            {
                return BadRequest();
            }
            else
            {
                isTokenValid.VerifiedAt = DateTime.UtcNow;
                _dbcontext.SaveChanges();
                return Ok("Email Address Verified");
            }
        }
        public IActionResult UnLocked(string token)
        {
           if(token == null)
           {
                return BadRequest();
           }
           var user =  _dbcontext.Users.FirstOrDefault(t => t.LockoutId == Guid.Parse(token));
            if(user == null)
            {
                return BadRequest();
            }
            user.FailedLoginAttempts = 0;
            _dbcontext.SaveChanges();

            return RedirectToAction("Index","Home");
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
