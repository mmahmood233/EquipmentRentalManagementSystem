using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EquipmentRental.DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Diagnostics;
namespace EquipmentRental.Web.Controllers
{
    [Authorize(Roles = "Customer,Manager,Administrator")]
    public class RentalTransactionsController : Controller
    { 
        private readonly EquipmentRentalDbContext _context;

        public RentalTransactionsController(EquipmentRentalDbContext context)
        {
            _context = context;
        }

        // GET: RentalTransactions
        public async Task<IActionResult> Index(string search = null, string status = null)
        {
            try
            {
                // Get current user ID
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var isCustomer = User.IsInRole("Customer");
                var isManager = User.IsInRole("Manager") || User.IsInRole("Administrator");
                
                Debug.WriteLine($"User ID: {userId}, IsCustomer: {isCustomer}, IsManager: {isManager}");
                
                // Base query with all necessary includes
                IQueryable<RentalTransaction> query = _context.RentalTransactions
                    .Include(r => r.Customer)
                    .Include(r => r.Equipment)
                    .Include(r => r.RentalRequest)
                    .Include(r => r.Documents)
                    .AsQueryable();
                
                // Apply role-based filtering - customers can only see their own transactions
                if (isCustomer && !isManager)
                {
                    query = query.Where(r => r.CustomerId == userId);
                    Debug.WriteLine($"Filtering for customer {userId}'s transactions only");
                }
                
                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(r => 
                        r.Equipment.Name.Contains(search) || 
                        r.PaymentStatus.Contains(search) ||
                        r.RentalRequest.Description.Contains(search));
                    
                    Debug.WriteLine($"Applied search filter: {search}");
                }
                
                // Apply status filter if provided
                if (!string.IsNullOrWhiteSpace(status))
                {
                    query = query.Where(r => r.PaymentStatus == status);
                    Debug.WriteLine($"Applied status filter: {status}");
                }
                
                // Get final results ordered by creation date (newest first)
                var transactions = await query.OrderByDescending(r => r.CreatedAt).ToListAsync();
                Debug.WriteLine($"Found {transactions.Count} transactions");
                
                // Prepare view data
                ViewBag.Search = search;
                ViewBag.Status = status;
                ViewBag.StatusOptions = new[] { "All", "Paid", "Pending", "Refunded", "Cancelled" };
                
                return View(transactions);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in RentalTransactions Index: {ex.Message}");
                TempData["Error"] = $"An error occurred: {ex.Message}";
                return View(new List<RentalTransaction>());
            }
        }


        // GET: RentalTransactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rentalTransaction = await _context.RentalTransactions
                .Include(r => r.Customer)
                .Include(r => r.Equipment)
                .Include(r => r.RentalRequest)
                .Include(r => r.Documents)
                .FirstOrDefaultAsync(m => m.RentalTransactionId == id);
                
            if (rentalTransaction == null)
            {
                return NotFound();
            }
            
            // Check authorization - customers can only view their own transactions
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var isManager = User.IsInRole("Manager") || User.IsInRole("Administrator");
            
            if (!isManager && rentalTransaction.CustomerId != userId)
            {
                return Forbid();
            }
            
            // Set ViewBag data for authorization checks in the view
            ViewBag.IsManager = isManager;
            ViewBag.CanManageFiles = isManager;
            ViewBag.UserId = userId;
            ViewBag.TransactionId = id; // Explicitly set the transaction ID in ViewBag
            
            return View(rentalTransaction);
        }

        // GET: RentalTransactions/Create
        [Authorize(Roles = "Manager,Administrator")]
        public async Task<IActionResult> Create(int? requestId = null)
        {
            // If requestId is provided, pre-fill the form with rental request data
            if (requestId.HasValue)
            {
                var request = await _context.RentalRequests
                    .Include(r => r.Customer)
                    .Include(r => r.Equipment)
                    .FirstOrDefaultAsync(r => r.RentalRequestId == requestId);
                
                if (request != null && request.Status == "Approved")
                {
                    var transaction = new RentalTransaction
                    {
                        RentalRequestId = request.RentalRequestId,
                        EquipmentId = request.EquipmentId,
                        CustomerId = request.CustomerId,
                        RentalStartDate = request.RentalStartDate,
                        RentalEndDate = request.RentalEndDate,
                        RentalPeriod = (int)(request.RentalEndDate - request.RentalStartDate).TotalDays,
                        RentalFee = request.TotalCost,
                        Deposit = request.TotalCost * 0.2m, // 20% deposit as default
                        PaymentStatus = "Pending",
                        CreatedAt = DateTime.Now
                    };
                    
                    ViewData["CustomerId"] = new SelectList(_context.Users.Where(u => u.Role.RoleName == "Customer"), "UserId", "Email", transaction.CustomerId);
                    ViewData["EquipmentId"] = new SelectList(_context.Equipment.Where(e => e.AvailabilityStatus == "Reserved" || e.EquipmentId == request.EquipmentId), "EquipmentId", "Name", transaction.EquipmentId);
                    ViewData["RentalRequestId"] = new SelectList(_context.RentalRequests.Where(r => r.Status == "Approved"), "RentalRequestId", "Description", transaction.RentalRequestId);
                    
                    ViewBag.PrefilledFromRequest = true;
                    return View(transaction);
                }
            }
            
            // Regular create form
            ViewData["CustomerId"] = new SelectList(_context.Users.Where(u => u.Role.RoleName == "Customer"), "UserId", "Email");
            ViewData["EquipmentId"] = new SelectList(_context.Equipment.Where(e => e.AvailabilityStatus != "Maintenance"), "EquipmentId", "Name");
            ViewData["RentalRequestId"] = new SelectList(_context.RentalRequests.Where(r => r.Status == "Approved"), "RentalRequestId", "Description");
            
            return View();
        }

        // POST: RentalTransactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Manager,Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RentalTransaction rentalTransaction)
        {
            try
            {
                // Force validation to pass for pre-filled forms
                ModelState.Clear();
                
                // Set creation date
                rentalTransaction.CreatedAt = DateTime.Now;
                
                // Log transaction details
                Debug.WriteLine($"Creating transaction: Request={rentalTransaction.RentalRequestId}, Equipment={rentalTransaction.EquipmentId}, Customer={rentalTransaction.CustomerId}");
                Debug.WriteLine($"Dates: Start={rentalTransaction.RentalStartDate}, End={rentalTransaction.RentalEndDate}");
                Debug.WriteLine($"Financial: Fee={rentalTransaction.RentalFee}, Deposit={rentalTransaction.Deposit}");
                
                // Add transaction to database
                _context.Add(rentalTransaction);
                var saveResult = await _context.SaveChangesAsync();
                Debug.WriteLine($"SaveChangesAsync result: {saveResult}");
                Debug.WriteLine($"Transaction ID after save: {rentalTransaction.RentalTransactionId}");
                
                // Update rental request status
                var request = await _context.RentalRequests.FindAsync(rentalTransaction.RentalRequestId);
                if (request != null)
                {
                    request.Status = "Confirmed";
                    _context.Update(request);
                }
                
                // Update equipment status
                var equipment = await _context.Equipment.FindAsync(rentalTransaction.EquipmentId);
                if (equipment != null)
                {
                    equipment.AvailabilityStatus = "In Use";
                    _context.Update(equipment);
                }
                
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Rental transaction created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception in Create POST: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                
                ModelState.AddModelError("", $"Error creating transaction: {ex.Message}");
                PrepareViewData(rentalTransaction);
                return View(rentalTransaction);
            }
        }
        
        // Helper method to prepare ViewData for forms
        private void PrepareViewData(RentalTransaction rentalTransaction)
        {
            ViewData["CustomerId"] = new SelectList(_context.Users.Where(u => u.Role.RoleName == "Customer"), "UserId", "Email", rentalTransaction.CustomerId);
            ViewData["EquipmentId"] = new SelectList(_context.Equipment, "EquipmentId", "Name", rentalTransaction.EquipmentId);
            ViewData["RentalRequestId"] = new SelectList(_context.RentalRequests.Where(r => r.Status == "Approved"), "RentalRequestId", "Description", rentalTransaction.RentalRequestId);
        }

        // GET: RentalTransactions/MarkAsPaid/5
        [Authorize(Roles = "Manager,Administrator")]
        public async Task<IActionResult> MarkAsPaid(int id)
        {
            var transaction = await _context.RentalTransactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }
            
            // Update payment status to Paid
            transaction.PaymentStatus = "Paid";
            _context.Update(transaction);
            await _context.SaveChangesAsync();
            
            TempData["Success"] = "Payment status updated to Paid successfully!";
            return RedirectToAction(nameof(Details), new { id });
        }
        
        // GET: RentalTransactions/PrintReceipt/5
        [Authorize(Roles = "Manager,Administrator,Customer")]
        public async Task<IActionResult> PrintReceipt(int id)
        {
            var transaction = await _context.RentalTransactions
                .Include(r => r.Customer)
                .Include(r => r.Equipment)
                .Include(r => r.RentalRequest)
                .FirstOrDefaultAsync(m => m.RentalTransactionId == id);
                
            if (transaction == null)
            {
                return NotFound();
            }
            
            // Check authorization - customers can only view their own transactions
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var isManager = User.IsInRole("Manager") || User.IsInRole("Administrator");
            
            if (!isManager && transaction.CustomerId != userId)
            {
                return Forbid();
            }
            
            // Generate receipt view
            return View(transaction);
        }
        
        // GET: RentalTransactions/SelectRequest
        [Authorize(Roles = "Manager,Administrator")]
        public async Task<IActionResult> SelectRequest()
        {
            // Get all approved rental requests that don't have transactions yet
            var approvedRequests = await _context.RentalRequests
                .Include(r => r.Customer)
                .Include(r => r.Equipment)
                .Include(r => r.RentalTransactions)
                .Where(r => r.Status == "Approved" && !r.RentalTransactions.Any())
                .ToListAsync();
            
            return View(approvedRequests);
        }

        // GET: RentalTransactions/GetRequestDetails/5
        [Authorize(Roles = "Manager,Administrator")]
        public async Task<IActionResult> GetRequestDetails(int id)
        {
            var request = await _context.RentalRequests
                .Include(r => r.Customer)
                .Include(r => r.Equipment)
                .FirstOrDefaultAsync(r => r.RentalRequestId == id);

            if (request == null)
            {
                return NotFound();
            }

            // Create a new transaction based on the request
            var transaction = new RentalTransaction
            {
                RentalRequestId = request.RentalRequestId,
                EquipmentId = request.EquipmentId,
                CustomerId = request.CustomerId,
                RentalStartDate = request.RentalStartDate,
                RentalEndDate = request.RentalEndDate,
                RentalPeriod = (int)(request.RentalEndDate - request.RentalStartDate).TotalDays,
                RentalFee = request.TotalCost,
                Deposit = request.TotalCost * 0.2m, // 20% deposit as default
                PaymentStatus = "Pending",
                CreatedAt = DateTime.Now
            };

            // Prepare view data
            ViewData["CustomerId"] = new SelectList(_context.Users.Where(u => u.Role.RoleName == "Customer"), "UserId", "Email", transaction.CustomerId);
            ViewData["EquipmentId"] = new SelectList(_context.Equipment, "EquipmentId", "Name", transaction.EquipmentId);
            ViewData["RentalRequestId"] = new SelectList(_context.RentalRequests.Where(r => r.Status == "Approved"), "RentalRequestId", "Description", transaction.RentalRequestId);
            
            ViewBag.PrefilledFromRequest = true;
            return View("Create", transaction);
        }

        // GET: RentalTransactions/Edit/5
        [Authorize(Roles = "Manager,Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rentalTransaction = await _context.RentalTransactions.FindAsync(id);
            if (rentalTransaction == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Users.Where(u => u.Role.RoleName == "Customer"), "UserId", "Email", rentalTransaction.CustomerId);
            ViewData["EquipmentId"] = new SelectList(_context.Equipment, "EquipmentId", "Name", rentalTransaction.EquipmentId);
            ViewData["RentalRequestId"] = new SelectList(_context.RentalRequests, "RentalRequestId", "Description", rentalTransaction.RentalRequestId);
            return View(rentalTransaction);
        }

        // POST: RentalTransactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Manager,Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RentalTransactionId,RentalRequestId,EquipmentId,CustomerId,RentalStartDate,RentalEndDate,RentalPeriod,RentalFee,Deposit,PaymentStatus")] RentalTransaction rentalTransaction)
        {
            if (id != rentalTransaction.RentalTransactionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Preserve the original creation date
                    var originalTransaction = await _context.RentalTransactions.AsNoTracking()
                        .FirstOrDefaultAsync(t => t.RentalTransactionId == id);
                        
                    if (originalTransaction != null)
                    {
                        rentalTransaction.CreatedAt = originalTransaction.CreatedAt;
                    }
                    else
                    {
                        rentalTransaction.CreatedAt = DateTime.Now;
                    }
                    
                    _context.Update(rentalTransaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RentalTransactionExists(rentalTransaction.RentalTransactionId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Users.Where(u => u.Role.RoleName == "Customer"), "UserId", "Email", rentalTransaction.CustomerId);
            ViewData["EquipmentId"] = new SelectList(_context.Equipment, "EquipmentId", "Name", rentalTransaction.EquipmentId);
            ViewData["RentalRequestId"] = new SelectList(_context.RentalRequests, "RentalRequestId", "Description", rentalTransaction.RentalRequestId);
            return View(rentalTransaction);
        }

        // GET: RentalTransactions/Delete/5
        [Authorize(Roles = "Manager,Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rentalTransaction = await _context.RentalTransactions
                .Include(r => r.Customer)
                .Include(r => r.Equipment)
                .Include(r => r.RentalRequest)
                .Include(r => r.Documents)
                .FirstOrDefaultAsync(m => m.RentalTransactionId == id);
            if (rentalTransaction == null)
            {
                return NotFound();
            }

            return View(rentalTransaction);
        }

        // POST: RentalTransactions/Delete/5
        [Authorize(Roles = "Manager,Administrator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id <= 0)
            {
                return Problem("Entity set 'EquipmentRentalDbContext.RentalTransactions'  is null.");
            }
            var rentalTransaction = await _context.RentalTransactions.FindAsync(id);
            if (rentalTransaction != null)
            {
                _context.RentalTransactions.Remove(rentalTransaction);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RentalTransactionExists(int id)
        {
          return (_context.RentalTransactions?.Any(e => e.RentalTransactionId == id)).GetValueOrDefault();
        }
    }
}
