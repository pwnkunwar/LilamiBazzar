using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LilamiBazzar.Areas.Accounts
{
    [Area("Accounts")]

    public class DashboardController : Controller
    {
        /*[Authorize(Roles = "USER")]*/
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Email()
        {
            return View();
        }
        public IActionResult ChangePassword()
        {
            return View();
        }
        public IActionResult TwoFactorAuthentication()
        {
            return View();
        }
        public IActionResult PersonalData()
        {
            return View();
        }
    }
}
