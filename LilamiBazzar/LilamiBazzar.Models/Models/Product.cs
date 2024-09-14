using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LilamiBazzar.Models.Models
{
    public class Product
    {
        [Key]
        public Guid ProductId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Height { get; set; }
        public string? Width { get; set; }
        public string? Depth { get; set; }
        public string? Provenance { get; set; }
        public string? Location { get; set; }
        public decimal StartingPrice { get; set; }
        [NotMapped]
        public List<IFormFile>? Photos { get; set; }

        public string? PhotoFilesNames { get; set; }
        public string? DocumentsNames { get; set; }
        public string? CategoryName { get;set; }
        [NotMapped]
        public IFormFile FrontImage { get; set; }
        [NotMapped]

        public string BackImage { get;set; }
        [NotMapped]

        public string SignatureImage { get; set; }
        [NotMapped]
        public List<IFormFile>? Documents { get; set; }
        public DateTime ListingDate { get; set; }
        public DateTime AunctionEndDate { get; set; }
        public Guid SellerId { get; set; }
       // public List<Bid>? Bids { get; set; }
        /*public bool Authenticity { get; set; }
        public ItemStatus Status { get; set; }*/
    }
}
