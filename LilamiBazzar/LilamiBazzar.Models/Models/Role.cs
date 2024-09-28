using System;
using System.Collections.Generic;

namespace LilamiBazzar.Models.Models
{
    public class Role
    {
        public Guid RoleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // Add this collection navigation property
        public ICollection<UserRole> UserRoles { get; set; } // A Role can have many UserRoles
    }
}
