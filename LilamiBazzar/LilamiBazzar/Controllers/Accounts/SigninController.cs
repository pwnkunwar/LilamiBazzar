﻿using Azure.Core;
using LilamiBazzar.DataAccess.Database;
using LilamiBazzar.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace LilamiBazzar.Controllers.Accounts
{
    public class SigninController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        public SigninController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        /*public IActionResult Index()
        {
            return View();
        }*/
        [HttpPost]
        public async Task<IActionResult> LoginAsync(UserLogin userLogin)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userLogin.Email);
                    if(user is null)
                    {
                        return BadRequest("User or Password Incorrect");
                    }
                    if(!VerifyPasswordHash(userLogin.Password, user.PasswordHash, user.PasswordSalt))
                    {
                        return BadRequest("User or Password Incorrect");
                    }
                    /*if(user.VerifiedAt == null)
                    {
                        return BadRequest("Please verified your email address");
                    }*/
                    return Ok($"Welcome Back {userLogin.Email}");
                    var authClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                        new Claim(ClaimTypes.GivenName, user.FirstName),
                        new Claim(ClaimTypes.Surname, user.LastName),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, user.Role)
                    };
                    var token = GenerateNewJsonWebToken(authClaims);
                    return Ok(token);

                }
                return BadRequest("Please use correct entries");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(ex.Message);
            }
        }
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA256(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        private string GenerateNewJsonWebToken(List<Claim> claims)
        {
            var authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var tokenObject = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(1),
                claims: claims,
                signingCredentials: new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256)
            );
            string token = new JwtSecurityTokenHandler().WriteToken(tokenObject);
            return token;
        }
    }
}
