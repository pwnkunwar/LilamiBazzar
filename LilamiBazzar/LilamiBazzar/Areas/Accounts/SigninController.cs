using Azure.Core;
using LilamiBazzar.DataAccess.Database;
using LilamiBazzar.Models.Models;
using LilamiBazzar.Services;
using LilamiBazzar.Services.PasswordHashingService;
using LilamiBazzar.Utility.Services.EmailService;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace LilamiBazzar.Areas.Accounts
{
    [Area("Accounts")]

    public class SigninController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IJwtService _jwtService;
        private readonly IPasswordHashingService _passwordHashingService;
        private readonly IEmailService _emailService;
        public SigninController(
            ApplicationDbContext context, 
            IConfiguration configuration, 
            IJwtService jwtService,
            IPasswordHashingService passwordHashingService,
            IEmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _jwtService = jwtService;
            _passwordHashingService = passwordHashingService;
            _emailService = emailService;
        }
        /*public IActionResult Index()
        {
            return View();
        }*/
        [HttpPost]
        public async Task<IActionResult> LoginAsync([FromBody] UserLogin userLogin)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userLogin.Email);
                    if (user is null)
                    {
                        return BadRequest("User or Password Incorrect");
                    }
                    if(user.FailedLoginAttempts > 6)
                    {
                        var isLockedTokenAlreadyAvailable = _context.Users.Any(u => u.UserId == user.UserId && u.LockoutId == null);
                        if(isLockedTokenAlreadyAvailable)
                        {
                            Guid unLocked = Guid.NewGuid();
                            user.LockoutId = unLocked;
                            _context.SaveChanges();
                            var email = new Email
                            {
                                To = userLogin.Email,
                                Subject = "Account UnLocked",
                                Body = $"Please click on this link to verify your account: https://localhost:7136/Accounts/Dashboard/UnLocked?token={unLocked}"
                            };
                            _emailService.SendEmail(email);
                        }
                         

                        return BadRequest("You have enter wrong credentials numerous times! We have send a Account UnLocked Code in the email");
                      

                    }
                    if (!_passwordHashingService.VerifyPasswordHash(userLogin.Password, user.PasswordHash, user.PasswordSalt))
                    {
                        user.FailedLoginAttempts += 1;
                        _context.SaveChanges();
                        TempData["error"] = "User or Password Incorrect";
                        return BadRequest("User or Password Incorrect");
                    }
                    /*if(user.VerifiedAt == null)
                    {
                        return BadRequest("Please verified your email address");
                    }*/
                    // return Ok($"Welcome Back {userLogin.Email}");

                    var token = _jwtService.AuthClaim(user);

                    // localStorage.setItem('Authorization', token);
                    /*return token*/

                    Response.Cookies.Append("Authorization", token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.Now.AddDays(1)
                    });
                    return Redirect("/");


                }
                return BadRequest("Please use correct entries");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(ex.Message);
            }
        }
       

       
    }
}
