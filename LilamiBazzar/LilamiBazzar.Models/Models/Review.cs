using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LilamiBazzar.Models.Models
{
    public class Review
    {
        [Key]
        public Guid ReviewId { get; set; }

        public Guid ProductId { get; set; }
        public virtual Product Item { get; set; }
        public Guid SellerId { get; set; }
        public Guid UserId { get; set; }
        public virtual User Reviewer { get; set; }

        public bool IsLiked { get; set; }
        public int Rating { get; set; }
        [MaxLength(1000)]
        public string Comment { get; set; }
        public DateTime ReviewDate { get; set; } = DateTime.UtcNow;

    }
}
