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
                    /*obj = _dbContext.Products.Where(b => b.ProductRoles == "PENDING" && b.bid.UserId == userId).ToList();*/
                    obj = _dbContext.Products.Where(u => u.SellerId == userId && u.ProductRoles == "PENDING").ToList();
                    break;
                case "inprocess":
                    obj = _dbContext.Products.Where(u => u.SellerId == userId && u.ProductRoles == "APPROVED" && u.AunctionEndDate > DateTime.UtcNow).ToList();
                    break;
                case "completed":
                    obj = _dbContext.Products.Where(u => u.SellerId == userId && u.AunctionEndDate < DateTime.UtcNow).ToList();
                    break;
                case "rejected":
                    obj = _dbContext.Products.Where(u => u.SellerId == userId && u.ProductRoles == "REJECTED").ToList();
                    break;
                case "purchased":
                    obj = _dbContext.Products.Where(u => u.Auction.HighestBidderId == userId && u.Auction.IsCompleted == true).ToList();
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
            if(product == null)
            {
                return BadRequest("BadRequest");
            }
            var pdt = _dbContext.Products.FirstOrDefault(P=>P.ProductId == product.ProductId);
       
            if (pdt is not null)
            {
                pdt.Title = product.Title;
                pdt.Description = product.Description;
                pdt.Location = product.Location;
                pdt.StartingPrice = product.StartingPrice;
                pdt.CategoryName = product.CategoryName;
                pdt.Days = product.Days;
                _dbContext.Products.Update(pdt);
                _dbContext.SaveChanges();
                TempData["success"] = "Updated Successfully!";
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult Delete(Guid orderId)
        {
            var product = _dbContext.Products.FirstOrDefault(p => p.ProductId == orderId);
            if (product == null)
            {
                return BadRequest();
            }
            _dbContext.Products.Remove(product);
            _dbContext.SaveChanges();
            return RedirectToAction("Index", "Order", new { area = "Admin" });
        }
        public IActionResult Purchased(Guid productId)
        {
            if (productId == Guid.Empty)
            {
                return BadRequest();
            }
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim is null)
            {
                return Unauthorized();
            }
            var userId = Guid.Parse(userIdClaim);
            var isCompletedAuction = _dbContext.Auctions.Any(i => i.IsCompleted == true);
            if (isCompletedAuction)
            {
                var completedAuctions = (from auction in _dbContext.Auctions
                                         join product in _dbContext.Products
                                         on auction.ProductId equals product.ProductId
                                         join seller in _dbContext.Users
                                         on product.SellerId equals seller.UserId
                                         join itemTracking in _dbContext.ItemTracking
                                         on product.ProductId equals itemTracking.ItemId
                                         join buyer in _dbContext.Users
                                         on auction.HighestBidderId equals buyer.UserId
                                         where auction.ProductId == productId && buyer.UserId == userId
                                         select new CompletedAuction
                                         {
                                             ProductId = product.ProductId,
                                             Title = product.Title,
                                             StartingPrice = product.StartingPrice,
                                             HighestBid = auction.CurrentHighestBid,
                                             Location = product.Location,
                                             Category = product.CategoryName,
                                             ShippingProvider = itemTracking.ShippingProvider,
                                             BuyerName = buyer.FullName,
                                             SellerName = seller.FullName,
                                             DeliveryStatus = itemTracking.CurrentStatus,
                                             EstimatedDeliveryDate = itemTracking.EstimatedDeliveryDate
                                         }).FirstOrDefault();

                return View(completedAuctions);
            }
            return View();
        }
    }
}
