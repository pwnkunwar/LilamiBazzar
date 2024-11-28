using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LilamiBazzar.Models.Models
{
    public class CompletedAuction
    {
        public Guid ProductId { get; set; }
        public string Title { get; set; }
        public decimal StartingPrice { get; set; }
        public decimal HighestBid {  get; set; }
        public string Location { get; set; }
        public string Category { get; set; }
        public string ShippingProvider { get; set; }
        public string BuyerName { get; set; }
        public string SellerName { get; set; }
        public string DeliveryStatus { get; set; }
        public DateTime EstimatedDeliveryDate { get; set; }
    }
}
