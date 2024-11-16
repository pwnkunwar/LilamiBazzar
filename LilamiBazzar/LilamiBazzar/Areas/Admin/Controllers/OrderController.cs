using LilamiBazzar.DataAccess.Database;
using LilamiBazzar.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                return Json(null);
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
            if (obj.Count > 0)
            {
                return Json(new { data = obj });
            }
            return Json(null);
        }
        public IActionResult Details(Guid orderId)
        {
            if (orderId == null)
            {
                return BadRequest();
            }
            var product = _dbContext.Products.FirstOrDefault(p => p.ProductId == orderId);
            if (product == null)
            {
                return BadRequest();
            }
            return View(product);
        }
        [HttpPost]
        public IActionResult Details(Product product)
        {
            if (ModelState.IsValid)
            {
                if (product.Photos != null && product.Photos.Any())
                {
                    // Handle file saving logic here
                }

                // Other logic...
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult Delete(Guid orderId)
        {
            if(orderId == null)
            {
                return BadRequest();
            }
            var product = _dbContext.Products.FirstOrDefault(p=>p.ProductId == orderId);
            if(product == null)
            {
                return BadRequest();
            }
            _dbContext.Products.Remove(product);
            _dbContext.SaveChanges();
            return RedirectToAction("Index","Order",new {area = "Admin"});
        }
    }
    
}
