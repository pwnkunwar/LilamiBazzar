using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LilamiBazzar.Models.Models
{
    public class ItemTracking
    {
        public Guid ItemTrackingId { get; set; }
        public Guid ItemId { get; set; }
        public Guid BuyerId { get; set; }
        public Guid SellerId { get; set; }
        public string CurrentStatus { get; set; } // e.g., Pending, Shipped, In Transit, Delivered
        public DateTime StatusUpdatedAt { get; set; }
        public string ShippingProvider { get; set; } //  e.g., FedEx, UPS
        public string TrackingNumber { get; set; } // Tracking number provided by the courier
        public DateTime EstimatedDeliveryDate { get; set; }
        public DateTime DeliverdAt { get; set; }

    }
}
