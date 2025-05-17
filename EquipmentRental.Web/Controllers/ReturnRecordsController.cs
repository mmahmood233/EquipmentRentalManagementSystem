using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EquipmentRental.DataAccess.Models;
using Microsoft.AspNetCore.Authorization;

namespace EquipmentRental.Web.Controllers
{
    [Authorize(Roles = "Customer,Manager,Administrator")]
    public class ReturnRecordsController : Controller

    {
        private readonly EquipmentRentalDbContext _context;
        private readonly NotificationService _notificationService;

        public ReturnRecordsController(EquipmentRentalDbContext context, NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }


        // GET: ReturnRecords
        public async Task<IActionResult> Index(string search = null, string condition = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            // Get current user ID
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            var isCustomer = User.IsInRole("Customer");
            var isManager = User.IsInRole("Manager") || User.IsInRole("Administrator");
            
            System.Diagnostics.Debug.WriteLine($"User ID: {userId}, IsCustomer: {isCustomer}, IsManager: {isManager}");
            
            // Base query with all necessary includes
            var query = _context.ReturnRecords
                .Include(r => r.RentalTransaction)
                .ThenInclude(rt => rt.Customer)
                .Include(r => r.RentalTransaction.Equipment)
                .AsQueryable();
            
            // Apply role-based filtering - customers can only see their own return records
            if (isCustomer && !isManager)
            {
                query = query.Where(r => r.RentalTransaction.CustomerId == userId);
                System.Diagnostics.Debug.WriteLine($"Filtering for customer {userId}'s return records only");
            }
            
            // Apply search filter if provided
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(r => 
                    r.RentalTransaction.Equipment.Name.Contains(search) || 
                    r.Notes.Contains(search) ||
                    r.ReturnCondition.Contains(search));
                
                System.Diagnostics.Debug.WriteLine($"Applied search filter: {search}");
            }
            
            // Apply condition filter if provided
            if (!string.IsNullOrWhiteSpace(condition))
            {
                query = query.Where(r => r.ReturnCondition == condition);
                System.Diagnostics.Debug.WriteLine($"Applied condition filter: {condition}");
            }
            
            // Apply date range filters if provided
            if (fromDate.HasValue)
            {
                query = query.Where(r => r.ActualReturnDate >= fromDate.Value);
                System.Diagnostics.Debug.WriteLine($"Applied from date filter: {fromDate.Value}");
            }
            
            if (toDate.HasValue)
            {
                query = query.Where(r => r.ActualReturnDate <= toDate.Value);
                System.Diagnostics.Debug.WriteLine($"Applied to date filter: {toDate.Value}");
            }
            
            // Get final results ordered by return date (newest first)
            var returnRecords = await query.OrderByDescending(r => r.ActualReturnDate).ToListAsync();
            
            // Prepare ViewBag data for filters
            ViewBag.Conditions = await _context.ReturnRecords
                .Select(r => r.ReturnCondition)
                .Distinct()
                .ToListAsync();
                
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentCondition = condition;
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");
            
            return View(returnRecords);
        }


        // GET: ReturnRecords/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var returnRecord = await _context.ReturnRecords
                .Include(r => r.RentalTransaction)
                .ThenInclude(rt => rt.Customer)
                .Include(r => r.RentalTransaction.Equipment)
                .FirstOrDefaultAsync(m => m.ReturnRecordId == id);
                
            if (returnRecord == null)
            {
                return NotFound();
            }
            
            // Check authorization - customers can only view their own return records
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            var isManager = User.IsInRole("Manager") || User.IsInRole("Administrator");
            
            if (!isManager && returnRecord.RentalTransaction.CustomerId != userId)
            {
                return Forbid();
            }
            
            // Calculate total charges (late fee + additional charges)
            ViewBag.TotalCharges = returnRecord.LateReturnFee + returnRecord.AdditionalCharges;
            
            // Calculate days late (if any)
            var daysLate = (returnRecord.ActualReturnDate - returnRecord.RentalTransaction.RentalEndDate).Days;
            ViewBag.DaysLate = daysLate > 0 ? daysLate : 0;
            
            // Set ViewBag data for the view
            ViewBag.IsManager = isManager;

            return View(returnRecord);
        }

        // GET: ReturnRecords/Create
        [Authorize(Roles = "Manager,Administrator")]
        public async Task<IActionResult> Create(int? transactionId = null)
        {
            // Get transactions that don't have return records
            var transactions = await _context.RentalTransactions
                .Include(rt => rt.Customer)
                .Include(rt => rt.Equipment)
                .Where(rt => !_context.ReturnRecords.Any(rr => rr.RentalTransactionId == rt.RentalTransactionId))
                .Where(rt => rt.PaymentStatus == "Paid")
                .ToListAsync();
                
            // Create select list items
            var items = transactions.Select(t => new SelectListItem
            {
                Value = t.RentalTransactionId.ToString(),
                Text = $"#{t.RentalTransactionId} - {t.Equipment?.Name} - {t.Customer?.FullName} - {t.RentalStartDate.ToShortDateString()} to {t.RentalEndDate.ToShortDateString()}",
                Selected = transactionId.HasValue && t.RentalTransactionId == transactionId.Value
            }).ToList();
            
            // Add empty item at the beginning
            items.Insert(0, new SelectListItem { Value = "", Text = "-- Select a Transaction --" });
            
            ViewBag.Transactions = items;
            
            // Set default values for a new return record
            var model = new ReturnRecord
            {
                ActualReturnDate = DateTime.Now,
                ReturnCondition = "Good",
                LateReturnFee = 0,
                AdditionalCharges = 0
            };
            
            // If a transaction ID was provided, pre-populate the form
            if (transactionId.HasValue)
            {
                var transaction = await _context.RentalTransactions
                    .Include(t => t.Equipment)
                    .Include(t => t.Customer)
                    .FirstOrDefaultAsync(t => t.RentalTransactionId == transactionId);
                    
                if (transaction != null)
                {
                    model.RentalTransactionId = transaction.RentalTransactionId;
                    ViewBag.Transaction = transaction;
                    
                    // Calculate late fee if returned after end date
                    if (DateTime.Now > transaction.RentalEndDate)
                    {
                        int daysLate = (int)(DateTime.Now - transaction.RentalEndDate).TotalDays;
                        model.LateReturnFee = daysLate * (transaction.RentalFee / transaction.RentalPeriod);
                    }
                }
            }
            
            return View(model);
        }

        // POST: ReturnRecords/Create
        [Authorize(Roles = "Manager,Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int RentalTransactionId, DateTime ActualReturnDate, string ReturnCondition, decimal LateReturnFee, decimal AdditionalCharges, string Notes)
        {
            // Log all parameters for debugging
            System.Diagnostics.Debug.WriteLine($"Form submission - RentalTransactionId: {RentalTransactionId}");
            System.Diagnostics.Debug.WriteLine($"Form submission - ActualReturnDate: {ActualReturnDate}");
            System.Diagnostics.Debug.WriteLine($"Form submission - ReturnCondition: {ReturnCondition}");
            System.Diagnostics.Debug.WriteLine($"Form submission - LateReturnFee: {LateReturnFee}");
            System.Diagnostics.Debug.WriteLine($"Form submission - AdditionalCharges: {AdditionalCharges}");
            System.Diagnostics.Debug.WriteLine($"Form submission - Notes: {Notes}");
            
            try
            {
                // Check if RentalTransactionId is valid
                if (RentalTransactionId <= 0)
                {
                    TempData["Error"] = "Please select a valid transaction";
                    return RedirectToAction(nameof(Create));
                }
                
                // Check if a return record already exists for this transaction
                var existingRecord = await _context.ReturnRecords
                    .FirstOrDefaultAsync(r => r.RentalTransactionId == RentalTransactionId);
                    
                if (existingRecord != null)
                {
                    TempData["Error"] = "A return record already exists for this transaction";
                    return RedirectToAction(nameof(Create), new { transactionId = RentalTransactionId });
                }
                
                // Create a new return record
                var returnRecord = new ReturnRecord
                {
                    RentalTransactionId = RentalTransactionId,
                    ActualReturnDate = ActualReturnDate,
                    ReturnCondition = ReturnCondition,
                    LateReturnFee = LateReturnFee,
                    AdditionalCharges = AdditionalCharges,
                    Notes = Notes
                };
                
                // Add the return record
                _context.ReturnRecords.Add(returnRecord);
                
                // Update the equipment status to Available
                var transaction = await _context.RentalTransactions
                    .Include(t => t.Equipment)
                    .FirstOrDefaultAsync(t => t.RentalTransactionId == RentalTransactionId);
                    
                if (transaction != null && transaction.Equipment != null)
                {
                    transaction.Equipment.AvailabilityStatus = "Available";
                    _context.Update(transaction.Equipment);
                }
                
                // Save changes to database
                await _context.SaveChangesAsync();
             
                // Send notification to the customer
                if (transaction != null)
                {
                    await _notificationService.CreateNotification(
                        transaction.CustomerId,
                        $"Your return for {transaction.Equipment?.Name ?? "equipment"} has been processed. Thank you!",
                        "Return Completed"
                    );
                }

                TempData["Success"] = "Equipment return processed successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating return record: {ex.Message}");
                TempData["Error"] = $"Error creating return record: {ex.Message}";
                return RedirectToAction(nameof(Create), new { transactionId = RentalTransactionId });
            }
        }

        // POST: ReturnRecords/CreateDirect
        [Authorize(Roles = "Manager,Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDirect(int RentalTransactionId, DateTime ActualReturnDate, string ReturnCondition, decimal LateReturnFee, decimal AdditionalCharges, string Notes)
        {
            System.Diagnostics.Debug.WriteLine("===== DIRECT CREATE METHOD CALLED =====");
            System.Diagnostics.Debug.WriteLine($"RentalTransactionId: {RentalTransactionId}");
            System.Diagnostics.Debug.WriteLine($"ActualReturnDate: {ActualReturnDate}");
            System.Diagnostics.Debug.WriteLine($"ReturnCondition: {ReturnCondition}");
            System.Diagnostics.Debug.WriteLine($"LateReturnFee: {LateReturnFee}");
            System.Diagnostics.Debug.WriteLine($"AdditionalCharges: {AdditionalCharges}");
            System.Diagnostics.Debug.WriteLine($"Notes: {Notes}");
            
            try
            {
                // Create a new return record
                var returnRecord = new ReturnRecord
                {
                    RentalTransactionId = RentalTransactionId,
                    ActualReturnDate = ActualReturnDate,
                    ReturnCondition = ReturnCondition,
                    LateReturnFee = LateReturnFee,
                    AdditionalCharges = AdditionalCharges,
                    Notes = Notes
                };
                
                // Add the return record
                _context.ReturnRecords.Add(returnRecord);
                
                // Update the equipment status to Available
                var transaction = await _context.RentalTransactions
                    .Include(t => t.Equipment)
                    .FirstOrDefaultAsync(t => t.RentalTransactionId == RentalTransactionId);
                    
                if (transaction != null && transaction.Equipment != null)
                {
                    transaction.Equipment.AvailabilityStatus = "Available";
                    _context.Update(transaction.Equipment);
                }
                
                // Save changes to database
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Equipment return processed successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in CreateDirect: {ex.Message}");
                TempData["Error"] = $"Error processing return: {ex.Message}";
                return RedirectToAction(nameof(Create), new { transactionId = RentalTransactionId });
            }
        }
        
        // Helper method to prepare the create form
        private async Task PrepareCreateForm(int? transactionId = null)
        {
            // Get only transactions that don't already have a return record
            var completedTransactionsWithoutReturns = await _context.RentalTransactions
                .Include(rt => rt.Customer)
                .Include(rt => rt.Equipment)
                .Where(rt => !_context.ReturnRecords.Any(rr => rr.RentalTransactionId == rt.RentalTransactionId) || rt.RentalTransactionId == transactionId)
                .Where(rt => rt.PaymentStatus == "Paid")
                .ToListAsync();
                
            // Create a select list with more informative display text
            var selectList = completedTransactionsWithoutReturns.Select(t => new
            {
                t.RentalTransactionId,
                DisplayText = $"#{t.RentalTransactionId} - {t.Equipment.Name} - {t.Customer.FullName} - {t.RentalStartDate.ToShortDateString()} to {t.RentalEndDate.ToShortDateString()}"
            });
            
            ViewData["RentalTransactionId"] = new SelectList(selectList, "RentalTransactionId", "DisplayText", transactionId);
            
            // If a transaction ID was provided, add transaction details to ViewBag
            if (transactionId.HasValue)
            {
                var transaction = await _context.RentalTransactions
                    .Include(t => t.Equipment)
                    .Include(t => t.Customer)
                    .FirstOrDefaultAsync(t => t.RentalTransactionId == transactionId);
                    
                if (transaction != null)
                {
                    ViewBag.Transaction = transaction;
                }
            }
        }

        // GET: ReturnRecords/Edit/5
        [Authorize(Roles = "Manager,Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ReturnRecords == null)
            {
                return NotFound();
            }

            var returnRecord = await _context.ReturnRecords
                .Include(r => r.RentalTransaction)
                .ThenInclude(t => t.Equipment)
                .Include(r => r.RentalTransaction.Customer)
                .FirstOrDefaultAsync(r => r.ReturnRecordId == id);
                
            if (returnRecord == null)
            {
                return NotFound();
            }
            
            // Get the transaction details for the sidebar
            ViewBag.Transaction = returnRecord.RentalTransaction;
            
            // Create a list with just this transaction for the dropdown
            var transaction = returnRecord.RentalTransaction;
            var items = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = transaction.RentalTransactionId.ToString(),
                    Text = $"#{transaction.RentalTransactionId} - {transaction.Equipment?.Name} - {transaction.Customer?.FullName} - {transaction.RentalStartDate.ToShortDateString()} to {transaction.RentalEndDate.ToShortDateString()}",
                    Selected = true
                }
            };
            
            ViewBag.Transactions = items;
            
            return View(returnRecord);
        }

        // POST: ReturnRecords/Edit/5
        [Authorize(Roles = "Manager,Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int RentalTransactionId, DateTime ActualReturnDate, 
            string ReturnCondition, decimal LateReturnFee, decimal AdditionalCharges, string Notes)
        {
            // Log all parameters for debugging
            System.Diagnostics.Debug.WriteLine($"Edit form submission - ReturnRecordId: {id}");
            System.Diagnostics.Debug.WriteLine($"Edit form submission - RentalTransactionId: {RentalTransactionId}");
            System.Diagnostics.Debug.WriteLine($"Edit form submission - ActualReturnDate: {ActualReturnDate}");
            System.Diagnostics.Debug.WriteLine($"Edit form submission - ReturnCondition: {ReturnCondition}");
            System.Diagnostics.Debug.WriteLine($"Edit form submission - LateReturnFee: {LateReturnFee}");
            System.Diagnostics.Debug.WriteLine($"Edit form submission - AdditionalCharges: {AdditionalCharges}");
            System.Diagnostics.Debug.WriteLine($"Edit form submission - Notes: {Notes}");
            
            try
            {
                // Find the existing record
                var returnRecord = await _context.ReturnRecords.FindAsync(id);
                if (returnRecord == null)
                {
                    return NotFound();
                }
                
                // Update the record properties
                returnRecord.RentalTransactionId = RentalTransactionId;
                returnRecord.ActualReturnDate = ActualReturnDate;
                returnRecord.ReturnCondition = ReturnCondition;
                returnRecord.LateReturnFee = LateReturnFee;
                returnRecord.AdditionalCharges = AdditionalCharges;
                returnRecord.Notes = Notes;
                
                // Update the record in the database
                _context.Update(returnRecord);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Return record updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating return record: {ex.Message}");
                TempData["Error"] = $"Error updating return record: {ex.Message}";
                return RedirectToAction(nameof(Edit), new { id });
            }
        }

        // GET: ReturnRecords/Delete/5
        [Authorize(Roles = "Manager,Administrator")]

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ReturnRecords == null)
            {
                return NotFound();
            }

            var returnRecord = await _context.ReturnRecords
                .Include(r => r.RentalTransaction)
                .FirstOrDefaultAsync(m => m.ReturnRecordId == id);
            if (returnRecord == null)
            {
                return NotFound();
            }

            return View(returnRecord);
        }

        // POST: ReturnRecords/Delete/5
        [Authorize(Roles = "Manager,Administrator")]

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ReturnRecords == null)
            {
                return Problem("Entity set 'EquipmentRentalDbContext.ReturnRecords'  is null.");
            }
            var returnRecord = await _context.ReturnRecords.FindAsync(id);
            if (returnRecord != null)
            {
                _context.ReturnRecords.Remove(returnRecord);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReturnRecordExists(int id)
        {
          return (_context.ReturnRecords?.Any(e => e.ReturnRecordId == id)).GetValueOrDefault();
        }
    }
}
