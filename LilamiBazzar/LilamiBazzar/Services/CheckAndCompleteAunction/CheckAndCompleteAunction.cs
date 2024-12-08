using LilamiBazzar.DataAccess.Database;
using LilamiBazzar.Models.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection; // To create service scopes
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LilamiBazzar.Services.CheckAndCompleteAuction
{
    public class CheckAndCompleteAuction : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public CheckAndCompleteAuction(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider; // Inject service provider to resolve scoped services
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CheckAndCompleteAuctionAsync();
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
             }
        }

        private async Task CheckAndCompleteAuctionAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(); // Create scoped DbContext

                var now = DateTime.UtcNow;
                var completedAuctions = await _context.Auctions
                    .Where(a => a.EndDate <= now && !a.IsCompleted)
                    .ToListAsync();

                foreach (var auction in completedAuctions)
                {
                    var maxTotalBid = _context.Bids
        .GroupBy(bid => bid.UserId)  // Group by UserId
        .Select(group => new
        {
            UserId = group.Key,
            TotalBidAmount = group.Sum(bid => bid.Amount)
        })
        .Max(x => x.TotalBidAmount);  // Get the max bid amount

                    // Retrieve the user(s) with the max bid
                    var topBidder = _context.Bids
                        .GroupBy(bid => bid.UserId)
                        .Select(group => new
                        {
                            UserId = group.Key,
                            TotalBidAmount = group.Sum(bid => bid.Amount)
                        })
                        .Where(x => x.TotalBidAmount == maxTotalBid)
                        .FirstOrDefault();

                    if (topBidder != null)
                    {
                        auction.HighestBidderId = topBidder.UserId;
                        auction.CurrentHighestBid = maxTotalBid;
                        auction.IsCompleted = true;
                        _context.SaveChanges();
                        var itemTracking = new ItemTracking
                        {
                            ItemTrackingId = Guid.NewGuid(),
                            ItemId = auction.ProductId,


                            // Check for null before accessing HighestBidderId
                            BuyerId = auction.HighestBidderId ?? Guid.Empty,

                            // Check for null before accessing SellerId
                            SellerId = auction.Product?.SellerId ?? Guid.Empty,  // Use null-conditional operator and default to Guid.Empty if null

                            CurrentStatus = "PENDING",
                            StatusUpdatedAt = DateTime.UtcNow,
                            ShippingProvider = "FedEx",
                            TrackingNumber = Guid.NewGuid().ToString(),
                            EstimatedDeliveryDate = DateTime.UtcNow.AddDays(7),
                            DeliverdAt = DateTime.UtcNow
                        };
                        _context.ItemTracking.Add(itemTracking);
                    }
                }
               
                await _context.SaveChangesAsync();
            }
        }
    }
}
