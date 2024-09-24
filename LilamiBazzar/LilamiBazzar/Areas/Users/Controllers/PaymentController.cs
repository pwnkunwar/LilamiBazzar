using LilamiBazzar.Models.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Cryptography;
using System.Text;

namespace LilamiBazzar.Areas.User.Controllers
{
    [Area("Users")]
    public class PaymentController : Controller
    {
        private readonly IConfiguration _configuration;
        public PaymentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            Guid guid = Guid.NewGuid();
            string totalAmount = "120";
            string productCode = "EPAYTEST";


            // Construct the message for signature generation
            string message = $"total_amount={totalAmount},transaction_uuid={guid},product_code={productCode}";

            string secret = _configuration.GetSection("Esewa")["Secret"];
            string hash = GenerateHMACSHA256Hash(message, secret);

            var Esewa = new EsewaPayment
            {
                Signature = hash,
                TransactionId = guid,
                ProductCode = productCode
            };

            return View(Esewa);
        }

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
