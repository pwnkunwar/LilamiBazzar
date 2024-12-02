using LilamiBazzar.DataAccess.Database;
using LilamiBazzar.DataAccess.DbInitializer;
using LilamiBazzar.Models.Models;
using LilamiBazzar.Services.PasswordHashingService;
using LilamiBazzar.Utility;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

public class DbInitializer : IDbInitializer
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IPasswordHashingService _passwordHashingService;

    public DbInitializer(ApplicationDbContext applicationDbContext, IPasswordHashingService passwordHashingService)
    {
        _applicationDbContext = applicationDbContext;
        _passwordHashingService = passwordHashingService;
    }

    public async void Initialize()
    {
        // Apply pending migrations
        try
        {
            if (_applicationDbContext.Database.GetPendingMigrations().Any())
            {
                 _applicationDbContext.Database.Migrate();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error applying migrations: {ex}");
            return;
        }

        await CreateAdminUserAndRolesAsync();
    }

    private async Task CreateAdminUserAndRolesAsync()
    {
        const string adminEmail = "pwnkunwar@gmail.com";
        const string adminPassword = "pwnkunwar";
        const string adminRoleName = StaticUserRoles.ADMIN;

        // Check if the admin user already exists
        var existingAdminUser =  _applicationDbContext.Users.FirstOrDefault(u => u.Email == adminEmail);
        if (existingAdminUser == null)
        {
            // Hash the password
            _passwordHashingService.GeneratePasswordHash(adminPassword, out byte[] passwordHash, out byte[] passwordSalt);

            // Create the admin user
            var adminUser = new User
            {
                UserId = Guid.NewGuid(),
                Email = adminEmail,
                FullName = "Admin User",
                Address = "localhost",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                VerifiedAt = DateTime.UtcNow
            };

             _applicationDbContext.Users.Add(adminUser);

            // Create the admin role if it doesn't exist
            var adminRole =  _applicationDbContext.Roles.FirstOrDefault(r => r.Name == adminRoleName);
            if (adminRole == null)
            {
                adminRole = new Role
                {
                    RoleId = Guid.NewGuid(),
                    Name = adminRoleName
                };

                 _applicationDbContext.Roles.Add(adminRole);
            }

            // Assign the admin role to the user if not already assigned
            var userRoleExists =  _applicationDbContext.UserRoles
                .Any(ur => ur.UserId == adminUser.UserId && ur.RoleId == adminRole.RoleId);

            if (!userRoleExists)
            {
                var userRole = new UserRole
                {
                    UserId = adminUser.UserId,
                    RoleId = adminRole.RoleId
                };

                 _applicationDbContext.UserRoles.Add(userRole);
            }

            // Save all changes in one batch
            _applicationDbContext.SaveChanges();
        }
        else
        {
            var userRole = _applicationDbContext.UserRoles.FirstOrDefault(r=>r.UserId == existingAdminUser.UserId);
            if (userRole == null)
            {
            var adminRole =  _applicationDbContext.Roles.FirstOrDefault(r => r.Name == adminRoleName);
                var uRole = new UserRole
                {
                    UserRoleId = Guid.NewGuid(),
                    RoleId = adminRole.RoleId,
                    UserId = existingAdminUser.UserId
                };
                _applicationDbContext.UserRoles.Add(uRole);
                _applicationDbContext.SaveChanges();

            }
            return;
        }

    }
}
