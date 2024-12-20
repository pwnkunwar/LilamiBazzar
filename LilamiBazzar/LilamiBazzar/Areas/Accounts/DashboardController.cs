﻿using LilamiBazzar.DataAccess.Database;
using LilamiBazzar.Models.Models;
using LilamiBazzar.Services.PasswordHashingService;
using LilamiBazzar.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using LilamiBazzar.Utility.Services.EmailService;

namespace LilamiBazzar.Areas.Accounts
{
    [Area("Accounts")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _dbcontext;
        private readonly IJwtService _jwtService;
        private readonly IPasswordHashingService _passwordHashingService;
        private readonly IEmailService _emailService;

        public DashboardController(ApplicationDbContext context, IJwtService jwtService, IPasswordHashingService passwordHashingService, IEmailService emailService)
        {
            _dbcontext = context;
            _jwtService = jwtService;
            _passwordHashingService = passwordHashingService;
            _emailService = emailService;
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
            if (userId is null)
            {
                TempData["error"] = "User is not authorized.";
                return Unauthorized();
            }
            else if (userId != Id.ToString())
            {
                TempData["error"] = "Unauthorized access.";
                return Unauthorized();
            }

            var user = _dbcontext.Users.Find(Id);
            user.FullName = fullName;
            user.Address = Address;
            _dbcontext.SaveChanges();

            var token = _jwtService.AuthClaim(user);
            Response.Cookies.Append("Authorization", token, new CookieOptions
            {
                // Configure these as needed
            });

            TempData["success"] = "Profile updated successfully!";
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
                TempData["error"] = "User is not authorized.";
                return Unauthorized();
            }
            else if (userId != Id.ToString())
            {
                TempData["error"] = "Unauthorized access.";
                return Unauthorized();
            }
            else if (newEmail == null)
            {
                TempData["error"] = "New email cannot be null.";
                return BadRequest();
            }
            else if (oldEmail == newEmail)
            {
                TempData["error"] = "New email cannot be the same as the old email.";
                return BadRequest();
            }

            var user = _dbcontext.Users.Find(Id);
            bool emailExists = _dbcontext.Users.Any(u => u.Email == newEmail);
            if (emailExists)
            {
                TempData["error"] = "Email already exists.";
                return BadRequest();
            }

            var updateEmailToken = Guid.NewGuid();
            user.NewEmail = newEmail;
            user.EmailChangeToken = updateEmailToken.ToString();
            user.EmailChangeTokenExpires = DateTime.UtcNow.AddDays(1);
            _dbcontext.SaveChanges();


            var email = new Email
            {
                To = _dbcontext.Users.Where(u=>u.UserId==Guid.Parse(userId)).Select(u=>u.NewEmail).FirstOrDefault(),
                Subject = "Confirm Email",
                Body = $"Please click on this link to confirm {newEmail}your Email Address: https://lilamibazzar.runasp.net/Accounts/Dashboard/ChangeEmail?token={updateEmailToken}"
            };
            _emailService.SendEmail(email);
            // Send email verification token to the user email client and later save the email

           

            TempData["success"] = "Confirmation token sent successfully!";
            return RedirectToAction("Index");
        }
        public IActionResult ChangeEmail(string token)
        {
            if(token == null)
            {
                return BadRequest();
            }
            var isTokenPresent = _dbcontext.Users.FirstOrDefault(t=>t.EmailChangeToken == token);
            if(isTokenPresent == null)
            {
                return BadRequest();
            }
            if(isTokenPresent.EmailChangeTokenExpires < DateTime.UtcNow)
            {
                return BadRequest();
            }
            isTokenPresent.Email = isTokenPresent.NewEmail;
            isTokenPresent.NewEmail = null;
            _dbcontext.SaveChanges();
            var authToken = _jwtService.AuthClaim(isTokenPresent);
            Response.Cookies.Append("Authorization", authToken, new CookieOptions
            {
                // Configure these as needed
            });
            TempData["success"] = "Email changed successfully!";
            return RedirectToAction("Index");
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            if (oldPassword == null || newPassword == null || confirmPassword == null)
            {
                TempData["error"] = "Please fill all the fields";
            return RedirectToAction("ChangePassword");

            }
            if (newPassword != confirmPassword)
            {
                TempData["error"] = "New password and confirmation do not match.";
            return RedirectToAction("ChangePassword");

            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var guUserId = Guid.Parse(userId);
            var user = _dbcontext.Users.Find(guUserId);

            if (_passwordHashingService.VerifyPasswordHash(newPassword, user.PasswordHash, user.PasswordSalt))
            {

                TempData["error"] = "New Password cannot be same as Old Password.";
                return RedirectToAction("ChangePassword");

            }

            if (!_passwordHashingService.VerifyPasswordHash(oldPassword, user.PasswordHash, user.PasswordSalt))
            {

                TempData["error"] = "Old password is incorrect.";
            return RedirectToAction("ChangePassword");

            }
            

            _passwordHashingService.GeneratePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            _dbcontext.SaveChanges();

            var token = _jwtService.AuthClaim(user);
            Response.Cookies.Append("Authorization", token, new CookieOptions
            {
                // Configure these as needed
            });

            TempData["success"] = "Password changed successfully!";
            return RedirectToAction("ChangePassword");

        }

        public IActionResult AccountVerification(string token)
        {
            if (token == null)
            {
                TempData["error"] = "Invalid verification token.";
                return RedirectToAction("Index","Home", new {area="Users"});
            }
            var isTokenValid = _dbcontext.Users.FirstOrDefault(t => t.VerificationToken == token);
            if (isTokenValid == null)
            {
                TempData["error"] = "Invalid verification token.";
                return RedirectToAction("Index","Home", new {area="Users"});

            }
            else
            {
                isTokenValid.VerifiedAt = DateTime.UtcNow;
                _dbcontext.SaveChanges();
                TempData["success"] = "Email Address Verified.";
                return RedirectToAction("Index","Home", new {area="Users"});

            }
        }

        public IActionResult UnLocked(string token)
        {
            if (token == null)
            {
                TempData["error"] = "Invalid token.";
                return BadRequest();
            }
            var user = _dbcontext.Users.FirstOrDefault(t => t.LockoutId == Guid.Parse(token));
            if (user == null)
            {
                TempData["error"] = "Invalid token.";
                return BadRequest();
            }
            user.FailedLoginAttempts = 0;
            _dbcontext.SaveChanges();

            TempData["success"] = "Account unlocked successfully!";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult LogOut()
        {
            if (Request.Cookies["Authorization"] != null)
            {
                Response.Cookies.Delete("Authorization");
                TempData["success"] = "Logged out successfully!";
            }
            return RedirectToAction("Index", "Home", new { area = "Users" });
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
