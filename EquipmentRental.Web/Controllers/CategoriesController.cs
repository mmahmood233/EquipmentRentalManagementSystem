// CONTROLLER: CategoriesController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectDBClassLibrary.Model;
using Microsoft.AspNetCore.Identity;
using AdvancedProgrammingASPProject.Areas.Identity.Data;
using System.Security.Claims;

namespace AdvancedProgrammingASPProject.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ProjectDBContext _context;
        private readonly LogService _logService;
        private readonly UserManager<Users> _userManager;

        public CategoriesController(ProjectDBContext context, LogService logService, UserManager<Users> userManager)
        {
            _context = context;
            _logService = logService;
            _userManager = userManager;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            bool isAdminOrManager = false;

            if (User.Identity.IsAuthenticated)
            {
                var identityUser = await _userManager.GetUserAsync(User);
                if (identityUser?.MainUserId != null)
                {
                    var mainUser = _context.Users.FirstOrDefault(u => u.UserId == identityUser.MainUserId);
                    if (mainUser != null && (mainUser.RoleId == 1 || mainUser.RoleId == 3))
                    {
                        isAdminOrManager = true;
                    }
                }
            }

            ViewBag.IsAdminOrManager = isAdminOrManager;

            var categories = await _context.Categories.ToListAsync();
            return View(categories);
        }


        // GET: Categories/Create
        public IActionResult Create()
        {
            if (!IsAdminOrManager()) return Forbid();
            return View();
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryId,CategoryName")] Category category)
        {
            if (!IsAdminOrManager()) return Forbid();

            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();

                var identityUserId = _userManager.GetUserId(User);
                await _logService.LogAsync(identityUserId, $"Created category: {category.CategoryName}", "CategoriesController");

                TempData["CreateMessage"] = "Category created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsAdminOrManager()) return Forbid();

            if (id == null) return NotFound();

            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();

            return View(category);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryId,CategoryName")] Category category)
        {
            if (!IsAdminOrManager()) return Forbid();

            if (id != category.CategoryId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();

                    var identityUserId = _userManager.GetUserId(User);
                    await _logService.LogAsync(identityUserId, $"Edited category: {category.CategoryName}", "CategoriesController");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Categories.Any(e => e.CategoryId == category.CategoryId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsAdminOrManager()) return Forbid();

            if (id == null) return NotFound();

            var category = await _context.Categories
                .Include(c => c.Equipment)
                .FirstOrDefaultAsync(m => m.CategoryId == id);

            if (category == null) return NotFound();

            ViewBag.EquipmentCount = category.Equipment.Count;
            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsAdminOrManager()) return Forbid();

            var category = await _context.Categories
                .Include(c => c.Equipment)
                    .ThenInclude(e => e.RentalRequests)
                        .ThenInclude(rr => rr.User)
                .Include(c => c.Equipment)
                    .ThenInclude(e => e.RentalRequests)
                        .ThenInclude(rr => rr.RentalTransactions)
                            .ThenInclude(rt => rt.ReturnInfos)
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category != null)
            {
                // Get affected customer list (those who had requests under equipment in this category)
                var affectedUsers = category.Equipment
                    .SelectMany(e => e.RentalRequests)
                    .Select(r => r.User)
                    .Where(u => u != null)
                    .Distinct()
                    .ToList();

                //  Create separate notifications per customer
                foreach (var user in affectedUsers)
                {
                    var personalNotification = new Notification
                    {
                        Type = "Category Deleted",
                        Message = $"The category '{category.CategoryName}' and all its equipment have been deleted. Any rentals or returns you had under this category were also removed.",
                        Status = false
                    };

                    _context.Notifications.Add(personalNotification);
                    await _context.SaveChangesAsync();

                    _context.NotificationUsers.Add(new NotificationUser
                    {
                        NotificationId = personalNotification.NotificationId,
                        UserId = user.UserId
                    });
                }

                // Delete associated data (same as your existing logic)
                foreach (var equipment in category.Equipment)
                {
                    foreach (var rentalRequest in equipment.RentalRequests)
                    {
                        foreach (var transaction in rentalRequest.RentalTransactions)
                        {
                            if (transaction.ReturnInfos.Any())
                            {
                                _context.ReturnInfos.RemoveRange(transaction.ReturnInfos);
                            }
                        }
                        _context.RentalTransactions.RemoveRange(rentalRequest.RentalTransactions);
                    }
                    _context.RentalRequests.RemoveRange(equipment.RentalRequests);
                }

                _context.Equipments.RemoveRange(category.Equipment);
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                var identityUserId = _userManager.GetUserId(User);
                await _logService.LogAsync(identityUserId, $"Deleted category: {category.CategoryName} and all related data (equipment, rental requests, transactions, return infos)", "CategoriesController");

                TempData["DeleteMessage"] = $"Category '{category.CategoryName}' and all related equipment, rental requests, transactions, and return infos were deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }


        private bool IsAdminOrManager()
        {
            if (!User.Identity.IsAuthenticated) return false;

            var identityUser = _userManager.GetUserAsync(User).Result;
            if (identityUser?.MainUserId == null) return false;

            var user = _context.Users.FirstOrDefault(u => u.UserId == identityUser.MainUserId);
            return user != null && (user.RoleId == 1 || user.RoleId == 3); // Admin or Manager
        }

    }
}
