using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LilamiBazzar.Models.Models
{
    public class Auction
    {
        [Key]
        public Guid AunctionId { get; set; }
        public Guid ProductId { get; set; }
       
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal CurrentHighestBid { get; set; }
        [NotMapped]
        public User HighestBidder { get; set; }
        [NotMapped]
        public List<Bid> Bids { get; set; }
        public bool IsCompleted { get; set; }
    }
}
