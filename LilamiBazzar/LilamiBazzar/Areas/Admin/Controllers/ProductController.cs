using LilamiBazzar.Models.Models;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;
using LilamiBazzar.DataAccess.Database;
using Microsoft.AspNetCore.Http;

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

        [HttpPost]
        public async Task<IActionResult> CreateAsync(Product product)
        {
            if (ModelState.IsValid)
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
                            ViewBag.Message = "Invalid image file(s) or file size exceeds 10MB.";
                            return View("Index");
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

                // Handle Document Uploads
                if (product.Documents != null && product.Documents.Any())
                {
                    var allowedDocumentExtensions = new[] { ".pdf", ".docx", ".xlsx", ".svg" };
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

                // Save product with file references
                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();

                ViewBag.Message = "Files uploaded successfully!";
                return View("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error uploading files.";
                Console.WriteLine(ex.ToString());
                return View("Index");
            }
        }
    }
}
