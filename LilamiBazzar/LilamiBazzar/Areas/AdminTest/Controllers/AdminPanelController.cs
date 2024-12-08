using Microsoft.AspNetCore.Mvc;

namespace LilamiBazzar.Areas.AdminTest.Controllers
{
    [Area("AdminTest")]
    public class AdminPanelController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
