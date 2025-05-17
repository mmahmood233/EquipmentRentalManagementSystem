using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using ProjectDBClassLibrary.Model;
using Microsoft.AspNetCore.Identity;
using AdvancedProgrammingASPProject.Areas.Identity.Data;
using System.Security.Claims;

namespace AdvancedProgrammingASPProject.Controllers
{
    [Authorize]
    public class RentalRequestsController : Controller
    {
        private readonly ProjectDBContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<Users> _userManager;

        public RentalRequestsController(ProjectDBContext context, IWebHostEnvironment webHostEnvironment, UserManager<Users> userManager)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int? customerFilter, string statusFilter, int? searchId, int page = 1)
        {
            int pageSize = 15;

            var identityUser = await _userManager.GetUserAsync(User);
            var mainUser = identityUser?.MainUserId != null
                ? await _context.Users.FindAsync(identityUser.MainUserId)
                : null;

            var isAdminOrManager = mainUser != null && (mainUser.RoleId == 1 || mainUser.RoleId == 3);
            var isCustomer = mainUser != null && mainUser.RoleId == 2;

            IQueryable<RentalRequest> rentalRequests = _context.RentalRequests
                .Include(r => r.User)
                .Include(r => r.Equipment)
                .Include(r => r.RentalTransactions);

            if (isCustomer)
                rentalRequests = rentalRequests.Where(r => r.UserId == mainUser.UserId);

            if (customerFilter.HasValue)
                rentalRequests = rentalRequests.Where(r => r.UserId == customerFilter.Value);

            if (!string.IsNullOrWhiteSpace(statusFilter))
                rentalRequests = rentalRequests.Where(r => r.Status == statusFilter);

            if (searchId.HasValue)
                rentalRequests = rentalRequests.Where(r => r.RentalRequestId == searchId.Value);

            // Load pending requests separately
            var pendingRequests = await rentalRequests
                .Where(r => r.Status == "Pending")
                .OrderBy(r => r.CreatedAt)
                .ToListAsync();

            var processedRequestsQuery = rentalRequests
                .Where(r => r.Status == "Approved" || r.Status == "Denied")
                .OrderByDescending(r => r.CreatedAt);

            var totalProcessedItems = await processedRequestsQuery.CountAsync();
            var processedItems = await processedRequestsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Setup ViewBag
            var customerList = await _context.Users
                .Where(u => u.RoleId == 2)
                .Select(u => new { u.UserId, Fullname = u.FirstName + " " + u.LastName })
                .ToListAsync();

            ViewBag.CustomerFilter = new SelectList(customerList, "UserId", "Fullname", customerFilter);
            ViewBag.StatusFilter = new SelectList(new List<string> { "Pending", "Approved", "Denied" }, statusFilter);
            ViewBag.SearchId = searchId;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalProcessedItems / (double)pageSize);
            ViewBag.IsCustomer = isCustomer;
            ViewBag.MainUserId = mainUser?.UserId;

            //  THIS IS THE IMPORTANT PART
            if (isCustomer)
            {
       
                var allCustomerRequests = pendingRequests.Concat(processedItems).ToList();
                return View(allCustomerRequests);
            }
            else
            {
                // For admins/managers: pending separate, processed paginated
                ViewBag.PendingRequests = pendingRequests;
                return View(processedItems);
            }
        }


        /// useful for making the book once available function
        private async Task<DateTime?> GetNearestApprovedReturnDate(int equipmentId)
        {
            var today = DateTime.Today;

            // Get the nearest approved return date regardless of being in the past or future
            return await _context.RentalRequests
                .Where(r => r.EquipmentId == equipmentId && r.Status == "Approved")
                .OrderByDescending(r => r.ReturnDate >= today) // First prefer future, then past
                .ThenBy(r => r.ReturnDate)
                .Select(r => r.ReturnDate)
                .FirstOrDefaultAsync();
        }




        public async Task<IActionResult> Create(int? equipmentId)
        {
            var identityUser = await _userManager.GetUserAsync(User);
            var mainUser = identityUser?.MainUserId != null ? await _context.Users.FindAsync(identityUser.MainUserId) : null;
            var isCustomer = mainUser?.RoleId == 2;

            if (isCustomer)
            {
                ViewBag.ReadOnlyUser = true;
                ViewBag.UserIdName = mainUser.FirstName + " " + mainUser.LastName;
                ViewBag.UserIdValue = mainUser.UserId;
            }
            else
            {
                var users = _context.Users
                    .Where(u => u.RoleId == 2)
                    .Select(u => new { u.UserId, Fullname = u.FirstName + " " + u.LastName })
                    .ToList();
                ViewData["UserId"] = new SelectList(users, "UserId", "Fullname");
                ViewBag.ReadOnlyUser = false;
            }

            if (equipmentId.HasValue)
            {
                var equipment = _context.Equipments.FirstOrDefault(e => e.EquipmentId == equipmentId.Value);
                if (equipment != null)
                {
                    if (!equipment.AvailabilityStatus)
                    {
                        var nearest = await GetNearestApprovedReturnDate(equipment.EquipmentId);
                        if (nearest.HasValue)
                        {
                            ViewBag.BookOnceAvailable = true;
                            ViewBag.MinStartDate = nearest.Value;

                        }
                        else
                        {
                            TempData["Error"] = "This equipment is unavailable and no approved rentals exist.";
                            return RedirectToAction("Index", "Equipments");
                        }
                    }

                    ViewBag.EquipmentName = equipment.Name;
                    ViewBag.DailyRentalPrice = equipment.RentalPrice;
                    ViewData["EquipmentId"] = equipment.EquipmentId;
                }
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RentalRequestId,EquipmentId,UserId,StartDate,ReturnDate,TotalCost,Status,IsBookOnceAvailable")] RentalRequest rentalRequest)
        {
            ModelState.Remove("Status");
            ModelState.Remove("TotalCost");
            ModelState.Remove("CreatedAt");
            ModelState.Remove("User");
            ModelState.Remove("Equipment");

            // Validate that ReturnDate is after StartDate
            if (rentalRequest.ReturnDate <= rentalRequest.StartDate)
            {
                ModelState.AddModelError("ReturnDate", "Return date must be after the start date.");
            }

            if (ModelState.IsValid)
            {
                var equipment = await _context.Equipments.FindAsync(rentalRequest.EquipmentId);
                if (equipment != null)
                {
                    if (!equipment.AvailabilityStatus)
                    {
                        var nearest = await GetNearestApprovedReturnDate(equipment.EquipmentId);
                        if (nearest.HasValue && rentalRequest.StartDate.Date < nearest.Value.Date)

                        {
                            ModelState.AddModelError("StartDate", $"Start date must be on or after {nearest.Value:yyyy-MM-dd} due to availability.");
                            ViewBag.BookOnceAvailable = true;
                            ViewBag.MinStartDate = nearest.Value;
                            await ReloadCreateViewBags(rentalRequest);
                            return View(rentalRequest);
                        }
                        rentalRequest.IsBookOnceAvailable = true;
                    }

                    var days = (rentalRequest.ReturnDate - rentalRequest.StartDate).Days;
                    rentalRequest.TotalCost = days > 0 ? days * equipment.RentalPrice : 0;
                }

                rentalRequest.Status = "Pending";
                rentalRequest.CreatedAt = DateTime.Now;

                _context.Add(rentalRequest);
                await _context.SaveChangesAsync();

                var notification = new Notification
                {
                    Type = "Rental Request Created",
                    Message = "Your rental request has been submitted.",
                    Status = false
                };
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                var notificationUser = new NotificationUser
                {
                    NotificationId = notification.NotificationId,
                    UserId = rentalRequest.UserId
                };
                _context.NotificationUsers.Add(notificationUser);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // Reload equipment info
            var equipmentReload = await _context.Equipments.FindAsync(rentalRequest.EquipmentId);
            if (equipmentReload != null)
            {
                ViewBag.EquipmentName = equipmentReload.Name;
                ViewBag.DailyRentalPrice = equipmentReload.RentalPrice;
                ViewData["EquipmentId"] = equipmentReload.EquipmentId;
            }

            // Reload user info
            var identityUser = await _userManager.GetUserAsync(User);
            var mainUser = identityUser?.MainUserId != null ? await _context.Users.FindAsync(identityUser.MainUserId) : null;
            var isCustomer = mainUser?.RoleId == 2;

            if (isCustomer)
            {
                ViewBag.ReadOnlyUser = true;
                ViewBag.UserIdName = mainUser.FirstName + " " + mainUser.LastName;
                ViewBag.UserIdValue = mainUser.UserId;
            }
            else
            {
                var users = _context.Users
                    .Where(u => u.RoleId == 2)
                    .Select(u => new { u.UserId, Fullname = u.FirstName + " " + u.LastName })
                    .ToList();
                ViewData["UserId"] = new SelectList(users, "UserId", "Fullname", rentalRequest.UserId);
                ViewBag.ReadOnlyUser = false;
            }

            // Restore Book Once Available warning (if needed)
            if (!rentalRequest.IsBookOnceAvailable.HasValue || rentalRequest.IsBookOnceAvailable == false)
            {
                var nearest = await GetNearestApprovedReturnDate(rentalRequest.EquipmentId);
                if (nearest.HasValue)
                {
                    ViewBag.BookOnceAvailable = true;
                    ViewBag.MinStartDate = nearest.Value;
                }
            }

            if (rentalRequest != null)
            {
                await ReloadCreateViewBags(rentalRequest);
            }
            return View(rentalRequest);

        }


        private async Task ReloadCreateViewBags(RentalRequest rentalRequest)
        {
            // Equipment info
            var equipmentReload = await _context.Equipments.FindAsync(rentalRequest.EquipmentId);
            if (equipmentReload != null)
            {
                ViewBag.EquipmentName = equipmentReload.Name;
                ViewBag.DailyRentalPrice = equipmentReload.RentalPrice;
                ViewData["EquipmentId"] = equipmentReload.EquipmentId;
            }

            // User info
            var identityUser = await _userManager.GetUserAsync(User);
            var mainUser = identityUser?.MainUserId != null ? await _context.Users.FindAsync(identityUser.MainUserId) : null;
            var isCustomer = mainUser?.RoleId == 2;

            if (isCustomer)
            {
                ViewBag.ReadOnlyUser = true;
                ViewBag.UserIdName = mainUser.FirstName + " " + mainUser.LastName;
                ViewBag.UserIdValue = mainUser.UserId;
            }
            else
            {
                var users = _context.Users
                    .Where(u => u.RoleId == 2)
                    .Select(u => new { u.UserId, Fullname = u.FirstName + " " + u.LastName })
                    .ToList();
                ViewData["UserId"] = new SelectList(users, "UserId", "Fullname", rentalRequest.UserId);
                ViewBag.ReadOnlyUser = false;
            }

            // BookOnceAvailable
            var nearest = await GetNearestApprovedReturnDate(rentalRequest.EquipmentId);
            if (nearest.HasValue)
            {
                ViewBag.BookOnceAvailable = true;
                ViewBag.MinStartDate = nearest.Value;
            }
        }





        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var rentalRequest = await _context.RentalRequests
                .Include(r => r.User)
                .Include(r => r.Equipment)
                .FirstOrDefaultAsync(r => r.RentalRequestId == id);

            if (rentalRequest == null) return NotFound();

            var identityUser = await _userManager.GetUserAsync(User);
            var mainUser = identityUser?.MainUserId != null ? await _context.Users.FindAsync(identityUser.MainUserId) : null;
            var isCustomer = mainUser?.RoleId == 2;

            ViewBag.UserName = rentalRequest.User.FirstName + " " + rentalRequest.User.LastName;
            ViewBag.EquipmentName = rentalRequest.Equipment.Name;
            ViewBag.IsCustomer = isCustomer;

            var users = await _context.Users
                .Where(u => u.RoleId == 2)
                .Select(u => new { u.UserId, Fullname = u.FirstName + " " + u.LastName })
                .ToListAsync();

            ViewBag.UserList = new SelectList(users, "UserId", "Fullname", rentalRequest.UserId);

            if (!rentalRequest.Equipment.AvailabilityStatus)
            {
                var nearest = await GetNearestApprovedReturnDate(rentalRequest.EquipmentId);
                if (nearest.HasValue)
                {
                    ViewBag.MinStartDate = nearest.Value;
                }
            }

            if (isCustomer)
            {
                if (rentalRequest.UserId != mainUser.UserId) return Forbid();
                if (rentalRequest.Status != "Pending")
                {
                    TempData["Error"] = "This rental request is not editable.";
                    return RedirectToAction("Index");
                }
            }

            return View(rentalRequest);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RentalRequestId,EquipmentId,UserId,StartDate,ReturnDate,TotalCost,Status,IsBookOnceAvailable")] RentalRequest rentalRequest)
        {
            ModelState.Remove("User");
            ModelState.Remove("Equipment");

            if (id != rentalRequest.RentalRequestId) return NotFound();

            if (ModelState.IsValid)
            {
                var existingRequest = await _context.RentalRequests
      .AsNoTracking()
      .Include(r => r.User)
      .FirstOrDefaultAsync(r => r.RentalRequestId == id);

                if (existingRequest == null) return NotFound();

                rentalRequest.CreatedAt = existingRequest.CreatedAt;

                var equipment = await _context.Equipments.FindAsync(rentalRequest.EquipmentId);
                if (equipment == null) return NotFound();

                // BookOnceAvailable logic
                if (existingRequest.IsBookOnceAvailable == true && existingRequest.Status != "Approved")
                {
                    var nearest = await GetNearestApprovedReturnDate(rentalRequest.EquipmentId);
                    if (nearest.HasValue && rentalRequest.StartDate.Date < nearest.Value.Date)
                    {
                        ModelState.AddModelError("StartDate", $"Start date must be on or after {nearest.Value:yyyy-MM-dd} due to availability.");
                        ViewBag.MinStartDate = nearest.Value;

                        // Show fixed info again
                        ViewBag.UserName = existingRequest.User?.FirstName + " " + existingRequest.User?.LastName;
                        ViewBag.EquipmentName = equipment.Name;
                        ViewData["EquipmentId"] = equipment.EquipmentId;
                        ViewData["UserId"] = new SelectList(_context.Users
                            .Where(u => u.RoleId == 2)
                            .Select(u => new { u.UserId, Fullname = u.FirstName + " " + u.LastName }),
                            "UserId", "Fullname", rentalRequest.UserId);
                        return View(rentalRequest);
                    }
                }

                // Handle stock logic if status changed
                if (existingRequest.Status != rentalRequest.Status)
                {
                    if (existingRequest.Status == "Approved" && rentalRequest.Status != "Approved")
                    {
                        equipment.Quantity += 1;
                    }
                    else if (existingRequest.Status != "Approved" && rentalRequest.Status == "Approved")
                    {
                        if (equipment.Quantity <= 0)
                        {
                            ModelState.AddModelError("Status", "Cannot approve this request. The equipment is currently out of stock. Please wait until another customer returns it.");

                            ViewBag.EquipmentName = equipment.Name;
                            ViewBag.UserName = existingRequest.User?.FirstName + " " + existingRequest.User?.LastName;
                            ViewData["EquipmentId"] = equipment.EquipmentId;
                            ViewData["UserId"] = new SelectList(_context.Users
                                .Where(u => u.RoleId == 2)
                                .Select(u => new { u.UserId, Fullname = u.FirstName + " " + u.LastName }),
                                "UserId", "Fullname", rentalRequest.UserId);
                            return View(rentalRequest);
                        }
                        rentalRequest.IsBookOnceAvailable = false; // auto convert if approved
                        equipment.Quantity -= 1;
                    }
                    equipment.AvailabilityStatus = equipment.Quantity > 0;
                    _context.Equipments.Update(equipment);
                }

                _context.Update(rentalRequest);
                await _context.SaveChangesAsync();

                var notification = new Notification
                {
                    Type = "Rental Request Updated",
                    Message = "Your rental request was updated.",
                    Status = false
                };
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                var notificationUser = new NotificationUser
                {
                    NotificationId = notification.NotificationId,
                    UserId = rentalRequest.UserId
                };
                _context.NotificationUsers.Add(notificationUser);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // On error, restore fixed display values
            var equipmentReload = await _context.Equipments.FindAsync(rentalRequest.EquipmentId);
            ViewBag.EquipmentName = equipmentReload?.Name ?? "Unknown";

            // Always fetch user and assign their name (readonly display only)
            var userReload = await _context.Users.FindAsync(rentalRequest.UserId);
            ViewBag.UserName = userReload != null ? userReload.FirstName + " " + userReload.LastName : "Unknown";

            // Set for view rendering logic
            var identityUser = await _userManager.GetUserAsync(User);
            var mainUser = identityUser?.MainUserId != null ? await _context.Users.FindAsync(identityUser.MainUserId) : null;
            ViewBag.IsCustomer = mainUser?.RoleId == 2;

            // Book Once Available warning (if needed)
            if (!equipmentReload.AvailabilityStatus)
            {
                var nearest = await GetNearestApprovedReturnDate(rentalRequest.EquipmentId);
                if (nearest.HasValue)
                {
                    ViewBag.MinStartDate = nearest.Value;

                }
            }

            return View(rentalRequest);

        }





        private bool RentalRequestExists(int id)
        {
            return _context.RentalRequests.Any(e => e.RentalRequestId == id);
        }


        [HttpGet("/api/equipment/{id}/price")]
        public async Task<IActionResult> GetEquipmentPrice(int id)
        {
            var equipment = await _context.Equipments.FirstOrDefaultAsync(e => e.EquipmentId == id);
            if (equipment == null) return NotFound();

            return Json(new { rentalPrice = equipment.RentalPrice }); // match the JS field
        }

        // GET: RentalRequests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var rentalRequest = await _context.RentalRequests
                .Include(r => r.User)
                .Include(r => r.Equipment)
                .FirstOrDefaultAsync(r => r.RentalRequestId == id);

            if (rentalRequest == null) return NotFound();

            var identityUser = await _userManager.GetUserAsync(User);
            var mainUser = identityUser?.MainUserId != null ? await _context.Users.FindAsync(identityUser.MainUserId) : null;

            //  Customers cannot delete
            if (mainUser == null || mainUser.RoleId == 2) return Forbid();


            return View(rentalRequest);
        }

        // POST: RentalRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var identityUser = await _userManager.GetUserAsync(User);
            var mainUser = identityUser?.MainUserId != null ? await _context.Users.FindAsync(identityUser.MainUserId) : null;

            // Customers cannot delete
            if (mainUser == null || mainUser.RoleId == 2) return Forbid();

            var rentalRequest = await _context.RentalRequests
                .Include(r => r.Equipment)
                .Include(r => r.RentalTransactions)
                    .ThenInclude(t => t.ReturnInfos)
                .FirstOrDefaultAsync(r => r.RentalRequestId == id);

            if (rentalRequest != null)
            {
                var userId = rentalRequest.UserId;
                var equipment = rentalRequest.Equipment;

                bool restoredFromReturn = false;
                bool hasReturnInfo = false;

                foreach (var transaction in rentalRequest.RentalTransactions.ToList())
                {
                    foreach (var returnInfo in transaction.ReturnInfos.ToList())
                    {
                        hasReturnInfo = true;

                        string condition = returnInfo.ReturnCondition?.Trim();
                        if (condition != null &&
                            (condition.Equals("Lost", StringComparison.OrdinalIgnoreCase) ||
                             condition.Equals("Returned with Damage", StringComparison.OrdinalIgnoreCase)))
                        {
                            // Restore stock since these conditions subtracted one
                            if (equipment != null)
                            {
                                equipment.Quantity += 1;
                                equipment.AvailabilityStatus = equipment.Quantity > 0;
                                _context.Equipments.Update(equipment);
                                restoredFromReturn = true;
                            }
                        }

                        _context.ReturnInfos.Remove(returnInfo);
                    }

                    _context.RentalTransactions.Remove(transaction);
                }

                // If Approved and there was NO return record, restore stock
                if (!hasReturnInfo && rentalRequest.Status == "Approved" && equipment != null)
                {
                    equipment.Quantity += 1;
                    equipment.AvailabilityStatus = equipment.Quantity > 0;
                    _context.Equipments.Update(equipment);
                }

                _context.RentalRequests.Remove(rentalRequest);
                await _context.SaveChangesAsync();

                // Notification
                var notification = new Notification
                {
                    Type = "Rental Request Deleted",
                    Message = "Your rental request and related data have been deleted by the admin.",
                    Status = false
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                _context.NotificationUsers.Add(new NotificationUser
                {
                    NotificationId = notification.NotificationId,
                    UserId = userId
                });

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }



        [Authorize]
        public async Task<IActionResult> MyCart()
        {
            var identityUser = await _userManager.GetUserAsync(User);
            var mainUser = identityUser?.MainUserId != null ? await _context.Users.FindAsync(identityUser.MainUserId) : null;

            if (mainUser == null || mainUser.RoleId != 2) return Forbid(); // Only customers

            var cartItems = await _context.RentalRequests
                .Include(r => r.Equipment)
                .Where(r => r.UserId == mainUser.UserId)
                .ToListAsync();

            return View(cartItems);
        }



    }
}