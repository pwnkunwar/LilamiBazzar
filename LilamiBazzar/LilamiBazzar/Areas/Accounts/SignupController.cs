using Microsoft.AspNetCore.Mvc;
using LilamiBazzar.DataAccess.Database;
using System.Security.Cryptography;
using LilamiBazzar.Models.Models;
using Microsoft.EntityFrameworkCore;
using LilamiBazzar.Services.PasswordHashingService;
using LilamiBazzar.Utility;
using LilamiBazzar.Utility.Services.EmailService;

namespace LilamiBazzar.Areas.Accounts
{
    [Area("Accounts")]
    public class SignupController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHashingService _passwordHashingService;
        private readonly IEmailService _emailService;
        public SignupController(ApplicationDbContext context, IPasswordHashingService passwordHashingService, IEmailService emailService)
        {
            _context = context;
            _passwordHashingService = passwordHashingService;
            _emailService = emailService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] LilamiBazzar.Models.Models.User user)
        {
            try
            {
                ModelState.Remove("UserRoles");
                if (ModelState.IsValid)
                {
                    // Use `FirstOrDefaultAsync` with a combined condition to reduce redundant checks
                    var existingUser = await _context.Users
                        .FirstOrDefaultAsync(u => u.Email == user.Email);

                    if (existingUser != null)
                    {
                        TempData["ErrorMessage"] = "Email already in use!";
                        return RedirectToAction("Index", "Home", new { area = "Users" });
                    }

                    var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == StaticUserRoles.USER);
                    if (role == null)
                    {
                        return BadRequest();
                    }

                    _passwordHashingService.GeneratePasswordHash(user.Password, out byte[] passwordHash, out byte[] passwordSalt);

                    var create = new LilamiBazzar.Models.Models.User
                    {
                        UserId = Guid.NewGuid(),
                        FullName = user.FullName,
                        Address = user.Address,
                        Email = user.Email,
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt,
                        VerificationToken = GenerateRandomToken(),
                    };

                    // Add user and role in a batch
                    await _context.Users.AddAsync(create);
                    await _context.UserRoles.AddAsync(new UserRole
                    {
                        UserId = create.UserId,
                        RoleId = role.RoleId
                    });

                    // Save changes in a single operation
                    await _context.SaveChangesAsync();

                    var email = new Email
                    {
                        To = user.Email,
                        Subject = "Please Verify Your Account",
                        Body = $"Please click on this link to verify your account: https://lilamibazzar.runasp.net/Accounts/Dashboard/AccountVerification?token={create.VerificationToken}"
                    };
                    _emailService.SendEmail(email);

                    TempData["success"] = "Account Created Successfully, Please visit Email Service to verify it!";
                    return Json(new { success = true, message = "Account Created Successfully, Please visit Email Service to verify it!" });
                }
                else
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors);
                    foreach (var error in errors)
                    {
                        Console.WriteLine(error.ErrorMessage);
                    }
                    return View();
                }
            }
            catch (Exception ex)
            {
                // Log exception here for debugging
                Console.WriteLine(ex.Message);
                return View();
            }
        }


        private string GenerateRandomToken()
        {
            return Guid.NewGuid().ToString("D");
        }

    }
}
