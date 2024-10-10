using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LilamiBazzar.Models.Models
{
    public class Payments
    {
        public Guid PaymentId { get; set; }
        public Guid TrasactionId { get; set; }
        public Guid UserId { get; set; }
        public Guid AunctionId { get; set; }
        public Guid BidId { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Signature { get; set; }

    }
}
