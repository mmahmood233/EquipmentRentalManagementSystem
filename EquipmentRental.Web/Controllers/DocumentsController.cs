using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EquipmentRental.DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace EquipmentRental.Web.Controllers
{
    [Authorize]
    public class DocumentsController : Controller
    {
        private readonly EquipmentRentalDbContext _context;
        private readonly string _uploadsFolder;

        public DocumentsController(EquipmentRentalDbContext context, Microsoft.AspNetCore.Hosting.IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "uploads");
            
            // Ensure uploads directory exists
            if (!Directory.Exists(_uploadsFolder))
            {
                Directory.CreateDirectory(_uploadsFolder);
            }
        }

        // GET: Documents for a specific transaction
        public async Task<IActionResult> Index(int? transactionId)
        {
            if (transactionId == null)
            {
                return BadRequest("Transaction ID is required");
            }

            try
            {
                var transaction = await _context.RentalTransactions
                    .Include(t => t.Customer)
                    .Include(t => t.Equipment)
                    .Include(t => t.Documents)
                    .FirstOrDefaultAsync(t => t.RentalTransactionId == transactionId);

                // Check authorization if transaction exists
                if (transaction != null)
                {
                    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                    var isAdmin = User.IsInRole("Administrator");
                    var isManager = User.IsInRole("Manager");

                    // Only allow access to the customer who owns the transaction or managers/admins
                    if (!isAdmin && !isManager && transaction.CustomerId != userId)
                    {
                        return Forbid();
                    }
                }

                // Get documents for this transaction ID even if transaction is null
                var documents = await _context.Documents
                    .Where(d => d.RentalTransactionId == transactionId)
                    .ToListAsync();

                // Set ViewBag data
                ViewBag.Transaction = transaction;
                ViewBag.TransactionId = transactionId;
                ViewBag.CanManageFiles = User.IsInRole("Administrator") || User.IsInRole("Manager");
                
                return View(documents);
            }
            catch (Exception ex)
            {
                // Log the exception
                System.Diagnostics.Debug.WriteLine($"Error in Index: {ex.Message}");
                
                // Return an empty list if there's an error
                ViewBag.TransactionId = transactionId;
                ViewBag.CanManageFiles = User.IsInRole("Administrator") || User.IsInRole("Manager");
                return View(new List<Document>());
            }
        }

        // GET: Download a document
        public async Task<IActionResult> Download(int? id)
        {
            if (id == null)
            {
                return BadRequest("Document ID is required");
            }

            var document = await _context.Documents
                .Include(d => d.RentalTransaction)
                .ThenInclude(t => t.Customer)
                .FirstOrDefaultAsync(d => d.DocumentId == id);

            if (document == null)
            {
                return NotFound("Document not found");
            }

            // Check authorization
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var isAdmin = User.IsInRole("Administrator");
            var isManager = User.IsInRole("Manager");

            // Only allow access to the customer who owns the transaction or managers/admins
            if (!isAdmin && !isManager && document.RentalTransaction?.CustomerId != userId)
            {
                return Forbid();
            }

            var filePath = document.StoragePath;
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found on server");
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, document.FileType, document.FileName);
        }

        // GET: Upload form
        [Authorize(Roles = "Manager,Administrator")]
        public async Task<IActionResult> Upload(int? transactionId)
        {
            if (transactionId == null)
            {
                return BadRequest("Transaction ID is required");
            }

            try
            {
                var transaction = await _context.RentalTransactions
                    .Include(t => t.Customer)
                    .Include(t => t.Equipment)
                    .FirstOrDefaultAsync(t => t.RentalTransactionId == transactionId);

                // Even if transaction is null, we'll still show the upload form
                // but with limited information
                ViewBag.Transaction = transaction;
                ViewBag.TransactionId = transactionId;
                return View();
            }
            catch (Exception ex)
            {
                // Log the exception
                System.Diagnostics.Debug.WriteLine($"Error in Upload GET: {ex.Message}");
                
                // Still allow the upload form to be shown with the transaction ID
                ViewBag.TransactionId = transactionId;
                return View();
            }
        }

        // POST: Upload a document
        [HttpPost]
        [Authorize(Roles = "Manager,Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(int transactionId, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Please select a file to upload";
                return RedirectToAction(nameof(Upload), new { transactionId });
            }

            var transaction = await _context.RentalTransactions.FindAsync(transactionId);
            if (transaction == null)
            {
                return NotFound("Transaction not found");
            }

            // Create a unique filename
            var fileName = Path.GetFileName(file.FileName);
            var fileExtension = Path.GetExtension(fileName);
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(_uploadsFolder, uniqueFileName);

            // Save the file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Create document record
            var document = new Document
            {
                RentalTransactionId = transactionId,
                UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0"),
                FileName = fileName,
                FileType = file.ContentType,
                StoragePath = filePath,
                UploadedAt = DateTime.Now
            };

            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            TempData["Success"] = "File uploaded successfully";
            return RedirectToAction(nameof(Index), new { transactionId });
        }

        // POST: Delete a document
        [HttpPost]
        [Authorize(Roles = "Manager,Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int transactionId)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document == null)
            {
                return NotFound();
            }

            // Delete the physical file
            if (System.IO.File.Exists(document.StoragePath))
            {
                System.IO.File.Delete(document.StoragePath);
            }

            // Delete the database record
            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();

            TempData["Success"] = "File deleted successfully";
            return RedirectToAction(nameof(Index), new { transactionId });
        }
    }
}
