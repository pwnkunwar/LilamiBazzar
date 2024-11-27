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
                /*TempData["ErrorMessage"] = "User already exists!";
                return RedirectToAction("Index", "Home", new { area = "Users" })*/;

                ModelState.Remove("UserRoles");
                if (ModelState.IsValid)
                {
                    if (_context.Users.Any(u => u.Email == user.Email))
                    {
                        TempData["ErrorMessage"] = "Email already in use!.";
                        return RedirectToAction("Index","Home",new {area="Users"});
                    }

                    var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == StaticUserRoles.USER);
                    if(role is null)
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
                    _context.Users.Add(create);
                    await _context.SaveChangesAsync();

                    var userRole = new UserRole
                    {
                        UserId = create.UserId,
                        RoleId = role.RoleId
                        
                    };
                   await _context.UserRoles.AddAsync(userRole);
                   await _context.SaveChangesAsync();

                    var email = new Email
                    {
                        To = user.Email,
                        Subject = "Please Verify Your Account",
                        Body = $"Please click on this link to verify your account: https://localhost:7136/Accounts/Dashboard/AccountVerification?token={create.VerificationToken}"
                    };
                    _emailService.SendEmail(email);

                    return Ok("User created successfully");
                }
                else
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors);
                    foreach (var error in errors)
                    {
                        Console.WriteLine(error.ErrorMessage); // Or use a logger
                    }
                    return View();

                }


            }
            catch (Exception ex)
            {
                return View();

            }

        }


        private string GenerateRandomToken()
        {
            return Guid.NewGuid().ToString("D");
        }

    }
}
