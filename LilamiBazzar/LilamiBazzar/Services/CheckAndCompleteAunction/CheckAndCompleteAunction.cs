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
                    var winningBid = await _context.Bids
                        .Where(b => b.AuctionId == auction.AunctionId)
                        .OrderByDescending(b => b.Amount)
                        .FirstOrDefaultAsync();

                    if (winningBid != null)
                    {
                        auction.HighestBidderId = winningBid.UserId;
                        auction.CurrentHighestBid = winningBid.Amount;
                        auction.IsCompleted = true;

                        var itemTracking = new ItemTracking
                        {
                            ItemTrackingId = Guid.NewGuid(),
                            ItemId = auction.ProductId,
                            BuyerId = auction.HighestBidderId ?? Guid.Empty,
                            SellerId = auction.Product.SellerId,
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
