//using LilamiBazzar.Models;
using LilamiBazzar.DataAccess.Database;
using LilamiBazzar.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using MimeKit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Ocsp;
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
        /*private readonly HttpClient _client;*/

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IConfiguration configuration/*, IHttpClientFactory httpClientFactory*/)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
            /*_client = httpClientFactory.CreateClient("KhaltiClient");*/
        }

        public IActionResult Index()
        {
            List<Product> products = _context.Products.Where(a => a.ProductRoles == "APPROVED" && a.AunctionEndDate > DateTime.UtcNow).ToList();

            return View(products);

        }
        public IActionResult Details(Guid productId)
        {
            TempData["productId"] = productId;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim is null)
            {
                userIdClaim = "f47ac10b-58cc-4372-a567-0e02b2c3d479";
            }
            var userId = Guid.Parse(userIdClaim);
            var isUserAlreadyBidder = _context.Bids.Any(b => b.UserId == userId && b.Auction.ProductId == productId);
            List<Bid> productBids = new List<Bid>();
            string username = string.Empty;
            decimal AlreadyPayAmount  = 0;  
            decimal RequiredPayAmount = 0;
            if (isUserAlreadyBidder)
            {
               
                productBids = _context.Bids.Where(b => b.Auction.ProductId == productId).ToList();
                AlreadyPayAmount = _context.Bids
    .Where(a => a.UserId == userId && a.Auction.ProductId == productId)
    .Sum(a => a.Amount);

                decimal highestBiddingAmount = _context.Auctions.Where(a => a.ProductId == productId).Select(a => a.CurrentHighestBid).FirstOrDefault();
                RequiredPayAmount = highestBiddingAmount - AlreadyPayAmount;
            }
            var AuctionEnds = _context.Auctions.Where(p => p.ProductId == productId).Select(d => d.EndDate).FirstOrDefault();

            bool canReview = _context.Auctions
    .Any(a => a.ProductId == productId && a.HighestBidderId == userId && a.IsCompleted);

            IEnumerable<dynamic> bidsWithUsers = (from bid in _context.Bids
                             join user in _context.Users
                             on bid.UserId equals user.UserId
                             where bid.Auction.ProductId == productId
                             select new
                             {
                                 Username = user.FullName,
                                 BidAmount = bid.Amount
                             }).ToList();

            IEnumerable<dynamic> Feedbacks;
            var feedbacks = _context.Reviews.Where(p => p.ProductId == productId).ToList();
            var currentHighestBid  = _context.Auctions
    .Where(p => p.ProductId == productId)
    .Max(b => (decimal?)b.CurrentHighestBid) ?? 0;
            var sellerName = _context.Products.Where(p => p.ProductId == productId).Join(
                _context.Users,
                product => product.SellerId,
                user => user.UserId,
                (product, user)=>user.FullName
                ).FirstOrDefault();

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
                    ThirdImage = thirdImage,
                    ProductBids = productBids,
                    UserName = username,
                    AlreadyPayAmount = AlreadyPayAmount,
                    RequiredPayAmount = RequiredPayAmount,
                    BidsWithUsers = bidsWithUsers,
                    AuctionEndDate = AuctionEnds,
                    CanReview = canReview,
                    ProductId = productId,
                    Feedbacks = feedbacks,
                    CurrentHighestBid = currentHighestBid,
                    SellerName = sellerName
                });
            }

        }



        [HttpPost]
        public async Task<IActionResult> PaymentsAsync([FromBody] ProductAmt productAmt)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim is null)
            {
                return Unauthorized();
            }
            var userId = Guid.Parse(userIdClaim);



            var productAunction = _context.Auctions.FirstOrDefault(a => a.ProductId == productAmt.ProductId);
            /*if(productAmt.Amount <= productAunction.CurrentHighestBid)
            {
                return BadRequest("Bid amount is less than actual");
            }*/
           /* if(productAunction.EndDate < DateTime.UtcNow)
            {
                return BadRequest("Acunction already ended");
            }*/

            decimal previousBidAmount = 0;
            decimal amountToPay = 0;
            var auction = _context.Auctions.FirstOrDefault(a => a.ProductId == productAmt.ProductId);
            var isUserAlreadyBidder = _context.Bids.FirstOrDefault(u => u.UserId == userId);
            if(isUserAlreadyBidder == null)
            {
                if(auction.CurrentHighestBid < productAmt.Amount)
                {
                    amountToPay = productAmt.Amount;

                }
                else
                {
                    return BadRequest("Amount is less");
                }
            }
            else
            {
                var previousBid = _context.Bids.Where(u => u.UserId == userId && u.AuctionId == auction.AunctionId).FirstOrDefault();
                amountToPay = auction.CurrentHighestBid - previousBid.Amount;
            }
            








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
            string totalAmount = amountToPay.ToString();
            string productCode = "EPAYTEST";
            string message = $"total_amount={totalAmount},transaction_uuid={guid},product_code={productCode}";
            string secret = _configuration.GetSection("Esewa")["Secret"];
            string hash = GenerateHMACSHA256Hash(message, secret);
            TempData["ProductId"] = productAmt.ProductId;

            var Esewa = new EsewaPayment
            {
                ProductId = productAmt.ProductId,
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
            decimal totalAmount = paymentData.total_amount; // Access total_amount
            Guid transactionId = paymentData.transaction_uuid; // Access transaction_uuid
            string productCode = paymentData.product_code;
            string message = $"total_amount={totalAmount},transaction_uuid={transactionId},product_code={productCode}";

            /*string message = $"transaction_code={transactionCode},status={status},total_amount={totalAmount},transaction_uuid={transactionId},product_code={productCode}";*/
            string secret = _configuration.GetSection("Esewa")["Secret"];
            string generatedSignature = GenerateHMACSHA256Hash(message, secret);
            string generatedSignatureBase64Decoded = Encoding.UTF8.GetString(Convert.FromBase64String(generatedSignature));
            /* if (generatedSignature != paymentData.Signature)
             {*/

           /* var paymets = new Payments
            {
                PaymentId = Guid.NewGuid(),
                TrasactionId = transactionId,
                UserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value),
                AunctionId = transactionId, 
                BidId 
                

            };
              */  

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim is null)
            {
                return Unauthorized();
            }
            var userId = Guid.Parse(userIdClaim);
               
                Guid productId = (Guid)TempData["ProductId"];
            var auction = _context.Auctions.FirstOrDefault(a => a.ProductId == productId);
            if(auction is null)
            {
                return BadRequest();
            }
                var bid = new Bid
                {
                    BidId = Guid.NewGuid(),
                    AuctionId = auction.AunctionId,
                    UserId = userId,
                    Amount = totalAmount,
                    BidTime = DateTime.Now

                };

                await _context.Bids.AddAsync(bid);
                await _context.SaveChangesAsync();

                //send mail to the users regarding the infomration of BId/Aunction
                return View();
            /*}
            else
            {
                // The signature is invalid, handle accordingly
                return BadRequest("Invalid Signature");
            }*/

        }
        //khalti integrations
        [HttpPost]
        public async Task<IActionResult> PaymentKhalti(decimal amountKhalti, Guid ProductId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim is null)
            {
                return Unauthorized();
            }
            var userId = Guid.Parse(userIdClaim);
            decimal previousBidAmount = 0;
            decimal amountToPay = 0;
            var productAunction = _context.Auctions.FirstOrDefault(a => a.ProductId == ProductId);
            if (productAunction is null)
            {
                return BadRequest();
            }
            var isUserAlreadyBidder = _context.Bids.FirstOrDefault(u => u.UserId == userId && u.Auction.ProductId == ProductId);

            if (isUserAlreadyBidder == null)
            {
                if (productAunction.CurrentHighestBid < amountKhalti)
                {
                    // as khalti take as paisa 1 Rs = 100 paisa
                    amountToPay = amountKhalti * 100;
                    TempData["amountToPay"] = amountKhalti.ToString();
                }
                else
                {
                    TempData["error"] = "Amount should be greater than Highest bidding amount!!";
                return RedirectToAction("Details", "Home", new { productId = ProductId });

                }

            }
            else
            {
                var auction = _context.Auctions.FirstOrDefault(a => a.ProductId == ProductId);
                var previousBid = _context.Bids.Where(u => u.UserId == userId && u.AuctionId == auction.AunctionId).FirstOrDefault();
                if(auction.CurrentHighestBid == previousBid.Amount)
                {
                    amountToPay = amountKhalti * 100;
                    var tAmount = previousBid.Amount + amountKhalti;
                    TempData["amountToPay"] = tAmount.ToString();

                }
                else
                {
                    decimal totalAmount = amountKhalti + previousBid.Amount;
                    if(productAunction.CurrentHighestBid < totalAmount)
                    {
                        amountToPay = amountKhalti * 100;
                        TempData["amountToPay"] = previousBid.Amount + amountKhalti;
                    }
                    else
                    {
                        TempData["error"] = "Amount should be greater than Highest bidding amount!!";
                        return RedirectToAction("Details", "Home", new { productId = ProductId });
                    }
                    
                }
                


            }

            var payload = new
            {
                return_url = "https://localhost:7136/Users/Home/PaymentVerifyKhalti",
                website_url = "https://localhost:7136/",
                amount = amountToPay,
                purchase_order_id= ProductId,
                purchase_order_name = "test"
            };
            var jsonPayload = JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            var client = new HttpClient();
            var secretKey = _configuration["Khalti:live_secret_key"];
            client.DefaultRequestHeaders.Authorization =
    new System.Net.Http.Headers.AuthenticationHeaderValue("key", secretKey);
            var response = await client.PostAsync(_configuration["Khalti:urlToMakePayment"], content);
            var repsonseContent = await response.Content.ReadAsStringAsync();

            var parsedResponse = JObject.Parse(repsonseContent);
            var paymentUrl = parsedResponse["payment_url"]?.ToString();
            if (!string.IsNullOrEmpty(paymentUrl))
            {
                // Redirect to the payment URL
                return Redirect(paymentUrl);
            }

            return RedirectToAction("Index");

        }
        [Authorize]
        public async Task<IActionResult> PaymentVerifyKhaltiAsync()
        {
            var queryParams = HttpContext.Request.Query;
            var pidx = queryParams["pidx"];
            var transactionId = queryParams["transaction_id"];
            decimal amount = decimal.Parse(queryParams["amount"]) / 100;
            var status = queryParams["status"];
            if(status == "Completed")
            {
                var payload = new
                {
                    pidx = pidx.ToString()
                };
                var jsonPayload = JsonConvert.SerializeObject(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var client = new HttpClient();
                var secretKey = _configuration["Khalti:live_secret_key"];
                client.DefaultRequestHeaders.Authorization =
        new System.Net.Http.Headers.AuthenticationHeaderValue("key", secretKey);
                var response = await client.PostAsync(_configuration["Khalti:urlToValidatePayment"], content);
                var repsonseContent = await response.Content.ReadAsStringAsync();

                var jsonObject = JObject.Parse(repsonseContent);

                // Access values dynamically
                var pidxResponse = jsonObject["pidx"]?.ToString();
                decimal totalAmount = (jsonObject["total_amount"]?.ToObject<decimal>() ?? 0) / 100;


                var statusResponse = jsonObject["status"]?.ToString();
                var transactionIdResponse = jsonObject["transaction_id"]?.ToString();
                Guid productId = (Guid)TempData["ProductId"];

                if (statusResponse == "Completed")
                {
                    if(pidx == pidxResponse && amount == totalAmount && transactionId == transactionIdResponse)
                    {
                        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        if (userIdClaim is null)
                        {
                            return Unauthorized();
                        }
                        var userId = Guid.Parse(userIdClaim);
                        var bid = new Bid
                        {
                            BidId = Guid.NewGuid(),
                            AuctionId = _context.Auctions.Where(p => p.ProductId == productId).Select(p => p.AunctionId).FirstOrDefault(),
                            UserId = userId,
                            Amount = totalAmount,
                            BidTime = DateTime.UtcNow
                        };

                        var auction = _context.Auctions.FirstOrDefault(p => p.ProductId == productId);
                        auction.CurrentHighestBid =decimal.Parse(TempData["amountToPay"].ToString());

                        /*var isUserAlreadyBidder =
                            _context.Bids.FirstOrDefault(u => u.UserId == userId && u.Auction.ProductId == productId);
                        if(isUserAlreadyBidder != null)
                        {
                            _context.Bids.Update(bid);
                            _context.SaveChanges();
                        }*/
                        _context.Auctions.Update(auction);
                        _context.Bids.Add(bid);
                        _context.SaveChanges();


                    }
                }
                return RedirectToAction("Details", "Home", new { productId = productId });

            }
            else
            {
                return BadRequest("Something went wrong");
            }


        }



        [HttpPost]
        public async Task<IActionResult> ReviewAsync([FromBody]  ReviewInput reviewInput)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim is null)
                {
                    return Unauthorized();
                }
                var userId = Guid.Parse(userIdClaim);
                Guid productId = Guid.Parse(reviewInput.ProductId);
                var isAllowedReview = _context.Auctions.Any(u => u.HighestBidderId == userId && u.ProductId == productId && u.IsCompleted);
                /*if (isAllowedReview)
                {*/
                    Guid sellerId = _context.Products.Where(s => s.ProductId == productId).Select(s => s.SellerId).FirstOrDefault();
                    var review = new Review
                    {
                        ReviewId = Guid.NewGuid(),
                        Comment = reviewInput.Review,
                        UserId = userId,
                        ProductId = productId,
                        SellerId = sellerId,
                        PositiveRating = 0,
                        NegativeRating = 0
                    };
                    if (reviewInput.ThumbsUp)
                    {
                        review.PositiveRating += 1;
                    }
                    if (reviewInput.ThumbsDown)
                    {
                        review.NegativeRating += 1;
                    }
                    await _context.Reviews.AddAsync(review);
                    await _context.SaveChangesAsync();
               /* }*/


                return View(reviewInput);


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return View();
        }

        public IActionResult Unauthorized()
        {
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