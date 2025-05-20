using EquipmentRental.DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Diagnostics;

[Authorize]
public class RentalRequestController : Controller
{
    private readonly EquipmentRentalDbContext _context;
    private readonly NotificationService _notificationService;

    public RentalRequestController(EquipmentRentalDbContext context, NotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    // GET: /RentalRequest/Details/5
    [Authorize]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var rentalRequest = await _context.RentalRequests
            .Include(r => r.Customer)
            .Include(r => r.Equipment)
            .FirstOrDefaultAsync(m => m.RentalRequestId == id);

        if (rentalRequest == null)
        {
            return NotFound();
        }

        // Check authorization - customers can only view their own requests
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var isManager = User.IsInRole("Manager") || User.IsInRole("Administrator") || User.IsInRole("Admin");

        if (!isManager && rentalRequest.CustomerId != userId)
        {
            return Forbid();
        }

        return View(rentalRequest);
    }

    // GET: /RentalRequest
    public async Task<IActionResult> Index(string search = null, string status = null)
    {
        // Get current user info
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "Unknown";
        var isCustomer = User.IsInRole("Customer");
        var isManager = User.IsInRole("Manager") || User.IsInRole("Administrator") || User.IsInRole("Admin");
        
        Debug.WriteLine($"User ID: {userId}, Role: {userRole}, IsCustomer: {isCustomer}, IsManager: {isManager}");

        // Start with base query
        var query = _context.RentalRequests
            .Include(r => r.Equipment)
            .Include(r => r.Customer)
            .AsQueryable();

        // Log the total number of rental requests in the database
        var totalRequests = await _context.RentalRequests.CountAsync();
        Debug.WriteLine($"Total rental requests in database: {totalRequests}");

        // Apply role-based filtering - modified to ensure all users can see their own requests
        if (!isManager) // If not a manager, only show own requests regardless of role
        {
            // All non-managers can only see their own requests
            query = query.Where(r => r.CustomerId == userId);
            Debug.WriteLine($"Filtering for user {userId}'s requests only (not a manager)");
        }
        else
        {
            Debug.WriteLine("No user-specific filtering applied (user is a manager)");
        }

        // Apply search filter if provided
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(r => 
                r.Equipment.Name.Contains(search) || 
                r.Description.Contains(search));
        }

        // Apply status filter if provided
        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(r => r.Status == status);
        }

        // Get final results
        var list = await query.OrderByDescending(r => r.CreatedAt).ToListAsync();
        
        // Log the number of results for debugging
        Debug.WriteLine($"Found {list.Count} rental requests");
        
        return View(list);
    }

    // GET: /RentalRequest/Create
    [Authorize] // Allow any authenticated user to create rental requests
    public async Task<IActionResult> Create()
    {
        // Only show available equipment
        ViewBag.EquipmentList = await _context.Equipment
            .Where(e => e.AvailabilityStatus == "Available")
            .ToListAsync();
            
        Debug.WriteLine($"Found {ViewBag.EquipmentList.Count} available equipment items");
        return View();
    }

    // POST: /RentalRequest/Create
    [Authorize] // Allow any authenticated user to create rental requests
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int EquipmentId, DateTime RentalStartDate, DateTime RentalEndDate, decimal TotalCost, string Description)
    {
        // Log form submission values for debugging
        Debug.WriteLine("== RentalRequest Form Submission Received ==");
        Debug.WriteLine($"EquipmentId: {EquipmentId}");
        Debug.WriteLine($"RentalStartDate: {RentalStartDate}");
        Debug.WriteLine($"RentalEndDate: {RentalEndDate}");
        Debug.WriteLine($"TotalCost: {TotalCost}");
        Debug.WriteLine($"Description: {Description}");

        // Basic validation
        if (EquipmentId <= 0)
        {
            ModelState.AddModelError("EquipmentId", "Please select an equipment");
        }
        else
        {
            // Verify equipment exists and is available
            var equipment = await _context.Equipment.FindAsync(EquipmentId);
            if (equipment == null)
            {
                ModelState.AddModelError("EquipmentId", "Selected equipment does not exist");
            }
            else if (equipment.AvailabilityStatus != "Available")
            {
                ModelState.AddModelError("EquipmentId", "Selected equipment is not available for rental");
            }
        }

        if (RentalStartDate == default)
        {
            ModelState.AddModelError("RentalStartDate", "Start date is required");
        }
        else if (RentalStartDate < DateTime.Today)
        {
            ModelState.AddModelError("RentalStartDate", "Start date cannot be in the past");
        }

        if (RentalEndDate == default)
        {
            ModelState.AddModelError("RentalEndDate", "End date is required");
        }
        else if (RentalEndDate < RentalStartDate)
        {
            ModelState.AddModelError("RentalEndDate", "End date must be after start date");
        }

        if (TotalCost <= 0)
        {
            ModelState.AddModelError("TotalCost", "Total cost must be greater than zero");
        }

        if (!ModelState.IsValid)
        {
            ViewBag.EquipmentList = await _context.Equipment.Where(e => e.AvailabilityStatus == "Available").ToListAsync();
            var model = new RentalRequest
            {
                EquipmentId = EquipmentId,
                RentalStartDate = RentalStartDate,
                RentalEndDate = RentalEndDate,
                TotalCost = TotalCost,
                Description = Description
            };
            return View(model);
        }

        try
        {
            // Get current user ID (customer)
            var customerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            
            // Create a new rental request
            var rentalRequest = new RentalRequest
            {
                CustomerId = customerId,
                EquipmentId = EquipmentId,
                RentalStartDate = RentalStartDate,
                RentalEndDate = RentalEndDate,
                TotalCost = TotalCost,
                Description = Description,
                Status = "Pending",
                CreatedAt = DateTime.Now
            };

            // Save to database
            _context.RentalRequests.Add(rentalRequest);
            int affected = await _context.SaveChangesAsync();
            Debug.WriteLine($"✅ Rental request saved successfully. Affected rows: {affected}");

            // Redirect with success message
            TempData["Success"] = "Rental request submitted successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            // Log the exception
            Debug.WriteLine($"❌ Error saving rental request: {ex.Message}");
            Debug.WriteLine($"❌ Stack trace: {ex.StackTrace}");
            
            // Add error message and return to form
            ModelState.AddModelError("", $"Error submitting rental request: {ex.Message}");
            ViewBag.EquipmentList = await _context.Equipment.Where(e => e.AvailabilityStatus == "Available").ToListAsync();
            
            var model = new RentalRequest
            {
                EquipmentId = EquipmentId,
                RentalStartDate = RentalStartDate,
                RentalEndDate = RentalEndDate,
                TotalCost = TotalCost,
                Description = Description
            };
            return View(model);
        }
    }
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var request = await _context.RentalRequests
            .Include(r => r.Equipment)
            .Include(r => r.Customer)
            .FirstOrDefaultAsync(r => r.RentalRequestId == id);

        if (request == null)
        {
            TempData["Error"] = "Rental request not found.";
            return RedirectToAction(nameof(Index));
        }

        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var isCustomer = User.IsInRole("Customer");
        var isManager = User.IsInRole("Manager");

        Debug.WriteLine($"Edit request - User ID: {userId}, IsCustomer: {isCustomer}, IsManager: {isManager}");
        Debug.WriteLine($"Request ID: {id}, Status: {request.Status}, CustomerId: {request.CustomerId}");

        // Access control
        // 1. Managers can edit any request
        // 2. Customers can only edit their own requests
        if (isCustomer && !isManager && request.CustomerId != userId)
        {
            Debug.WriteLine("Access denied: Customer trying to edit someone else's request");
            TempData["Error"] = "You can only edit your own rental requests.";
            return RedirectToAction(nameof(Index));
        }

        // 3. Customers can only edit if status is still Pending
        if (isCustomer && !isManager && request.Status != "Pending")
        {
            Debug.WriteLine("Access denied: Customer trying to edit a non-pending request");
            TempData["Error"] = "You can only edit requests that are still pending.";
            return RedirectToAction(nameof(Index));
        }

        // Prepare view data
        ViewBag.AvailableEquipment = await _context.Equipment
            .Where(e => e.AvailabilityStatus == "Available" || e.EquipmentId == request.EquipmentId)
            .ToListAsync();
        ViewBag.IsManager = isManager;
        ViewBag.IsCustomer = isCustomer;
        ViewBag.EditableStatuses = new[] { "Pending", "Approved", "Rejected", "Confirmed", "Picked-up", "Returned" };

        return View(request);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Edit(int RentalRequestId, string Description, string Status)
    {
        // Log form submission values for debugging
        Debug.WriteLine("== RentalRequest Edit Form Submission Received ==");
        Debug.WriteLine($"RentalRequestId: {RentalRequestId}");
        Debug.WriteLine($"Description: {Description}");
        Debug.WriteLine($"Status: {Status}");

        // Find the existing rental request
        var existingRequest = await _context.RentalRequests
            .Include(r => r.Equipment)
            .Include(r => r.Customer)
            .FirstOrDefaultAsync(r => r.RentalRequestId == RentalRequestId);

        if (existingRequest == null)
        {
            return NotFound();
        }

        // Get current user info
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var isManager = User.IsInRole("Manager") || User.IsInRole("Administrator") || User.IsInRole("Admin");

        // Check if user is authorized to edit this request
        if (!isManager && existingRequest.CustomerId != userId)
        {
            return Forbid();
        }
        
        // For customers, only allow description updates if request is still pending
        if (!isManager && existingRequest.CustomerId == userId)
        {
            // Check if request status allows customer updates
            string currentStatus = existingRequest.Status?.ToLower() ?? "";
            bool canUpdate = currentStatus == "pending" || currentStatus == "" || currentStatus == "draft";
            
            if (!canUpdate)
            {
                TempData["Error"] = "You can only update the description of pending requests.";
                return RedirectToAction(nameof(Index));
            }
            
            // Customers can only update the description
            existingRequest.Description = Description;
            _context.Update(existingRequest);
            await _context.SaveChangesAsync();
            
            TempData["Success"] = "Request description updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            // For managers, allow full updates
            if (isManager)
            {
                // Update the existing rental request
                existingRequest.Description = Description;
                
                // Only managers can change status
                if (!string.IsNullOrEmpty(Status))
                {
                    existingRequest.Status = Status;
                    
                    // If status is changed to approved, update equipment availability
                    if (Status.ToLower() == "approved")
                    {
                        var equipment = await _context.Equipment.FindAsync(existingRequest.EquipmentId);
                        if (equipment != null)
                        {
                            equipment.AvailabilityStatus = "Reserved";
                            _context.Update(equipment);
                        }
                    }
                }
                
                // Save to database
                _context.Update(existingRequest);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Rental request updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            
            // This code should not be reached for customers as we handle their updates earlier
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating rental request: {ex.Message}");
            TempData["Error"] = $"Error updating rental request: {ex.Message}";
            
            // Redirect back to edit form
            return RedirectToAction(nameof(Edit), new { id = RentalRequestId });
        }
    }

    // GET: RentalRequest/Delete/5
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

        var request = await _context.RentalRequests
            .Include(r => r.Equipment)
            .Include(r => r.Customer)
            .FirstOrDefaultAsync(r => r.RentalRequestId == id);

        if (request == null) return NotFound();

        // Only allow owner or manager/admin
        bool isManagerOrAdmin = userRole == "Manager" || userRole == "Administrator" || userRole == "Admin";
        if (!isManagerOrAdmin && request.CustomerId != userId)
            return Forbid();

        return View(request);
    }

    // POST: RentalRequest/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

        var request = await _context.RentalRequests.FindAsync(id);
        if (request == null) return NotFound();

        bool isManagerOrAdmin = userRole == "Manager" || userRole == "Administrator" || userRole == "Admin";
        if (!isManagerOrAdmin && request.CustomerId != userId)
            return Forbid();

        _context.RentalRequests.Remove(request);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Rental Request deleted successfully.";
        return RedirectToAction("Index");
    }

    // GET: RentalRequest/Approve/5
    [Authorize(Roles = "Manager,Administrator,Admin")]
    public async Task<IActionResult> Approve(int id)
    {
        var request = await _context.RentalRequests
            .Include(r => r.Equipment)
            .FirstOrDefaultAsync(r => r.RentalRequestId == id);

        if (request == null) return NotFound();

        // Only pending requests can be approved
        if (request.Status != "Pending")
        {
            TempData["Error"] = "Only pending requests can be approved.";
            return RedirectToAction(nameof(Index));
        }

        // Update request status
        request.Status = "Approved";
        _context.Update(request);
        await _context.SaveChangesAsync();

        // Update equipment availability status
        var equipment = await _context.Equipment.FindAsync(request.EquipmentId);
        if (equipment != null)
        {
            equipment.AvailabilityStatus = "Reserved";
            _context.Update(equipment);
            await _context.SaveChangesAsync();
        }
        // Send notification to the customer
        await _notificationService.CreateNotification(
            request.CustomerId,
            $"Your rental request for {request.Equipment?.Name ?? "equipment"} has been approved!",
            "Rental Approved"
        );

        TempData["Success"] = "Rental request approved successfully.";
        return RedirectToAction(nameof(Index));
    }


    // GET: RentalRequest/Reject/5
    [Authorize(Roles = "Manager,Administrator,Admin")]
    public async Task<IActionResult> Reject(int id)
    {
        var request = await _context.RentalRequests.FindAsync(id);

        if (request == null) return NotFound();

        // Only pending requests can be rejected
        if (request.Status != "Pending")
        {
            TempData["Error"] = "Only pending requests can be rejected.";
            return RedirectToAction(nameof(Index));
        }

        // Update request status
        request.Status = "Rejected";
        _context.Update(request);
        await _context.SaveChangesAsync();
        var equipment = await _context.Equipment.FindAsync(request.EquipmentId);
        string equipmentName = equipment?.Name ?? "equipment";
        await _notificationService.CreateNotification(
            request.CustomerId,
            $"Your rental request for {equipmentName} was rejected.",
            "Rental Rejected"
        );

        TempData["Success"] = "Rental request rejected successfully.";
        return RedirectToAction(nameof(Index));
    }
}
