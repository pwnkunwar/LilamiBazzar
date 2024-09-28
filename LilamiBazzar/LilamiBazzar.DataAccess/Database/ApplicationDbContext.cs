using LilamiBazzar.Models.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace LilamiBazzar.DataAccess.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }  // Add UserRole DbSet
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seeding roles data
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = Guid.NewGuid(), Name = StaticUserRoles.AMDIN, Description = "Administrator role" },
                new Role { RoleId = Guid.NewGuid(), Name = StaticUserRoles.USER, Description = "Regular user role" }
            );

            // Defining the many-to-many relationship between User and Role
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => ur.UserRoleId); // Primary key

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
               .WithMany(u => u.UserRoles)  // A User can have many UserRoles
                .HasForeignKey(ur => ur.UserId); // Foreign key


            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)  // A Role can have many UserRoles
                .HasForeignKey(ur => ur.RoleId); // Foreign key
        }
    }
}
