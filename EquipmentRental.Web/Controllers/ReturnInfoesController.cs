using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvancedProgrammingASPProject.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectDBClassLibrary.Model;

namespace AdvancedProgrammingASPProject.Controllers
{
    [Authorize]
    public class ReturnInfoesController : Controller
    {
        private readonly ProjectDBContext _context;
        private readonly UserManager<Users> _userManager;

        public ReturnInfoesController(ProjectDBContext context, UserManager<Users> userManager)
        {
            _context = context;
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

        public async Task<IActionResult> Index(int? returnIdSearch, string? paymentStatusFilter, string? conditionFilter, int page = 1)
        {
            int pageSize = 15;
            var (mainUser, isAdminOrManager, isCustomer) = await GetCurrentUser();

            var returnInfosQuery = _context.ReturnInfos
                .Include(r => r.Transaction)
                .ThenInclude(t => t.User)
                .AsQueryable();

            if (isCustomer)
                returnInfosQuery = returnInfosQuery.Where(r => r.Transaction.UserId == mainUser.UserId);

            // Materialize query before applying trimming and lowercase filters
            var returnInfos = await returnInfosQuery.ToListAsync();

            if (returnIdSearch.HasValue)
                returnInfos = returnInfos.Where(r => r.ReturnId == returnIdSearch.Value).ToList();

            if (!string.IsNullOrWhiteSpace(paymentStatusFilter))
            {
                var normalizedPayment = paymentStatusFilter.Trim().ToLower();
                returnInfos = returnInfos
                    .Where(r => !string.IsNullOrEmpty(r.Transaction.PaymentStatus) &&
                                r.Transaction.PaymentStatus.Trim().ToLower() == normalizedPayment)
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(conditionFilter))
            {
                var normalizedCondition = conditionFilter.Trim().ToLower();
                returnInfos = returnInfos
                    .Where(r => !string.IsNullOrEmpty(r.ReturnCondition) &&
                                r.ReturnCondition.Trim().ToLower() == normalizedCondition)
                    .ToList();
            }

            var totalItems = returnInfos.Count;
            var items = returnInfos
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.PaymentStatusFilter = new SelectList(new List<string> { "Paid", "Not Yet", "Partially Paid" }, paymentStatusFilter);

            var conditionList = await _context.ReturnEquipmentConditions
                .Select(c => new SelectListItem { Value = c.Condition, Text = c.Condition })
                .ToListAsync();

            ViewBag.ConditionFilter = new SelectList(conditionList, "Value", "Text", conditionFilter);
            ViewBag.IsAdminOrManager = isAdminOrManager;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return View(items);
        }





        private void LoadReturnConditions(string? selectedCondition = null, bool excludeFirst = false)
        {
            var conditions = _context.ReturnEquipmentConditions
                .OrderBy(c => c.ReturnEquipmentConditionID)
                .ToList();

            if (excludeFirst)
                conditions = conditions.Skip(1).ToList(); // Remove "On Time in Good Condition"

            var list = conditions
                .Select(c => new SelectListItem
                {
                    Value = c.Condition,
                    Text = c.Condition
                }).ToList();

            ViewBag.ConditionList = new SelectList(list, "Value", "Text", selectedCondition);
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var returnInfo = await _context.ReturnInfos
                .Include(r => r.Transaction)
                .FirstOrDefaultAsync(m => m.ReturnId == id);
            if (returnInfo == null)
            {
                return NotFound();
            }

            return View(returnInfo);
        }

        public async Task<IActionResult> HandleReturn(int rentalRequestId)
        {

            var (mainUser, isAdminOrManager, isCustomer) = await GetCurrentUser();
            if (isCustomer)
                return Forbid();


            var transaction = await _context.RentalTransactions
                .Include(t => t.RentalRequest)
                .Include(t => t.User)
                .Include(t => t.Equipment)
                .FirstOrDefaultAsync(t => t.RentalRequestId == rentalRequestId);

            if (transaction == null) return NotFound();

            var returnInfo = await _context.ReturnInfos
                .FirstOrDefaultAsync(r => r.TransactionId == transaction.RentalTransactionId);

            if (returnInfo != null)
                return RedirectToAction("Edit", new { id = returnInfo.ReturnId });

            ViewBag.Transaction = transaction;
            ViewBag.Today = DateTime.Now.ToString("yyyy-MM-dd");
            LoadReturnConditions();

            return View("Create", new ReturnInfo
            {
                TransactionId = transaction.RentalTransactionId,
                CreatedAt = DateTime.Now,
                ReturnDate = DateTime.Now
            });
        }

        public async Task<IActionResult> Create()
        {
            var (_, isAdminOrManager, isCustomer) = await GetCurrentUser();
            if (!isAdminOrManager) return Forbid();

            ViewData["TransactionId"] = new SelectList(_context.RentalTransactions, "RentalTransactionId", "PaymentStatus");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReturnInfo returnInfo)
        {
            ModelState.Remove("Transaction");

            var (mainUser, isAdminOrManager, isCustomer) = await GetCurrentUser();

            var transaction = await _context.RentalTransactions
                .Include(t => t.User)
                .Include(t => t.Equipment)
                .Include(t => t.RentalRequest)
                .FirstOrDefaultAsync(t => t.RentalTransactionId == returnInfo.TransactionId);

            if (transaction == null)
                return NotFound();

            if (isCustomer && transaction.UserId != mainUser.UserId)
                return Forbid();

            // Server-side validations
            if (!string.Equals(returnInfo.ReturnCondition?.Trim(), "On Time in Good Condition", StringComparison.OrdinalIgnoreCase))
            {
                if (returnInfo.LateReturnFees == null || returnInfo.LateReturnFees <= 0)
                {
                    ModelState.AddModelError("LateReturnFees", "Late Fee is required when condition is not 'On Time in Good Condition'.");
                }
            }

            if (returnInfo.PaidLateFees < 0)
                ModelState.AddModelError("PaidLateFees", "Paid Late Fees cannot be negative.");

            if (ModelState.IsValid)
            {
                returnInfo.CreatedAt = DateTime.Now;
                returnInfo.PaidLateFees ??= 0;

                _context.Add(returnInfo);
                await _context.SaveChangesAsync();


                var restorableConditions = new[] { "On Time in Good Condition", "Late But Good Condition" };
                bool isRestorable = restorableConditions.Contains(returnInfo.ReturnCondition?.Trim(), StringComparer.OrdinalIgnoreCase);

                if (isRestorable && returnInfo.StockRestored != true)
                {
                    var equipment = transaction.Equipment;
                    equipment.Quantity += 1;
                    equipment.AvailabilityStatus = equipment.Quantity > 0;
                    returnInfo.StockRestored = true;

                    _context.Update(equipment);
                }




                // Update payment status
                double totalDue = transaction.RentalFee + (returnInfo.LateReturnFees ?? 0) + (returnInfo.AdditionalCharges ?? 0);
                double totalPaid = transaction.Deposit + returnInfo.PaidLateFees.Value;

                transaction.PaymentStatus = totalPaid == 0
                    ? "Not yet"
                    : totalPaid < totalDue ? "Partially Paid" : "Paid";

                _context.Update(transaction);
                await _context.SaveChangesAsync();

                var notification = new Notification
                {
                    Type = "Return Submitted",
                    Message = $"A return has been submitted for your rental transaction of {transaction.Equipment?.Name}.",
                    Status = false
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                _context.NotificationUsers.Add(new NotificationUser
                {
                    NotificationId = notification.NotificationId,
                    UserId = transaction.UserId
                });

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            LoadReturnConditions(returnInfo.ReturnCondition);
            ViewBag.Transaction = transaction;
            ViewBag.Today = DateTime.Now.ToString("yyyy-MM-dd");
            return View(returnInfo);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var returnInfo = await _context.ReturnInfos
                .Include(r => r.Transaction)
                .ThenInclude(t => t.User)
                .FirstOrDefaultAsync(r => r.ReturnId == id);

            if (returnInfo == null)
                return NotFound();

            var (mainUser, isAdminOrManager, isCustomer) = await GetCurrentUser();

            if (isCustomer)
                return Forbid();


            ViewBag.Transaction = await _context.RentalTransactions
                .Include(t => t.User)
                .Include(t => t.Equipment)
                .FirstOrDefaultAsync(t => t.RentalTransactionId == returnInfo.TransactionId);

            ViewBag.Today = DateTime.Now.ToString("yyyy-MM-dd");
            LoadReturnConditions(returnInfo.ReturnCondition);

            return View(returnInfo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ReturnInfo returnInfo)
        {
            if (id != returnInfo.ReturnId)
                return NotFound();

            ModelState.Remove("Transaction");

            var (mainUser, isAdminOrManager, isCustomer) = await GetCurrentUser();

            var existingReturn = await _context.ReturnInfos
                .Include(r => r.Transaction)
                .ThenInclude(t => t.Equipment)
                .FirstOrDefaultAsync(r => r.ReturnId == id);

            if (existingReturn == null)
                return NotFound();

            if (isCustomer)
                return Forbid();


            // Server-side validations
            if (!string.Equals(returnInfo.ReturnCondition?.Trim(), "On Time in Good Condition", StringComparison.OrdinalIgnoreCase))
            {
                if (returnInfo.LateReturnFees == null || returnInfo.LateReturnFees <= 0)
                {
                    ModelState.AddModelError("LateReturnFees", "Late Fee is required when condition is not 'On Time in Good Condition'.");
                }
            }

            if (returnInfo.PaidLateFees < 0)
                ModelState.AddModelError("PaidLateFees", "Paid Late Fees cannot be negative.");

            if (ModelState.IsValid)
            {
                try
                {
                    if (returnInfo.ReturnDate < new DateTime(1753, 1, 1))
                        returnInfo.ReturnDate = DateTime.Now;

                    if (returnInfo.CreatedAt < new DateTime(1753, 1, 1))
                        returnInfo.CreatedAt = DateTime.Now;

                    returnInfo.PaidLateFees ??= 0;

                    // Handle stock logic BEFORE applying updated values
                    var restorableConditions = new[] { "On Time in Good Condition", "Late But Good Condition" };
                    var nonRestorableConditions = new[] { "Lost", "Returned with Damage" };

                    string oldCondition = existingReturn.ReturnCondition?.Trim();
                    string newCondition = returnInfo.ReturnCondition?.Trim();

                    bool wasRestorable = restorableConditions.Contains(oldCondition, StringComparer.OrdinalIgnoreCase);
                    bool isNowRestorable = restorableConditions.Contains(newCondition, StringComparer.OrdinalIgnoreCase);
                    bool wasNonRestorable = nonRestorableConditions.Contains(oldCondition, StringComparer.OrdinalIgnoreCase);
                    bool isNowNonRestorable = nonRestorableConditions.Contains(newCondition, StringComparer.OrdinalIgnoreCase);
                    bool wasRestored = existingReturn.StockRestored ?? false;

                    var equipment = existingReturn.Transaction.Equipment;

                    if (!wasRestorable && isNowRestorable && !wasRestored)
                    {
                        equipment.Quantity += 1;
                        existingReturn.StockRestored = true;
                    }
                    else if (wasRestorable && !isNowRestorable && wasRestored)
                    {
                        equipment.Quantity = Math.Max(0, equipment.Quantity - 1);
                        existingReturn.StockRestored = false;
                    }

                    equipment.AvailabilityStatus = equipment.Quantity > 0;
                    _context.Equipments.Update(equipment);

                    // Update returnInfo fields but preserve StockRestored
                    bool currentStockRestored = existingReturn.StockRestored ?? false;
                    _context.Entry(existingReturn).CurrentValues.SetValues(returnInfo);
                    existingReturn.StockRestored = currentStockRestored;
                    await _context.SaveChangesAsync();






                    // Update payment status
                    var transaction = await _context.RentalTransactions
                        .Include(t => t.ReturnInfos)
                        .FirstOrDefaultAsync(t => t.RentalTransactionId == returnInfo.TransactionId);

                    double totalDue = transaction.RentalFee + (returnInfo.LateReturnFees ?? 0) + (returnInfo.AdditionalCharges ?? 0);
                    double totalPaid = transaction.Deposit + returnInfo.PaidLateFees.Value;

                    transaction.PaymentStatus = totalPaid == 0
                        ? "Not yet"
                        : totalPaid < totalDue ? "Partially Paid" : "Paid";

                    _context.Update(transaction);
                    await _context.SaveChangesAsync();

                    var notification = new Notification
                    {
                        Type = "Return Updated",
                        Message = $"Your return information for {existingReturn.Transaction?.Equipment?.Name} was updated.",
                        Status = false
                    };

                    _context.Notifications.Add(notification);
                    await _context.SaveChangesAsync();

                    _context.NotificationUsers.Add(new NotificationUser
                    {
                        NotificationId = notification.NotificationId,
                        UserId = existingReturn.Transaction.UserId
                    });

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.ReturnInfos.Any(e => e.ReturnId == returnInfo.ReturnId))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            LoadReturnConditions(returnInfo.ReturnCondition, excludeFirst: true);

            ViewBag.Transaction = await _context.RentalTransactions
                .Include(t => t.User)
                .Include(t => t.Equipment)
                .FirstOrDefaultAsync(t => t.RentalTransactionId == returnInfo.TransactionId);

            ViewBag.Today = DateTime.Now.ToString("yyyy-MM-dd");
            return View(returnInfo);
        }


        public async Task<IActionResult> Delete(int? id)
        {

            var (mainUser, isAdminOrManager, isCustomer) = await GetCurrentUser();
            if (isCustomer)
                return Forbid();

            if (id == null)
            {
                return NotFound();
            }

            var returnInfo = await _context.ReturnInfos
                .Include(r => r.Transaction)
                .FirstOrDefaultAsync(m => m.ReturnId == id);
            if (returnInfo == null)
            {
                return NotFound();
            }

            return View(returnInfo);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var (mainUser, isAdminOrManager, isCustomer) = await GetCurrentUser();
            if (isCustomer)
                return Forbid();

            var returnInfo = await _context.ReturnInfos
                .Include(r => r.Transaction)
                .ThenInclude(t => t.Equipment)
                .FirstOrDefaultAsync(r => r.ReturnId == id);

            if (returnInfo != null)
            {
                var restorableConditions = new[] { "On Time in Good Condition", "Late But Good Condition" };
                bool wasRestored = returnInfo.StockRestored ?? false;
                bool isRestorable = restorableConditions.Contains(returnInfo.ReturnCondition?.Trim(), StringComparer.OrdinalIgnoreCase);

              
                if (wasRestored && isRestorable)
                {
                    var equipment = returnInfo.Transaction.Equipment;
                    equipment.Quantity = Math.Max(0, equipment.Quantity - 1);
                    equipment.AvailabilityStatus = equipment.Quantity > 0;
                    _context.Equipments.Update(equipment);
                }


                _context.ReturnInfos.Remove(returnInfo);
                await _context.SaveChangesAsync();

                var notification = new Notification
                {
                    Type = "Return Deleted",
                    Message = $"Your return information for {returnInfo.Transaction?.Equipment?.Name} has been deleted.",
                    Status = false
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                _context.NotificationUsers.Add(new NotificationUser
                {
                    NotificationId = notification.NotificationId,
                    UserId = returnInfo.Transaction.UserId
                });

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        private bool ReturnInfoExists(int id)
        {
            return _context.ReturnInfos.Any(e => e.ReturnId == id);
        }
    }
}
