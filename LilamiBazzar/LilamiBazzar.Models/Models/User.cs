using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LilamiBazzar.Models.Models
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; }
        //public string UserName { get; set; }
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        [NotMapped]
        public string Password { get; set; }
        public byte[] PasswordHash { get; set; } = new byte[32];
        public byte[] PasswordSalt { get; set; } = new byte[32];

        public string VerificationToken { get; set; } = string.Empty;

        public DateTime? VerifiedAt { get; set; }
        public string PasswordResetToken { get; set; } = string.Empty;
        public DateTime PasswordResetTokenExpires { get; set; }
        public string? NewEmail { get; set; }
        public string? EmailChangeToken {  get; set; } 
        public DateTime? EmailChangeTokenExpires { get; set; }

        public Guid  LockoutId { get; set; } 
        [NotMapped]
        public ICollection<UserRole> UserRoles { get; set; }
        public int FailedLoginAttempts { get; set; } = 0;
    }


}
