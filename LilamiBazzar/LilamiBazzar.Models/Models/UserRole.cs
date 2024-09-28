using System;
using System.ComponentModel.DataAnnotations;

namespace LilamiBazzar.Models.Models
{
    public class UserRole
    {
        [Key]
        public Guid UserRoleId { get; set; }

        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }

        // Navigation properties
        public User User { get; set; }  // Reference to the User entity
        public Role Role { get; set; }  // Reference to the Role entity
    }
}
