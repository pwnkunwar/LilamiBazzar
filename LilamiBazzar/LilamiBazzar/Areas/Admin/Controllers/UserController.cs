using LilamiBazzar.DataAccess.Database;
using LilamiBazzar.Models.Models;
using LilamiBazzar.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace LilamiBazzar.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IJwtService _jwtService;
        public UserController(ApplicationDbContext dbContext,
            IJwtService jwtService)
        {
            _dbContext = dbContext;
            _jwtService = jwtService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RoleManagement(Guid userId)
        {
            // Fetch the user
            var user = _dbContext.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null)
            {
                return NotFound();
            }

            // Fetch the user role
            var userRole = _dbContext.UserRoles.FirstOrDefault(u => u.UserId == user.UserId);
            if (userRole == null)
            {
                return NotFound();
            }

            // Fetch the role name
            var roleName = _dbContext.Roles
                .Where(r => r.RoleId == userRole.RoleId)
                .Select(r => r.Name)
                .FirstOrDefault();

            // Map to the view model
            var mapper = new RoleManagmentVM
            {
                UserId = user.UserId,
                Name = user.FullName,
                Role = roleName
            };

            return View(mapper);
        }



        [HttpPost]
        public IActionResult RoleManagement(LilamiBazzar.Models.Models.RoleManagmentVM roleManagmentVM)
        {

            var user = _dbContext.Users.FirstOrDefault(u => u.UserId == roleManagmentVM.UserId);
            if (user == null)
            {
                return NotFound();
            }
            if(roleManagmentVM.Role == "ADMIN")
            {
                Guid roleId = _dbContext.Roles
    .Where(rn => rn.Name == roleManagmentVM.Role)
    .Select(rn => rn.RoleId)
    .FirstOrDefault();

                var role = _dbContext.UserRoles.FirstOrDefault(u => u.UserId == roleManagmentVM.UserId);
                role.RoleId = roleId;
                _dbContext.Update(role);
                _dbContext.SaveChanges();



            }
            else if(roleManagmentVM.Role == "USER")
            {
                Guid roleId = _dbContext.Roles
.Where(rn => rn.Name == roleManagmentVM.Role)
.Select(rn => rn.RoleId)
.FirstOrDefault();

                var role = _dbContext.UserRoles.FirstOrDefault(u => u.UserId ==roleManagmentVM.UserId);
                if (role == null)
                {
                    return BadRequest();
                }
                role.RoleId = roleId;
                _dbContext.Update(role);
                _dbContext.SaveChanges();

            }
            var token = _jwtService.AuthClaim(user);
            Response.Cookies.Append("Authorization", token, new CookieOptions
            {
                // Configure these as needed
            });


            return RedirectToAction("Index");
        }



        [HttpGet]
        public IActionResult GetAll()
        {
            var objUserList = _dbContext.Users
                .Join(_dbContext.UserRoles,
                user => user.UserId,
                userRole => userRole.UserId,
                (user, userRole) => new
                {
                    user.UserId,
                    user.FullName,
                    user.Email,
                    user.Address,
                    Role = userRole.Role.Name,
                    LockoutEnd = user.LockoutEnd
                })
                .ToList();



            return Json(new { data = objUserList });
        }


        [HttpPost]
        public IActionResult LockUnlock([FromBody] Guid id)
        {

            var objFromDb = _dbContext.Users.FirstOrDefault(u => u.UserId == id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }

            if (objFromDb.LockoutEnd != null && objFromDb.LockoutEnd > DateTime.Now)
            {
                //user is currently locked and we need to unlock them
                objFromDb.LockoutEnd = DateTime.UtcNow;
            }
            else
            {
                objFromDb.LockoutEnd = DateTime.UtcNow.AddYears(100);
            }
            _dbContext.Users.Update(objFromDb);
            _dbContext.SaveChanges();
            return Json(new { success = true, message = "Operation Successful" });
        }
    }
}


