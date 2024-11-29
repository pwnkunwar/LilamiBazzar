using LilamiBazzar.DataAccess.Database;
using LilamiBazzar.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Security;

namespace LilamiBazzar.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CompletedAuctionsController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        public CompletedAuctionsController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            var isCompletedAuction = _dbContext.Auctions.Any(i => i.IsCompleted == true);
            if (isCompletedAuction) {
                var completedAuctions = (from auction in _dbContext.Auctions
                                         join product in _dbContext.Products
                                         on auction.ProductId equals product.ProductId
                                         join seller in _dbContext.Users
                                         on product.SellerId equals seller.UserId
                                         join itemTracking in _dbContext.ItemTracking
                                         on product.ProductId equals itemTracking.ItemId
                                         join buyer in _dbContext.Users
                                         on auction.HighestBidderId equals buyer.UserId
                                         select new
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
                                             DeliveryStatus = itemTracking.CurrentStatus
                                         }).ToList();

                return View(completedAuctions);
            }

            return View();
        }
        [HttpGet]
        public IActionResult Edit(Guid id)
        {
            if(id == Guid.Empty)
            {
                return NotFound();
            }
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
                                         where auction.ProductId == id
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
                                             DeliveryStatus = itemTracking.CurrentStatus
                                         }).FirstOrDefault();

                return View(completedAuctions);
            }
            return View();
        }
        [HttpPost]
        public IActionResult Update(string ShippingProvider, string DeliveryStatus, Guid ProductId)
        {
            if(DeliveryStatus == null && ShippingProvider == null && ProductId == Guid.Empty)
            {
                return BadRequest();
            }
            var itemTracking = _dbContext.ItemTracking.FirstOrDefault(p => p.ItemId == ProductId);
            if(itemTracking == null)
            {
                return BadRequest();
            }
            itemTracking.CurrentStatus = DeliveryStatus;
            itemTracking.ShippingProvider = ShippingProvider;
            _dbContext.SaveChanges();
            TempData["success"] = "Auction data updated successfully!";
            return RedirectToAction("Index");
        }
    }
}
