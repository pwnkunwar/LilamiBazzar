using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LilamiBazzar.Models.Models
{
    public class Bid
    {
        public Guid BidId { get; set; }
        public Auction Auction { get; set; }
        public User Bidder { get; set; }
        public decimal Amount { get; set; }
        public DateTime BidTime { get; set; }

    }
}
