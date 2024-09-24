using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LilamiBazzar.Models.Models
{
    public class EsewaPayment
    {
        public decimal Amount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal ProductServiceCharge    { get; set; }
        public decimal ProductDeliveryCharge { get; set; }
        public string ProductCode   { get; set; }
        public decimal TotalAmount { get; set; }
        public Guid TransactionId   { get; set; }
        public string Signature     { get; set; }
        public string SignedFieldNames { get; set; }

    }
}
