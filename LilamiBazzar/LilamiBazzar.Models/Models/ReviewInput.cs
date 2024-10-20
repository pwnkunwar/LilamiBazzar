using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LilamiBazzar.Models.Models
{
    public class ReviewInput
    {
        public string Review { get; set; }
        public bool ThumbsUp { get; set; }
        public bool ThumbsDown { get; set; }
        public string ProductId { get; set; }
    }
}
