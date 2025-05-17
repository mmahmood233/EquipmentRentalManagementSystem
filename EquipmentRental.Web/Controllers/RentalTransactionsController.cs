using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AdvancedProgrammingASPProject.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectDBClassLibrary.Model;

namespace AdvancedProgrammingASPProject.Controllers
{
    [Authorize]
    public class RentalTransactionsController : Controller
    {
        private readonly ProjectDBContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<Users> _userManager;

        public RentalTransactionsController(ProjectDBContext context, IWebHostEnvironment webHostEnvironment, UserManager<Users> userManager)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }

        private async Task<(User mainUser, bool isAdminOrManager, bool isCustomer)> GetCurrentUser()
        {
            var identityUser = await _userManager.GetUserAsync(User);
            var mainUser = identityUser?.MainUserId != null
                ? await _context.Users.FindAsync(identityUser.MainUserId)
                : null;

            bool isAdminOrManager = mainUser != null && (mainUser.RoleId == 1 || mainUser.RoleId == 3);
            bool isCustomer = mainUser != null && mainUser.RoleId == 2;

            return (mainUser, isAdminOrManager, isCustomer);
        }

        public async Task<IActionResult> Index(int? searchId, int? equipmentFilter, string? paymentStatusFilter, int page = 1)
        {
            int pageSize = 15;

            var (mainUser, isAdminOrManager, isCustomer) = await GetCurrentUser();

            var transactions = _context.RentalTransactions
                .Include(r => r.Equipment)
                .Include(r => r.RentalRequest)
                .Include(r => r.User)
                .Include(r => r.ReturnInfos)
                .AsQueryable();

            if (isCustomer)
                transactions = transactions.Where(t => t.UserId == mainUser.UserId);

            if (equipmentFilter.HasValue)
                transactions = transactions.Where(t => t.EquipmentId == equipmentFilter.Value);

            if (!string.IsNullOrEmpty(paymentStatusFilter))
                transactions = transactions.Where(t => t.PaymentStatus == paymentStatusFilter);

            if (searchId.HasValue)
                transactions = transactions.Where(t => t.RentalTransactionId == searchId.Value);

            var totalItems = await transactions.CountAsync();
            var items = await transactions.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.EquipmentFilter = new SelectList(await _context.Equipments.ToListAsync(), "EquipmentId", "Name", equipmentFilter);
            ViewBag.PaymentStatusFilter = new SelectList(new List<string> { "Paid", "Partially Paid", "Not yet" }, paymentStatusFilter);
            ViewBag.IsAdminOrManager = isAdminOrManager;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return View(items);
        }




        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var rentalTransaction = await _context.RentalTransactions
                .Include(r => r.Equipment)
                .Include(r => r.RentalRequest)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.RentalTransactionId == id);

            if (rentalTransaction == null) return NotFound();

            return View(rentalTransaction);
        }

        public async Task<IActionResult> HandlePayment(int rentalRequestId)
        {
            var existingTransaction = await _context.RentalTransactions
                .FirstOrDefaultAsync(t => t.RentalRequestId == rentalRequestId);

            if (existingTransaction != null)
            {
                return RedirectToAction("Edit", new { id = existingTransaction.RentalTransactionId });
            }
            else
            {
                return RedirectToAction("Create", new { rentalRequestId = rentalRequestId });
            }
        }

        public async Task<IActionResult> Create(int? rentalRequestId)
        {
            var (mainUser, isAdminOrManager, isCustomer) = await GetCurrentUser();

            if (!isAdminOrManager)
                return Forbid();

            if (rentalRequestId == null) return NotFound();

            var rentalRequest = await _context.RentalRequests
                .Include(r => r.Equipment)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.RentalRequestId == rentalRequestId);

            if (rentalRequest == null) return NotFound();

            var transaction = new RentalTransaction
            {
                RentalRequestId = rentalRequest.RentalRequestId,
                EquipmentId = rentalRequest.EquipmentId,
                UserId = rentalRequest.UserId,
                RentalTransactionStartDate = rentalRequest.StartDate,
                RentalTransactionReturnDate = rentalRequest.ReturnDate,
                RentalFee = rentalRequest.TotalCost,
                RentalPeriod = (rentalRequest.ReturnDate - rentalRequest.StartDate).Days,
                CreatedAt = DateTime.Now,
                PaymentStatus = "Not yet",
                User = rentalRequest.User,
                Equipment = rentalRequest.Equipment
            };

            return View(transaction);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RentalTransaction rentalTransaction, IFormFile DocumentFile)
        {
            ModelState.Remove("RentalRequest");
            ModelState.Remove("User");
            ModelState.Remove("Equipment");
            ModelState.Remove("PaymentStatus");
            ModelState.Remove("DocumentFile");

            var rentalRequest = await _context.RentalRequests
                .Include(r => r.Equipment)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.RentalRequestId == rentalTransaction.RentalRequestId);

            if (rentalRequest == null) return NotFound();

            if (rentalRequest.Status != "Approved")
            {
                ModelState.AddModelError("", "Transaction cannot be created unless the Rental Request is approved.");
                rentalTransaction.User = rentalRequest.User;
                rentalTransaction.Equipment = rentalRequest.Equipment;
                return View(rentalTransaction);
            }

            if (rentalTransaction.Deposit > rentalTransaction.RentalFee)
            {
                ModelState.AddModelError("Deposit", "Deposit cannot exceed Rental Fee.");
                rentalTransaction.User = rentalRequest.User;
                rentalTransaction.Equipment = rentalRequest.Equipment;
                return View(rentalTransaction);
            }




            double totalDue = rentalTransaction.RentalFee;
            var returnInfo = await _context.ReturnInfos.FirstOrDefaultAsync(r => r.TransactionId == rentalTransaction.RentalTransactionId);
            if (returnInfo != null)
                totalDue += (returnInfo.LateReturnFees ?? 0) + (returnInfo.AdditionalCharges ?? 0);

            double paid = rentalTransaction.Deposit + (returnInfo?.PaidLateFees ?? 0);
            if (paid == 0)
                rentalTransaction.PaymentStatus = "Not yet";
            else if (paid < totalDue)
                rentalTransaction.PaymentStatus = "Partially Paid";
            else
                rentalTransaction.PaymentStatus = "Paid";


            rentalTransaction.CreatedAt = DateTime.Now;

            if (DocumentFile != null && DocumentFile.Length > 0)
            {
                var uploadsPath = Path.Combine(_webHostEnvironment.WebRootPath, "documents");
                Directory.CreateDirectory(uploadsPath);
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(DocumentFile.FileName);
                var filePath = Path.Combine(uploadsPath, fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await DocumentFile.CopyToAsync(stream);
                rentalTransaction.DocumentPath = fileName;

                // Save document to Document table
                var documentEntry = new Document
                {
                    UserId = rentalTransaction.UserId,
                    FileName = fileName,
                    FileType = GetMimeTypeFromExtension(Path.GetExtension(DocumentFile.FileName)),

                    UploadDate = DateTime.Now
                };

                using (var memoryStream = new MemoryStream())
                {
                    await DocumentFile.CopyToAsync(memoryStream);
                    documentEntry.FileUpload = memoryStream.ToArray();
                }

                _context.Documents.Add(documentEntry);

            }

            _context.Add(rentalTransaction);

            rentalRequest.TotalCost = rentalTransaction.RentalFee;
            rentalRequest.StartDate = rentalTransaction.RentalTransactionStartDate;
            rentalRequest.ReturnDate = rentalTransaction.RentalTransactionReturnDate;

            await _context.SaveChangesAsync();

            var notification = new Notification
            {
                Type = "Rental Transaction Created",
                Message = "A new rental transaction has been created for your rental request.",
                Status = false
            };
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            _context.NotificationUsers.Add(new NotificationUser
            {
                NotificationId = notification.NotificationId,
                UserId = rentalRequest.UserId
            });
            await _context.SaveChangesAsync();

            TempData["CreateMessage"] = "Rental Transaction created successfully!";
            return RedirectToAction(nameof(Index));
        }



        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var transaction = await _context.RentalTransactions
                .Include(r => r.Equipment)
                .Include(r => r.User)
                .Include(r => r.RentalRequest)
                .FirstOrDefaultAsync(r => r.RentalTransactionId == id);

            if (transaction == null) return NotFound();

            var (mainUser, isAdminOrManager, isCustomer) = await GetCurrentUser();
            if (isCustomer) return Forbid();

            return View(transaction);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RentalTransaction rentalTransaction, IFormFile DocumentFile, string ExistingDocumentPath, bool RemoveDocument)

        {
            ModelState.Remove("RentalRequest");
            ModelState.Remove("User");
            ModelState.Remove("Equipment");
            ModelState.Remove("PaymentStatus");
            ModelState.Remove("DocumentFile");
            ModelState.Remove("ExistingDocumentPath");


            if (id != rentalTransaction.RentalTransactionId) return NotFound();

            var rentalRequest = await _context.RentalRequests
                .Include(r => r.Equipment)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.RentalRequestId == rentalTransaction.RentalRequestId);

            if (rentalRequest == null) return NotFound();



            int days = (rentalTransaction.RentalTransactionReturnDate - rentalTransaction.RentalTransactionStartDate).Days;
            rentalTransaction.RentalPeriod = days > 0 ? days : 0;
            rentalTransaction.RentalFee = rentalTransaction.RentalPeriod * rentalRequest.Equipment.RentalPrice;


            if (rentalTransaction.Deposit > rentalTransaction.RentalFee)
            {
                ModelState.AddModelError("Deposit", "Deposit cannot exceed Rental Fee.");
                rentalTransaction.User = rentalRequest.User;
                rentalTransaction.Equipment = rentalRequest.Equipment;
                return View(rentalTransaction);
            }


            double totalDue = rentalTransaction.RentalFee;
            var returnInfo = await _context.ReturnInfos.FirstOrDefaultAsync(r => r.TransactionId == rentalTransaction.RentalTransactionId);
            if (returnInfo != null)
                totalDue += (returnInfo.LateReturnFees ?? 0) + (returnInfo.AdditionalCharges ?? 0);

            double paid = rentalTransaction.Deposit + (returnInfo?.PaidLateFees ?? 0);
            if (paid == 0)
                rentalTransaction.PaymentStatus = "Not yet";
            else if (paid < totalDue)
                rentalTransaction.PaymentStatus = "Partially Paid";
            else
                rentalTransaction.PaymentStatus = "Paid";


            if (rentalTransaction.CreatedAt < new DateTime(1753, 1, 1))
                rentalTransaction.CreatedAt = DateTime.Now;

            // Handle document logic
            if (RemoveDocument)
            {
                // Delete the file
                if (!string.IsNullOrEmpty(ExistingDocumentPath))
                {
                    var oldPath = Path.Combine(_webHostEnvironment.WebRootPath, "documents", ExistingDocumentPath);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);

                    // Delete document from the database
                    var oldDoc = await _context.Documents.FirstOrDefaultAsync(d => d.FileName == ExistingDocumentPath);
                    if (oldDoc != null)
                    {
                        _context.Documents.Remove(oldDoc);
                    }
                }

                rentalTransaction.DocumentPath = null;
            }

            else if (DocumentFile != null && DocumentFile.Length > 0)
            {
                // Replace with new file
                if (!string.IsNullOrEmpty(ExistingDocumentPath))
                {
                    var oldPath = Path.Combine(_webHostEnvironment.WebRootPath, "documents", ExistingDocumentPath);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                var uploadsPath = Path.Combine(_webHostEnvironment.WebRootPath, "documents");
                Directory.CreateDirectory(uploadsPath);
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(DocumentFile.FileName);
                var filePath = Path.Combine(uploadsPath, fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await DocumentFile.CopyToAsync(stream);
                rentalTransaction.DocumentPath = fileName;


                // Delete old document record if exists
                var oldDoc = await _context.Documents.FirstOrDefaultAsync(d => d.FileName == ExistingDocumentPath);
                if (oldDoc != null)
                {
                    _context.Documents.Remove(oldDoc);
                }


                // Save document to Document table
                var documentEntry = new Document
                {
                    UserId = rentalTransaction.UserId,
                    FileName = fileName,
                    FileType = GetMimeTypeFromExtension(Path.GetExtension(DocumentFile.FileName)),

                    UploadDate = DateTime.Now
                };

                using (var memoryStream = new MemoryStream())
                {
                    await DocumentFile.CopyToAsync(memoryStream);
                    documentEntry.FileUpload = memoryStream.ToArray();
                }

                _context.Documents.Add(documentEntry);

            }
            else
            {
                rentalTransaction.DocumentPath = ExistingDocumentPath;
            }

            rentalTransaction.User = rentalRequest.User;
            rentalTransaction.Equipment = rentalRequest.Equipment;

            if (ModelState.IsValid)
            {
                try
                {
                    rentalRequest.TotalCost = rentalTransaction.RentalFee;
                    rentalRequest.StartDate = rentalTransaction.RentalTransactionStartDate;
                    rentalRequest.ReturnDate = rentalTransaction.RentalTransactionReturnDate;
                    _context.Update(rentalRequest);

                    _context.Update(rentalTransaction);
                    await _context.SaveChangesAsync();

                    var notification = new Notification
                    {
                        Type = "Rental Transaction Updated",
                        Message = "Your rental transaction has been updated.",
                        Status = false
                    };
                    _context.Notifications.Add(notification);
                    await _context.SaveChangesAsync();

                    _context.NotificationUsers.Add(new NotificationUser
                    {
                        NotificationId = notification.NotificationId,
                        UserId = rentalRequest.UserId
                    });
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.RentalTransactions.Any(e => e.RentalTransactionId == id))
                        return NotFound();
                    else throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(rentalTransaction);
        }



        [HttpGet("/api/rental-request/{id}/price")]
        public async Task<IActionResult> GetRentalPrice(int id, DateTime startDate, DateTime returnDate)
        {
            var request = await _context.RentalRequests
                .Include(r => r.Equipment)
                .FirstOrDefaultAsync(r => r.RentalRequestId == id);

            if (request == null || request.Equipment == null)
                return NotFound();

            int period = (returnDate - startDate).Days;
            double fee = (period > 0 ? period : 0) * request.Equipment.RentalPrice;

            return Json(new { period, fee });
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var rentalTransaction = await _context.RentalTransactions
                .Include(r => r.Equipment)
                .Include(r => r.RentalRequest)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.RentalTransactionId == id);

            var (mainUser, isAdminOrManager, isCustomer) = await GetCurrentUser();

            if (isCustomer) return Forbid();

            if (rentalTransaction == null) return NotFound();

            return View(rentalTransaction);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var (mainUser, isAdminOrManager, isCustomer) = await GetCurrentUser();

            if (isCustomer) return Forbid();

            var rentalTransaction = await _context.RentalTransactions
                .Include(t => t.ReturnInfos)
                .Include(t => t.RentalRequest)
                    .ThenInclude(r => r.User)
                .Include(t => t.Equipment)
                .FirstOrDefaultAsync(t => t.RentalTransactionId == id);

            if (rentalTransaction == null) return NotFound();

            // Define restorable return conditions
            var restorableConditions = new[] { "On Time in Good Condition", "Late But Good Condition" };

            foreach (var returnInfo in rentalTransaction.ReturnInfos.ToList())
            {
                bool wasRestored = returnInfo.StockRestored ?? false;
                bool isRestorable = restorableConditions.Contains(returnInfo.ReturnCondition?.Trim(), StringComparer.OrdinalIgnoreCase);

                if (wasRestored && isRestorable)
                {
                    var equipment = rentalTransaction.Equipment;
                    if (equipment != null)
                    {
                        equipment.Quantity = Math.Max(0, equipment.Quantity - 1);
                        equipment.AvailabilityStatus = equipment.Quantity > 0;
                        _context.Equipments.Update(equipment);
                    }
                }

                _context.ReturnInfos.Remove(returnInfo);
            }

            // Delete document from wwwroot/documents and from Document table
            if (!string.IsNullOrEmpty(rentalTransaction.DocumentPath))
            {
                var path = Path.Combine(_webHostEnvironment.WebRootPath, "documents", rentalTransaction.DocumentPath);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);

                var document = await _context.Documents
                    .FirstOrDefaultAsync(d => d.FileName == rentalTransaction.DocumentPath);

                if (document != null)
                    _context.Documents.Remove(document);
            }

            _context.RentalTransactions.Remove(rentalTransaction);
            await _context.SaveChangesAsync();

            var notification = new Notification
            {
                Type = "Rental Transaction Deleted",
                Message = "Your rental transaction has been deleted by the admin.",
                Status = false
            };
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            _context.NotificationUsers.Add(new NotificationUser
            {
                NotificationId = notification.NotificationId,
                UserId = rentalTransaction.RentalRequest.UserId
            });
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



        private bool RentalTransactionExists(int id)
        {
            return _context.RentalTransactions.Any(e => e.RentalTransactionId == id);
        }

        public async Task<IActionResult> DownloadDocument(int id)
        {
            var transaction = await _context.RentalTransactions.FindAsync(id);
            if (transaction == null || string.IsNullOrEmpty(transaction.DocumentPath))
                return NotFound();

            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "documents", transaction.DocumentPath);

            // First try from wwwroot
            if (System.IO.File.Exists(filePath))
            {
                var contentType = "application/octet-stream";
                return File(System.IO.File.ReadAllBytes(filePath), contentType, Path.GetFileName(filePath));
            }

            // If not found in wwwroot, try fetching from the database
            var doc = await _context.Documents
                .FirstOrDefaultAsync(d => d.FileName == transaction.DocumentPath && d.UserId == transaction.UserId);

            if (doc != null && doc.FileUpload != null)
            {
                var contentType = GetMimeTypeFromExtension(Path.GetExtension(doc.FileName));

                return File(doc.FileUpload, contentType, doc.FileName);
            }

            return NotFound("Document not found in filesystem or database.");
        }


        private string GetMimeTypeFromExtension(string extension)
        {
            return extension.ToLower() switch
            {
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".txt" => "text/plain",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream",
            };
        }







        // Updated HandleExtraFee GET
        public async Task<IActionResult> HandleExtraFee(int id)
        {
            var transaction = await _context.RentalTransactions
                .Include(t => t.ReturnInfos)
                .Include(t => t.RentalRequest)
                .Include(t => t.User)
                .Include(t => t.Equipment)
                .FirstOrDefaultAsync(t => t.RentalTransactionId == id);

            if (transaction == null) return NotFound();

            var identityUser = await _userManager.GetUserAsync(User);
            var mainUser = identityUser?.MainUserId != null
                ? await _context.Users.FindAsync(identityUser.MainUserId)
                : null;

            if (mainUser == null || (mainUser.RoleId != 1 && mainUser.RoleId != 3))
                return Forbid();

            var returnInfo = transaction.ReturnInfos.FirstOrDefault();
            if (returnInfo == null || ((returnInfo.LateReturnFees ?? 0) + (returnInfo.AdditionalCharges ?? 0)) == 0)
                return RedirectToAction("Index");

            var expectedReturn = transaction.RentalTransactionReturnDate.Date;
            var actualReturn = returnInfo.ReturnDate.Date;
            var lateDays = Math.Max(0, (int)(actualReturn - expectedReturn).TotalDays);

            double perDayLateFee = returnInfo.LateReturnFees ?? 0;
            double extraCharges = returnInfo.AdditionalCharges ?? 0;
            double totalLateFee = (perDayLateFee * lateDays) + extraCharges;
            double alreadyPaid = returnInfo.PaidLateFees ?? 0;
            double remainingDue = Math.Max(0, totalLateFee - alreadyPaid);

            // Set all needed ViewBags
            ViewBag.LateFeePerDay = perDayLateFee;
            ViewBag.LateDays = lateDays;
            ViewBag.TotalLateFee = totalLateFee;
            ViewBag.PaidLateFees = alreadyPaid; 
            ViewBag.ExtraCharges = extraCharges;
            ViewBag.TotalExtraDue = remainingDue;

            return View(transaction);
        }



        // Updated HandleExtraFee POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HandleExtraFee(int id, double extraPaid)
        {
            var transaction = await _context.RentalTransactions
                .Include(t => t.ReturnInfos)
                .Include(t => t.RentalRequest)
                .Include(t => t.User)
                .Include(t => t.Equipment)
                .FirstOrDefaultAsync(t => t.RentalTransactionId == id);

            if (transaction == null) return NotFound();

            var identityUser = await _userManager.GetUserAsync(User);
            var mainUser = identityUser?.MainUserId != null
                ? await _context.Users.FindAsync(identityUser.MainUserId)
                : null;

            if (mainUser == null || (mainUser.RoleId != 1 && mainUser.RoleId != 3))
                return Forbid();

            var returnInfo = transaction.ReturnInfos.FirstOrDefault();
            if (returnInfo == null) return RedirectToAction("Index");

            var expectedReturn = transaction.RentalTransactionReturnDate.Date;
            var actualReturn = returnInfo.ReturnDate.Date;
            var lateDays = Math.Max(0, (int)(actualReturn - expectedReturn).TotalDays);

            double perDayFee = returnInfo.LateReturnFees ?? 0;
            double extraCharges = returnInfo.AdditionalCharges ?? 0;
            double totalLateFee = (perDayFee * lateDays) + extraCharges;

            double alreadyPaidLate = returnInfo.PaidLateFees ?? 0;
            double remainingDue = Math.Max(0, totalLateFee - alreadyPaidLate);

            if (extraPaid > remainingDue)
            {
                ViewBag.TotalExtraDue = remainingDue;
                ViewBag.LateFeePerDay = perDayFee;
                ViewBag.LateDays = lateDays;
                ViewBag.TotalLateFee = totalLateFee;
                ModelState.AddModelError("", $"Amount exceeds remaining balance. Only {remainingDue:C} is due.");
                return View(transaction);
            }

            returnInfo.PaidLateFees = alreadyPaidLate + extraPaid;

            double totalPaid = (transaction.Deposit) + returnInfo.PaidLateFees.Value;
            double fullRequired = transaction.RentalFee + totalLateFee;

            transaction.PaymentStatus = totalPaid == 0
                ? "Not yet"
                : totalPaid < fullRequired ? "Partially Paid" : "Paid";

            await _context.SaveChangesAsync();
            TempData["ExtraFeePaid"] = $"Extra payment of {extraPaid:C} recorded.";
            return RedirectToAction("Index");
        }



        ///view late fee payment statement 
        
        public async Task<IActionResult> ViewLateFeeStatement(int id)
        {
            var transaction = await _context.RentalTransactions
                .Include(t => t.ReturnInfos)
                .Include(t => t.RentalRequest)
                .Include(t => t.User)
                .Include(t => t.Equipment)
                .FirstOrDefaultAsync(t => t.RentalTransactionId == id);

            if (transaction == null) return NotFound();

            var (mainUser, isAdminOrManager, isCustomer) = await GetCurrentUser();

            if (isCustomer && transaction.UserId != mainUser.UserId)
                return Forbid(); // Block customers from viewing other users' transactions


            var returnInfo = transaction.ReturnInfos.FirstOrDefault();
            if (returnInfo == null) return RedirectToAction("Index");

            var expectedReturn = transaction.RentalTransactionReturnDate.Date;
            var actualReturn = returnInfo.ReturnDate.Date;
            var lateDays = Math.Max(0, (int)(actualReturn - expectedReturn).TotalDays);

            double perDayLateFee = returnInfo.LateReturnFees ?? 0;
            double extraCharges = returnInfo.AdditionalCharges ?? 0;
            double totalLateFee = (perDayLateFee * lateDays) + extraCharges;
            double paid = returnInfo.PaidLateFees ?? 0;

            ViewBag.LateFeePerDay = perDayLateFee;
            ViewBag.LateDays = lateDays;
            ViewBag.TotalLateFee = totalLateFee;
            ViewBag.PaidLateFees = paid;
            ViewBag.ExtraCharges = extraCharges;
            ViewBag.IsStatement = true;

            return View("HandleExtraFee", transaction); // reuse existing view
        }





    }

}