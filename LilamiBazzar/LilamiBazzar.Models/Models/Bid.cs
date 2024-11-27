using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LilamiBazzar.Models.Models
{
    public class Bid
    {
        [Key]
        public Guid BidId { get; set; }

        // Foreign Key to Auction
        public Guid AuctionId { get; set; }

        // Navigation property for the related Auction
        public Auction Auction { get; set; }

        // Foreign Key to User (the Bidder)
        public Guid UserId { get; set; }

        // Navigation property for the related User (the Bidder)

        public decimal Amount { get; set; }
        public DateTime BidTime { get; set; }
        public User User { get; set; }
        [NotMapped]
        public Guid ProductId { get; set; }
        [NotMapped]
        public Product Product { get; set; }
    }
}
