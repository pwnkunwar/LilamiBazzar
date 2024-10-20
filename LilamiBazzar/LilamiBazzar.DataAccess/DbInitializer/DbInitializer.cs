using LilamiBazzar.DataAccess.Database;
using LilamiBazzar.Models.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LilamiBazzar.Services.PasswordHashingService;
using LilamiBazzar.Utility;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
namespace LilamiBazzar.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IPasswordHashingService _psswdHash;
        public DbInitializer(ApplicationDbContext applicationDbContext, IPasswordHashingService psswdHash)
        {
            _applicationDbContext = applicationDbContext;
            _psswdHash = psswdHash;
        }
        public async void Initialize()
        {
            //apply migrations if they are not applied
            try
            {
                if(_applicationDbContext.Database.GetPendingMigrations().Any())
                {
                    _applicationDbContext.Database.Migrate();
                }
            }catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
           await CreateRoles();
        }
        private async Task CreateRoles()
        {
            var user = _applicationDbContext.Users.FirstOrDefault(e => e.Email == "pwnkunwar@gmail.com");
            if (user == null)
            {
                // Call the GeneratePasswordHash method
                _psswdHash.GeneratePasswordHash("pwnkunwar", out byte[] passwordHash, out byte[] passwordSalt);
                var adminUser = new User
                {
                    UserId = Guid.NewGuid(),
                    Email = "pwnkunwar@gmail.com",
                    Address = "localhost",
                    FullName = "admin",
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt
                };
                _applicationDbContext.Users.Add(adminUser);
                _applicationDbContext.SaveChanges();

                var role = _applicationDbContext.Roles.FirstOrDefault(r => r.Name == StaticUserRoles.ADMIN);

                var userRole = new UserRole
                {
                    UserId = adminUser.UserId,
                    RoleId = role.RoleId
                };
                _applicationDbContext.UserRoles.Add(userRole);
                _applicationDbContext.SaveChanges();
            }
            else
            {
                return;
            }
            

        }
    }
}
