//using LilamiBazzar.Models;
using LilamiBazzar.DataAccess.Database;
using LilamiBazzar.Models.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Transactions;

namespace LilamiBazzar.Areas.User.Controllers
{
    [Area("Users")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            List<Product> products = _context.Products.ToList();

            return View(products);

        }
        public IActionResult Details(Guid productId)
        {
            


            var product = _context.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product is null)
            {
                return NotFound();
            }
            else
            {
                var images = product.PhotoFilesNames.Split(',');
                var firstImage = images.Length >= 3 ? images[2] : images.LastOrDefault();
                var secondImage = images.Length >= 3 ? images[2] : images.LastOrDefault();
                var thirdImage = images.Length >= 3 ? images[2] : images.LastOrDefault();


                

                return View(new
                {
                    Product = product,
                    FirstImage = firstImage,
                    SecondImage = secondImage,
                    ThirdImage = thirdImage
                });
            }

        }



        [HttpPost]
        public async Task<IActionResult> PaymentsAsync([FromBody] ProductAmt productAmt)
        {

            

           
            var product = await _context.Auctions.FirstOrDefaultAsync(p => p.ProductId == productAmt.ProductId);
/*            var aunctionEndDate = product.EndDate;
            if (aunctionEndDate < DateTime.Now)
            {
                return BadRequest("Aunction already finished");
            }
            if (product.CurrentHighestBid > productAmt.Amount)
            {
                return BadRequest("total amount is less than currenthighestBid");
            }*/


            Guid guid = Guid.NewGuid();
            string totalAmount = productAmt.Amount.ToString();
            string productCode = "EPAYTEST";
            string message = $"total_amount={totalAmount},transaction_uuid={guid},product_code={productCode}";
            string secret = _configuration.GetSection("Esewa")["Secret"];
            string hash = GenerateHMACSHA256Hash(message, secret);

            var Esewa = new EsewaPayment
            {
                TotalAmount = productAmt.Amount,
                Signature = hash,
                TransactionId = guid,
                ProductCode = productCode
            };

            return Json(Esewa);
           

        }
        public async Task<IActionResult> PaymentVerify(string data)
        {
            string decodedString = Encoding.UTF8.GetString(Convert.FromBase64String(data));
            var paymentData = JsonConvert.DeserializeObject<dynamic>(decodedString);
            string transactionCode = paymentData.transaction_code;
            string status = paymentData.status;
            string totalAmount = paymentData.total_amount; // Access total_amount
            string transactionId = paymentData.transaction_uuid; // Access transaction_uuid
            string productCode = paymentData.product_code;
            string message = $"transaction_code={transactionCode},status={status},total_amount={totalAmount},transaction_uuid={transactionId},product_code={productCode}";
            string secret = _configuration.GetSection("Esewa")["Secret"];
            string generatedSignature = GenerateHMACSHA256Hash(message, secret);
            string generatedSignatureBase64Decoded = Encoding.UTF8.GetString(Convert.FromBase64String(generatedSignature));
            if (generatedSignature.Trim() == paymentData.Signature.ToString().Trim())
            {
                // The signature is valid, proceed with further processing
                return Ok("Payment Verified");
            }
            else
            {
                // The signature is invalid, handle accordingly
                return BadRequest("Invalid Signature");
            }

        }


        public async Task<IActionResult> AunctionAsync(Guid productId, decimal total_amount)
        {

            var product = await _context.Auctions.FirstOrDefaultAsync(p => p.ProductId == productId);
            var aunctionEndDate = product.EndDate;
            if (aunctionEndDate < DateTime.Now)
            {
                return BadRequest("Aunction already finished");
            }
            if (product.CurrentHighestBid > total_amount)
            {
                return BadRequest("total amount is less than currenthighestBid");
            }



            var gId = Guid.NewGuid();
            var auction = new Auction
            {
                AunctionId = gId,
                ProductId = productId,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
                CurrentHighestBid = total_amount,
                IsCompleted = false
            };
            await _context.Auctions.AddAsync(auction);
            await _context.SaveChangesAsync();

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userId = Guid.Parse(userIdClaim);

            var bid = new Bid
            {
                BidId = Guid.NewGuid(),
                AuctionId = gId,
                UserId = userId,
                Amount = total_amount,
                BidTime = DateTime.Now

            };

            await _context.Bids.AddAsync(bid);
            await _context.SaveChangesAsync();

            //send mail to the users regarding the infomration of BId/Aunction
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        /* [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
         public IActionResult Error()
         {
             return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
         }*/

     

        public string GenerateHMACSHA256Hash(string message, string secret)
        {
            try
            {
                // Create the HMACSHA256 object with the secret key
                using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret)))
                {
                    // Compute the hash
                    var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));

                    // Convert the byte array to a Base64 string
                    string hash = Convert.ToBase64String(hashBytes);

                    // Return the generated hash
                    return hash;
                }
            }
            catch (Exception e)
            {
                // Handle any errors appropriately
                throw new Exception("Error generating HMACSHA256 hash", e);
            }
        }
    }

}