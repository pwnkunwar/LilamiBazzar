using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LilamiBazzar.Models.Models
{
    public class RoleManagmentVM
    {
        public User user { get; set; }  // Represents the user data
        public IEnumerable<SelectListItem> RoleList { get; set; }  // List of roles for the dropdown
        public string Role { get; set; }  // The selected role from the dropdown
    }

}
