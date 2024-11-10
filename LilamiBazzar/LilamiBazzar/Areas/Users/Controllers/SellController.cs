using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LilamiBazzar.Areas.User.Controllers
{
    [Area("Users")]
    
    public class SellController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
