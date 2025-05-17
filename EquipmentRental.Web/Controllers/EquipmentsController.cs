// EquipmentController.cs (with role-based access)
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using ProjectDBClassLibrary.Model;
using AdvancedProgrammingASPProject.Areas.Identity.Data;

namespace AdvancedProgrammingASPProject.Controllers
{
    public class EquipmentsController : Controller
    {
        private readonly ProjectDBContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly LogService _logService;
        private readonly UserManager<Users> _userManager;

        public EquipmentsController(ProjectDBContext context, IWebHostEnvironment webHostEnvironment, LogService logService, UserManager<Users> userManager)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _logService = logService;
            _userManager = userManager;
        }

        private bool IsAdminOrManager()
        {
            if (!User.Identity.IsAuthenticated) return false;

            var identityUser = _userManager.GetUserAsync(User).Result;
            if (identityUser?.MainUserId != null)
            {
                var mainUser = _context.Users.FirstOrDefault(u => u.UserId == identityUser.MainUserId);
                return mainUser != null && (mainUser.RoleId == 1 || mainUser.RoleId == 3);
            }
            return false;
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult ClearTempMessage()
        {
            TempData.Remove("CreateMessage");
            TempData.Remove("DeleteMessage");
            return Ok();
        }


        public async Task<IActionResult> Index(int? categoryFilter, string statusFilter, string searchName, int page = 1)
        {
            int pageSize = 15;


            // Auto-update availability based on quantity
            var allEquipments = await _context.Equipments.ToListAsync();
            foreach (var eq in allEquipments)
            {
                if (eq.Quantity > 0 && !eq.AvailabilityStatus)
                {
                    eq.AvailabilityStatus = true;
                    _context.Equipments.Update(eq);
                }
                else if (eq.Quantity == 0 && eq.AvailabilityStatus)
                {
                    eq.AvailabilityStatus = false;
                    _context.Equipments.Update(eq);
                }
            }

            await _context.SaveChangesAsync();

            IQueryable<Equipment> equipments = _context.Equipments.Include(e => e.Category);

            if (categoryFilter.HasValue)
                equipments = equipments.Where(e => e.CategoryId == categoryFilter.Value);

            if (!string.IsNullOrWhiteSpace(statusFilter))
            {
                if (statusFilter == "Available")
                    equipments = equipments.Where(e => e.AvailabilityStatus);
                else if (statusFilter == "Unavailable")
                    equipments = equipments.Where(e => !e.AvailabilityStatus);
            }

            if (!string.IsNullOrWhiteSpace(searchName))
                equipments = equipments.Where(e => e.Name.Contains(searchName));

            var totalItems = await equipments.CountAsync();
            var items = await equipments
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var categoryList = await _context.Categories
                .Select(c => new { c.CategoryId, c.CategoryName })
                .ToListAsync();

            ViewBag.CreateMessage = TempData["CreateMessage"];
            ViewBag.DeleteMessage = TempData["DeleteMessage"];

            ViewBag.CategoryFilter = new SelectList(categoryList, "CategoryId", "CategoryName", categoryFilter);
            ViewBag.StatusFilter = new SelectList(new[] { "Available", "Unavailable" }, statusFilter);
            ViewBag.SearchName = searchName;

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return View(items);
        }



        public IActionResult Create()
        {
            if (!IsAdminOrManager()) return Forbid();
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Equipment equipment, IFormFile ImageFile)
        {
            if (!IsAdminOrManager()) return Forbid();
            ModelState.Remove("Category");

            if (ModelState.IsValid)
            {
                // Auto-set availability based on quantity
                equipment.AvailabilityStatus = equipment.Quantity > 0;

                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadsFolder);
                    var uniqueFileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(fileStream);
                    }
                    equipment.ImagePath = uniqueFileName;
                }

                _context.Add(equipment);
                await _context.SaveChangesAsync();

                var identityUserId = _userManager.GetUserId(User);
                await _logService.LogAsync(identityUserId, "Created equipment: " + equipment.Name, "EquipmentsController");

                TempData["CreateMessage"] = "Equipment created successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", equipment.CategoryId);
            return View(equipment);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsAdminOrManager()) return Forbid();
            if (id == null) return NotFound();

            var equipment = await _context.Equipments.FindAsync(id);
            if (equipment == null) return NotFound();

            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", equipment.CategoryId);

            // Add this to pass existing image path back to the form (if no image uploaded again)
            ViewBag.ExistingImagePath = equipment.ImagePath;

            return View(equipment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Equipment equipment, IFormFile ImageFile)
        {
            if (!IsAdminOrManager()) return Forbid();

            ModelState.Remove("Category");
            ModelState.Remove("ImagePath");
            ModelState.Remove("ImageFile");

            if (id != equipment.EquipmentId) return NotFound();

            if (ModelState.IsValid)
            {
                var existingEquipment = await _context.Equipments
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.EquipmentId == id);

                if (existingEquipment == null) return NotFound();

                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadsFolder);
                    var uniqueFileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(fileStream);
                    }

                    if (!string.IsNullOrEmpty(existingEquipment.ImagePath))
                    {
                        var oldImagePath = Path.Combine(uploadsFolder, existingEquipment.ImagePath);
                        if (System.IO.File.Exists(oldImagePath))
                            System.IO.File.Delete(oldImagePath);
                    }

                    equipment.ImagePath = uniqueFileName;
                }
                else
                {
                    equipment.ImagePath = existingEquipment.ImagePath;
                }

                // Auto-update availability
                equipment.AvailabilityStatus = equipment.Quantity > 0;

                _context.Update(equipment);
                await _context.SaveChangesAsync();

                var identityUserId = _userManager.GetUserId(User);
                await _logService.LogAsync(identityUserId, $"Edited equipment: {equipment.Name}", "EquipmentsController");

                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", equipment.CategoryId);
            return View(equipment);
        }





        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsAdminOrManager()) return Forbid();
            if (id == null) return NotFound();

            var equipment = await _context.Equipments.Include(e => e.Category)
                .FirstOrDefaultAsync(m => m.EquipmentId == id);
            if (equipment == null) return NotFound();

            return View(equipment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsAdminOrManager()) return Forbid();

            var equipment = await _context.Equipments
                .Include(e => e.RentalRequests)
                    .ThenInclude(r => r.User)
                .Include(e => e.RentalRequests)
                    .ThenInclude(r => r.RentalTransactions)
                        .ThenInclude(t => t.ReturnInfos)
                .FirstOrDefaultAsync(e => e.EquipmentId == id);

            if (equipment != null)
            {
                //  Collect affected customer list (distinct)
                var affectedUsers = equipment.RentalRequests
                    .Select(r => r.User)
                    .Where(u => u != null)
                    .Distinct()
                    .ToList();

                //  For each user, create a personal notification
                foreach (var user in affectedUsers)
                {
                    var notification = new Notification
                    {
                        Type = "Equipment Deleted",
                        Message = $"The equipment '{equipment.Name}' has been deleted. Any associated rentals were also removed.",
                        Status = false
                    };

                    _context.Notifications.Add(notification);
                    await _context.SaveChangesAsync();

                    _context.NotificationUsers.Add(new NotificationUser
                    {
                        NotificationId = notification.NotificationId,
                        UserId = user.UserId
                    });
                }

                //  Cascade delete rental data
                foreach (var request in equipment.RentalRequests)
                {
                    foreach (var transaction in request.RentalTransactions)
                    {
                        _context.ReturnInfos.RemoveRange(transaction.ReturnInfos);
                    }
                    _context.RentalTransactions.RemoveRange(request.RentalTransactions);
                }
                _context.RentalRequests.RemoveRange(equipment.RentalRequests);

                //  Delete equipment image if exists
                if (!string.IsNullOrEmpty(equipment.ImagePath))
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    var imagePath = Path.Combine(uploadsFolder, equipment.ImagePath);
                    if (System.IO.File.Exists(imagePath))
                        System.IO.File.Delete(imagePath);
                }

                _context.Equipments.Remove(equipment);
                await _context.SaveChangesAsync();

                var identityUserId = _userManager.GetUserId(User);
                await _logService.LogAsync(identityUserId, $"Deleted equipment: {equipment.Name}", "EquipmentsController");

                TempData["DeleteMessage"] = "Equipment deleted successfully.";
            }

            return RedirectToAction(nameof(Index));
        }





        private bool EquipmentExists(int id) => _context.Equipments.Any(e => e.EquipmentId == id);
    }
}
