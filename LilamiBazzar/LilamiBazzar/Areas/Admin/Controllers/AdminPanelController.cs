using LilamiBazzar.DataAccess.Database;
using LilamiBazzar.Models.Models;
using Microsoft.AspNetCore.Mvc;

namespace LilamiBazzar.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminPanelController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        public AdminPanelController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            int totalUsers = _dbContext.Users.Count();
            int totalListings = _dbContext.Products.Where(p => p.ProductRoles == "APPROVED").Count();
            int totalOngoingAuction = _dbContext.Auctions.Where(a=>a.EndDate > DateTime.UtcNow).Count();
            AdminPanel adminPanel =new AdminPanel
            {
                TotalUsers = totalUsers,
                TotalListings = totalListings,
                TotalOngoingAuction = totalOngoingAuction

            };
            return View(adminPanel);

        }
    }
}
