using Microsoft.AspNetCore.Mvc;
using LilamiBazzar.DataAccess.Database;
using System.Security.Cryptography;
using LilamiBazzar.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace LilamiBazzar.Areas.Accounts
{
    [Area("Accounts")]
    public class SignupController : Controller
    {
        private readonly ApplicationDbContext _context;
        public SignupController(ApplicationDbContext context)
        {
            _context = context;
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
                        return BadRequest("User already exists!.");
                    }

                    var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == StaticUserRoles.USER);
                    if(role is null)
                    {
                        return BadRequest();
                    }
                    GeneratePasswordHash(user.Password, out byte[] passwordHash, out byte[] passwordSalt);
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
        private void GeneratePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA256())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }

        }

        private string GenerateRandomToken()
        {
            return Guid.NewGuid().ToString("D");
        }

    }
}
