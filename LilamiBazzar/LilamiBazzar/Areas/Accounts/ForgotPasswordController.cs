using LilamiBazzar.DataAccess.Database;
using LilamiBazzar.Models.Models;
using LilamiBazzar.Services.PasswordHashingService;
using LilamiBazzar.Utility.Services.EmailService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LilamiBazzar.Areas.Accounts
{
    [Area("Accounts")]

    public class ForgotPasswordController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IEmailService _emailService;
        private readonly IPasswordHashingService _passwordHashingService;
        public ForgotPasswordController(ApplicationDbContext dbContext,
            IEmailService emailService,
            IPasswordHashingService passwordHashingService)
        {
            _dbContext = dbContext;
            _emailService = emailService;
            _passwordHashingService = passwordHashingService;   
        }
        [HttpPost]
        public IActionResult Index(string emailAddress)
        {
            if(emailAddress == null)
            {
                TempData["error"] = "Please enter your email address";
                return RedirectToAction("Index","Home", new {area = "Users"});
            }
            var isEmailExists = _dbContext.Users.FirstOrDefault(e => e.Email == emailAddress);
            if (isEmailExists == null)
            {
                TempData["success"] = "We will send Password link to your email if exists!!";
                return RedirectToAction("Index","Home", new {area = "Users"});
            }
            Guid token = Guid.NewGuid();
            isEmailExists.PasswordResetToken = token.ToString();
            isEmailExists.PasswordResetTokenExpires = DateTime.UtcNow.AddDays(1);
            _dbContext.SaveChanges();

            var email= new Email
            {
                To = emailAddress,
                Subject = "Password Reset",
                Body = $"Please click on this link to reset your account password: https://lilamibazzar.runasp.net/Accounts/ForgotPassword/Reset?token={token}"
            };
            _emailService.SendEmail(email);

            TempData["success"] = "We will send Password link to your email if exists!!";

            return RedirectToAction("Index","Home", new {area = "Users"});

        }
        [HttpGet]
        public IActionResult Reset(string token)
        {
            return View((object)token);
        }

        [HttpPost]
        public IActionResult Reset(string token, string newPassword, string confirmPassword)
        {
            if (token == null || newPassword == null || confirmPassword == null)
            {
                TempData["error"] = "Please enter value for all required fields";
                return RedirectToAction("Index");
            }
            if(newPassword != confirmPassword)
            {
                TempData["error"] = "NewPassword and ConfirmPassword must match";
                return RedirectToAction("Index");

            }

            var isTokenExists = _dbContext.Users.FirstOrDefault(t => t.PasswordResetToken == token);
            if (isTokenExists == null)
            {
                TempData["error"] = "Incorrect Password Token";
                return RedirectToAction("Reset");
            }
            if (_passwordHashingService.VerifyPasswordHash(newPassword, isTokenExists.PasswordHash, isTokenExists.PasswordSalt))
            {

                TempData["error"] = "New Password cannot be same as Old Password.";
                return RedirectToAction("Reset");

            }
            if (isTokenExists.PasswordResetTokenExpires < DateTime.UtcNow)
            {
                TempData["error"] = "Passowrd Reset Token Expired, Please request new one";
                return RedirectToAction("Reset");
            }
            _passwordHashingService.GeneratePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);
            isTokenExists.PasswordHash = passwordHash;
            isTokenExists.PasswordSalt = passwordSalt;
            isTokenExists.PasswordResetToken = Guid.NewGuid().ToString();
            _dbContext.SaveChanges();

            TempData["success"] = "Password reset successfully";
            return RedirectToAction("Index", "Home", new { area = "Users" });

        }
    }
}
