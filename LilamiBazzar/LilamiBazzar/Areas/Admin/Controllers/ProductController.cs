using LilamiBazzar.Models.Models;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;
using LilamiBazzar.DataAccess.Database;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using LilamiBazzar.Utility;
using Azure.Core;
using Microsoft.EntityFrameworkCore;

namespace LilamiBazzar.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<Product> products = _context.Products.ToList();
            return View(products);
        }

        public IActionResult Category()
        {
            var categories = _context.Categories.ToList();
            return Json(categories);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateAsync(Product product)
        {
            if (!ModelState.IsValid)
            {
                return View("Index");
            }

            try
            {
                // Handle Photo Uploads
                if (product.Photos != null && product.Photos.Any())
                {
                    var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png" };
                    var uploadedImageFiles = new List<string>();

                    // Validate and save each photo
                    foreach (var photo in product.Photos)
                    {
                        var imageExtension = Path.GetExtension(photo.FileName).ToLowerInvariant();

                        // Ensure file is an image and less than 10MB
                        if (!allowedImageExtensions.Contains(imageExtension) || photo.Length > 10 * 1024 * 1024)
                        {
                            TempData["error"] = "Invalid image file(s) or file size exceeds 10MB.";
                            return RedirectToAction("Index", "Sell", new { ares = "Users" });
                        }

                        // Generate a unique filename and save image
                        var imageFilename = Guid.NewGuid().ToString() + imageExtension;
                        var imageUploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "images");

                        if (!Directory.Exists(imageUploadPath))
                        {
                            Directory.CreateDirectory(imageUploadPath);
                        }

                        var imagePath = Path.Combine(imageUploadPath, imageFilename);
                        using (var stream = new FileStream(imagePath, FileMode.Create))
                        {
                            await photo.CopyToAsync(stream);
                        }

                        uploadedImageFiles.Add(imageFilename);
                    }
                    product.PhotoFilesNames = string.Join(",", uploadedImageFiles);
                }
                else
                {
                    TempData["error"] = "Error uploading images";
                }

                // Handle Document Uploads
                if (product.Documents != null && product.Documents.Any())
                {
                    var allowedDocumentExtensions = new[] { ".pdf", ".docx", ".xlsx", ".svg", ".png" };
                    var uploadedDocumentFiles = new List<string>();

                    // Validate and save each document
                    foreach (var document in product.Documents)
                    {
                        var documentExtension = Path.GetExtension(document.FileName).ToLowerInvariant();

                        // Ensure file is a document and less than 20MB
                        if (!allowedDocumentExtensions.Contains(documentExtension) || document.Length > 20 * 1024 * 1024)
                        {
                            ViewBag.Message = "Invalid document file(s) or file size exceeds 20MB.";
                            return View("Index");
                        }

                        // Generate a unique filename and save document
                        var documentFilename = Guid.NewGuid().ToString() + documentExtension;
                        var documentUploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "documents");

                        if (!Directory.Exists(documentUploadPath))
                        {
                            Directory.CreateDirectory(documentUploadPath);
                        }

                        var documentPath = Path.Combine(documentUploadPath, documentFilename);
                        using (var stream = new FileStream(documentPath, FileMode.Create))
                        {
                            await document.CopyToAsync(stream);
                        }

                        uploadedDocumentFiles.Add(documentFilename);
                    }
                    product.DocumentsNames = string.Join(",", uploadedDocumentFiles);
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim is null)
                {
                    return Unauthorized();
                }
                var userId = Guid.Parse(userIdClaim);
                product.SellerId = userId;
                product.ProductRoles = StaticProductRoles.Pending;
                // Save product with file references
                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();


                var auction = new Auction
                {
                    AunctionId = Guid.NewGuid(),
                    ProductId = product.ProductId,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now,
                    CurrentHighestBid = product.StartingPrice,
                    IsCompleted = false
                };

                await _context.Auctions.AddAsync(auction);
                await _context.SaveChangesAsync();


                ViewBag.Message = "Files uploaded successfully!";
                return RedirectToAction("Index","Home");
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error uploading files.";
                Console.WriteLine(ex.ToString());
                return View("Index");
            }
        }
        public IActionResult Edit(Guid id)
        {
            if(id == null)
            {
                return BadRequest();
            }
            var product = _context.Products.FirstOrDefault(p=>p.ProductId == id);
            if(product == null)
            {
                return BadRequest();    
            }
            return View(product);
        }
        [HttpPost]
        public IActionResult Edit(Product product)
        {
            if(product == null)
            {
                return BadRequest();
            }
            if(product.ProductRoles == "APPROVED")
            {
                if (product.Days.ToString() == "1")
                {
                    product.ListingDate = DateTime.UtcNow;
                    product.AunctionEndDate = DateTime.UtcNow.AddDays(1);
                }

                else if (product.Days.ToString() == "3")
                {
                    product.ListingDate = DateTime.UtcNow;
                    product.AunctionEndDate = DateTime.UtcNow.AddDays(3);
                }
                else if (product.Days.ToString() == "7")
                {
                    product.ListingDate = DateTime.UtcNow;
                    product.AunctionEndDate = DateTime.UtcNow.AddDays(7);
                }
                else
                {
                    return BadRequest();
                }
               
            }
            product.ProductRoles = "APPROVED";
            _context.Update(product);
            _context.SaveChanges();
            TempData["success"] = "Data Uploaded Successfully!!";
            return RedirectToAction("Index", "Product");

        }
        public IActionResult Delete(Guid id)
        {
            if(id == null)
            {
                return BadRequest();
            }
            var product = _context.Products.FirstOrDefault(p=>p.ProductId == id);
            if(product == null)
            {
                return NotFound();
            }
            _context.Remove(product);
            _context.SaveChanges();
            TempData["success"] = "Product Item deleted successfully";
            return RedirectToAction("Index", "Product");

        }
    }
}
