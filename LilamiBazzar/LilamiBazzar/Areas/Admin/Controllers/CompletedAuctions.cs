﻿using LilamiBazzar.DataAccess.Database;
using LilamiBazzar.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Security;

namespace LilamiBazzar.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CompletedAuctions : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        public CompletedAuctions(ApplicationDbContext dbContext)
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
    }
}
