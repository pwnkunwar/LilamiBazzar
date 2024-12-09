using Microsoft.AspNetCore.Mvc;

namespace LilamiBazzar.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminPanelController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
