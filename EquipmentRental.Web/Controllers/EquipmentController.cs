using EquipmentRental.DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class EquipmentController : Controller
{
    private readonly EquipmentRentalDbContext _context;

    public EquipmentController(EquipmentRentalDbContext context)
    {
        _context = context;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index(string? search, int? categoryId)
    {
        ViewBag.Categories = await _context.Categories.ToListAsync();

        var query = _context.Equipment.Include(e => e.Category).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(e => e.Name.Contains(search) || e.Description.Contains(search));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(e => e.CategoryId == categoryId);
        }

        var equipmentList = await query.ToListAsync();
        return View(equipmentList);
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        // Get user info
        var userEmail = User.Identity.IsAuthenticated
            ? User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
            : null;
        bool canLeaveFeedback = false;

        if (User.IsInRole("Customer") && userEmail != null)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

            // Check: Did the user rent this equipment before?
            bool hasRented = await _context.RentalTransactions
                .AnyAsync(rt => rt.CustomerId == user.UserId && rt.EquipmentId == id);

            // Check: Did the user already leave feedback for this equipment?
            bool alreadyLeftFeedback = await _context.Feedbacks
                .AnyAsync(f => f.UserId == user.UserId && f.EquipmentId == id);

            canLeaveFeedback = hasRented && !alreadyLeftFeedback;
        }

        ViewBag.CanLeaveFeedback = canLeaveFeedback;

        // Get equipment (with category)
        var equipment = await _context.Equipment
            .Include(e => e.Category)
            .FirstOrDefaultAsync(e => e.EquipmentId == id);

        if (equipment == null)
        {
            return NotFound();
        }

        // Get feedbacks for this equipment, only visible ones for regular users, all for admins/managers
        var userRole = User.IsInRole("Administrator") || User.IsInRole("Manager") ? "Admin" : "Customer";
        IQueryable<Feedback> feedbacksQuery = _context.Feedbacks
            .Where(f => f.EquipmentId == id)
            .Include(f => f.User)
            .OrderByDescending(f => f.CreatedAt);

        if (userRole == "Customer")
        {
            feedbacksQuery = feedbacksQuery.Where(f => f.IsVisible);
        }

        var feedbacks = await feedbacksQuery.ToListAsync();

        ViewBag.Feedbacks = feedbacks;

        return View(equipment);
    }



    [Authorize(Roles = "Administrator,Manager")]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await _context.Categories.ToListAsync();
        return View();
    }
    
    // Test action to directly insert a record
    [HttpGet]
    public async Task<IActionResult> TestInsert()
    {
        try
        {
            // Create a test equipment record
            var equipment = new Equipment
            {
                Name = "Test Equipment",
                Description = "Test Description",
                CategoryId = 1, // Make sure this category exists
                RentalPrice = 10.00m,
                AvailabilityStatus = "Available",
                ConditionStatus = "New",
                CreatedAt = DateTime.Now
            };
            
            // Add to database
            _context.Equipment.Add(equipment);
            var affected = await _context.SaveChangesAsync();
            
            return Content($"Successfully inserted test equipment. Affected rows: {affected}");
        }
        catch (Exception ex)
        {
            return Content($"Error inserting test equipment: {ex.Message}\n{ex.StackTrace}");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrator,Manager")]
    public async Task<IActionResult> Create(string Name, string Description, int CategoryId, decimal RentalPrice, string AvailabilityStatus, string ConditionStatus)
    {
        // Log form submission values for debugging
        Console.WriteLine("== Form Submission Received (Direct Parameters) ==");
        Console.WriteLine($"Name: {Name}");
        Console.WriteLine($"Description: {Description}");
        Console.WriteLine($"CategoryId: {CategoryId}");
        Console.WriteLine($"RentalPrice: {RentalPrice}");
        Console.WriteLine($"AvailabilityStatus: {AvailabilityStatus}");
        Console.WriteLine($"ConditionStatus: {ConditionStatus}");

        // Basic validation
        if (string.IsNullOrWhiteSpace(Name))
        {
            ModelState.AddModelError("Name", "Name is required");
        }

        if (CategoryId <= 0)
        {
            ModelState.AddModelError("CategoryId", "Please select a valid category");
        }

        if (RentalPrice <= 0)
        {
            ModelState.AddModelError("RentalPrice", "Rental price must be greater than zero");
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            var equipment = new Equipment
            {
                Name = Name,
                Description = Description,
                CategoryId = CategoryId,
                RentalPrice = RentalPrice,
                AvailabilityStatus = AvailabilityStatus,
                ConditionStatus = ConditionStatus
            };
            return View(equipment);
        }

        try
        {
            // Create a new equipment object directly
            var equipment = new Equipment
            {
                Name = Name,
                Description = Description,
                CategoryId = CategoryId,
                RentalPrice = RentalPrice,
                AvailabilityStatus = string.IsNullOrEmpty(AvailabilityStatus) ? "Available" : AvailabilityStatus,
                ConditionStatus = string.IsNullOrEmpty(ConditionStatus) ? "New" : ConditionStatus,
                CreatedAt = DateTime.Now
            };

            // Save to database
            _context.Equipment.Add(equipment);
            int affected = await _context.SaveChangesAsync();
            Console.WriteLine($"✅ Equipment saved successfully. Affected rows: {affected}");

            // Redirect with success message
            TempData["Success"] = "Equipment added successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            // Log the exception
            Console.WriteLine($"❌ Error saving equipment: {ex.Message}");
            Console.WriteLine($"❌ Stack trace: {ex.StackTrace}");
            
            // Add error message and return to form
            ModelState.AddModelError("", $"Error saving equipment: {ex.Message}");
            ViewBag.Categories = await _context.Categories.ToListAsync();
            
            var equipment = new Equipment
            {
                Name = Name,
                Description = Description,
                CategoryId = CategoryId,
                RentalPrice = RentalPrice,
                AvailabilityStatus = AvailabilityStatus,
                ConditionStatus = ConditionStatus
            };
            return View(equipment);
        }
    }




    [Authorize(Roles = "Administrator,Manager")]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var equipment = await _context.Equipment.FindAsync(id);
        if (equipment == null)
        {
            return NotFound();
        }

        ViewBag.Categories = await _context.Categories.ToListAsync();
        return View(equipment);
    }

    [Authorize(Roles = "Administrator,Manager")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int EquipmentId, string Name, string Description, int CategoryId, decimal RentalPrice, string AvailabilityStatus, string ConditionStatus)
    {
        // Log form submission values for debugging
        Console.WriteLine("== Edit Form Submission Received (Direct Parameters) ==");
        Console.WriteLine($"EquipmentId: {EquipmentId}");
        Console.WriteLine($"Name: {Name}");
        Console.WriteLine($"Description: {Description}");
        Console.WriteLine($"CategoryId: {CategoryId}");
        Console.WriteLine($"RentalPrice: {RentalPrice}");
        Console.WriteLine($"AvailabilityStatus: {AvailabilityStatus}");
        Console.WriteLine($"ConditionStatus: {ConditionStatus}");

        // Find the existing equipment
        var existingEquipment = await _context.Equipment.FindAsync(EquipmentId);
        if (existingEquipment == null)
        {
            return NotFound();
        }

        // Basic validation
        if (string.IsNullOrWhiteSpace(Name))
        {
            ModelState.AddModelError("Name", "Name is required");
        }

        if (CategoryId <= 0)
        {
            ModelState.AddModelError("CategoryId", "Please select a valid category");
        }

        if (RentalPrice <= 0)
        {
            ModelState.AddModelError("RentalPrice", "Rental price must be greater than zero");
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            var equipment = new Equipment
            {
                EquipmentId = EquipmentId,
                Name = Name,
                Description = Description,
                CategoryId = CategoryId,
                RentalPrice = RentalPrice,
                AvailabilityStatus = AvailabilityStatus,
                ConditionStatus = ConditionStatus,
                CreatedAt = existingEquipment.CreatedAt
            };
            return View(equipment);
        }

        try
        {
            // Update the existing equipment
            existingEquipment.Name = Name;
            existingEquipment.Description = Description;
            existingEquipment.CategoryId = CategoryId;
            existingEquipment.RentalPrice = RentalPrice;
            existingEquipment.AvailabilityStatus = string.IsNullOrEmpty(AvailabilityStatus) ? "Available" : AvailabilityStatus;
            existingEquipment.ConditionStatus = string.IsNullOrEmpty(ConditionStatus) ? "New" : ConditionStatus;
            
            // Save to database
            _context.Equipment.Update(existingEquipment);
            int affected = await _context.SaveChangesAsync();
            Console.WriteLine($"✅ Equipment updated successfully. Affected rows: {affected}");

            // Redirect with success message
            TempData["Success"] = "Equipment updated successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            // Log the exception
            Console.WriteLine($"❌ Error updating equipment: {ex.Message}");
            Console.WriteLine($"❌ Stack trace: {ex.StackTrace}");
            
            // Add error message and return to form
            ModelState.AddModelError("", $"Error updating equipment: {ex.Message}");
            ViewBag.Categories = await _context.Categories.ToListAsync();
            
            var equipment = new Equipment
            {
                EquipmentId = EquipmentId,
                Name = Name,
                Description = Description,
                CategoryId = CategoryId,
                RentalPrice = RentalPrice,
                AvailabilityStatus = AvailabilityStatus,
                ConditionStatus = ConditionStatus,
                CreatedAt = existingEquipment.CreatedAt
            };
            return View(equipment);
        }
    }

    [Authorize(Roles = "Administrator,Manager")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var equipment = await _context.Equipment.FindAsync(id);
        if (equipment == null)
        {
            return NotFound();
        }

        _context.Equipment.Remove(equipment);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Equipment deleted successfully!";
        return RedirectToAction("Index");
    }
}
