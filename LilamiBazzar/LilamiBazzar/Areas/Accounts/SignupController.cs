using Microsoft.AspNetCore.Mvc;
using LilamiBazzar.DataAccess.Database;
using System.Security.Cryptography;
using LilamiBazzar.Models.Models;

namespace LilamiBazzar.Areas.Accounts
{
    [Area("Admin")]
    public class SignupController : Controller
    {
        private readonly ApplicationDbContext _context;
        public SignupController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<IActionResult> CreateAsync(LilamiBazzar.Models.Models.User user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (_context.Users.Any(u => u.Email == user.Email))
                    {
                        return BadRequest("User already exists!.");
                    }
                    GeneratePasswordHash(user.Password, out byte[] passwordHash, out byte[] passwordSalt);
                    var create = new LilamiBazzar.Models.Models.User
                    {
                        UserId = Guid.NewGuid(),
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        PasswordHash = passwordHash,
                        PasswordSalt = passwordSalt,
                        VerificationToken = GenerateRandomToken(),
                        Role = StaticUserRoles.USER
                    };
                    _context.Users.Add(create);
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
