/*using LilamiBazzar.DataAccess.Database;
using LilamiBazzar.Models.Models;
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
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult RoleManagement(Guid userId)
        {
            var user = _dbContext.Users.FirstOrDefault(u=> u.UserId == userId);
            var mapper = new RoleManagmentVM
            {
                *//*u*//*ser = user,
                Role = user.Role*//*
            };
            return View(mapper);
        }

        [HttpPost]
        public IActionResult RoleManagement(LilamiBazzar.Models.Models.RoleManagmentVM roleManagmentVM)
        {

            var user = _dbContext.Users.FirstOrDefault(u => u.UserId == roleManagmentVM.user.UserId);
            string oldRole = user.Role;



            if (!(roleManagmentVM.user.Role == oldRole))
            {
               user.Role = roleManagmentVM.user.Role;
                _dbContext.Users.Update(user);
                _dbContext.SaveChanges();
            }
            else
            {
            return RedirectToAction("Index");

            }

            return RedirectToAction("Index");
        }



        [HttpGet]
        public IActionResult GetAll()
        {
            List<LilamiBazzar.Models.Models.User> objUserList = _dbContext.Users.ToList();

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
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            _dbContext.Users.Update(objFromDb);
            _dbContext.SaveChanges();
            return Json(new { success = true, message = "Operation Successful" });
        }

    }
}
*/