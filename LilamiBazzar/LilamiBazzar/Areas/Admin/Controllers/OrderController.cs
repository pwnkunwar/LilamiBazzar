using LilamiBazzar.DataAccess.Database;
using LilamiBazzar.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Security;
using System.Security.Claims;

namespace LilamiBazzar.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        public OrderController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext; 
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult GetAll(string status)
        {
            List<Product> obj = new List<Product>();
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim is null)
            {
                return Unauthorized();
            }
            var userId = Guid.Parse(userIdClaim);

            switch (status)
            {
                case "pending":
                    obj = _dbContext.Products.Where(u => u.SellerId == userId && u.ProductRoles == "PENDING").ToList();
                    break;
                case "inprocess":
                    obj = _dbContext.Products.Where(u => u.SellerId == userId && u.ProductRoles == "APPROVED").ToList();
                    break;
                case "completed":
                    obj = _dbContext.Products.Where(u => u.SellerId == userId && u.AunctionEndDate < DateTime.UtcNow).ToList();
                    break;
                case "rejected":
                    obj = _dbContext.Products.Where(u => u.SellerId == userId && u.ProductRoles == "REJECTED").ToList();
                    break;
                default:
                    break;
            }
            return Json(new { data = obj });
        }
    }
    
}
