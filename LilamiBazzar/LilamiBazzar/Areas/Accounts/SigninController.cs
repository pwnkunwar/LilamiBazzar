using Azure.Core;
using LilamiBazzar.DataAccess.Database;
using LilamiBazzar.Models.Models;
using LilamiBazzar.Services;
using LilamiBazzar.Services.PasswordHashingService;
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
        public SigninController(
            ApplicationDbContext context, 
            IConfiguration configuration, 
            IJwtService jwtService,
            IPasswordHashingService passwordHashingService)
        {
            _context = context;
            _configuration = configuration;
            _jwtService = jwtService;
            _passwordHashingService = passwordHashingService;
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
                    if (!_passwordHashingService.VerifyPasswordHash(userLogin.Password, user.PasswordHash, user.PasswordSalt))
                    {
                        return BadRequest("User or Password Incorrect");
                    }
                    /*if(user.VerifiedAt == null)
                    {
                        return BadRequest("Please verified your email address");
                    }*/
                    // return Ok($"Welcome Back {userLogin.Email}");


                    var token = _jwtService.AuthClaim(user);

                    // localStorage.setItem('Authorization', token);


                    Response.Cookies.Append("Authorization", token, new CookieOptions
                    {
                        /*HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.Now.AddDays(1)*/
                    });
                    return RedirectToAction("Index", "Home");

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
