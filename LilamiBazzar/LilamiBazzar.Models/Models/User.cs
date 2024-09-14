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
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public string? Email { get; set; }
        [NotMapped]
        public string Password { get; set; }
        public byte[] PasswordHash { get; set; } = new byte[32];
        public byte[] PasswordSalt { get; set; } = new byte[32];

        public string VerificationToken { get; set; } = string.Empty;

        public DateTime? VerifiedAt { get; set; }
        public string PasswordResetToken { get; set; } = string.Empty;
        public DateTime? PasswordResetTokenExpires { get; set; }

        public string? Role { get; set; }

    }
    

}
