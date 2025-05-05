using System.Text;
using EquipmentRental.DataAccess.Models;
using EquipmentRental.Web.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Security.Claims;

[Authorize(Roles = "Administrator")]
public class AdminController : Controller
{
    private readonly EquipmentRentalDbContext _context;

    public AdminController(EquipmentRentalDbContext context)
    {
        _context = context;
    }

    // Admin dashboard with system statistics
    public async Task<IActionResult> Dashboard()
    {
        // Get system statistics
        ViewBag.UserCount = await _context.Users.CountAsync();
        ViewBag.EquipmentCount = await _context.Equipment.CountAsync();
        ViewBag.RentalRequestCount = await _context.RentalRequests.CountAsync();
        ViewBag.CategoryCount = await _context.Categories.CountAsync();
        
        // Get recent activity logs (top 5)
        var recentLogs = await _context.Logs
            .OrderByDescending(l => l.Timestamp)
            .Take(5)
            .ToListAsync() ?? new List<Log>();

        try
        {
            // Load user information for logs that have a valid UserId
            var userIds = recentLogs.Where(l => l.UserId.HasValue).Select(l => l.UserId.Value).Distinct().ToList();
            if (userIds.Any())
            {
                var users = await _context.Users.Where(u => userIds.Contains(u.UserId)).ToDictionaryAsync(u => u.UserId, u => u);
                foreach (var log in recentLogs.Where(l => l.UserId.HasValue))
                {
                    if (users.TryGetValue(log.UserId.Value, out var user))
                    {
                        log.User = user;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            // Log the exception but continue - don't let user loading issues break the dashboard
            System.Diagnostics.Debug.WriteLine($"Error loading users for logs: {ex.Message}");
        }
        
        ViewBag.RecentLogs = recentLogs;
            
        return View();
    }

    // View audit logs
    public IActionResult Logs()
    {
        // Get all logs directly from the database
        var logs = _context.Logs.ToList();

        // Try to load user information for logs
        var users = _context.Users.ToDictionary(u => u.UserId);
        
        // Set user information for each log if available
        foreach (var log in logs)
        {
            if (log.UserId.HasValue && users.ContainsKey(log.UserId.Value))
            {
                log.User = users[log.UserId.Value];
            }
        }
        
        // Sort logs by timestamp (newest first)
        logs = logs.OrderByDescending(l => l.Timestamp).ToList();
        
        return View(logs);
    }



    // Manage user list with search functionality
    public async Task<IActionResult> ManageUsers(string search = null, int? role = null)
    {
        // Start with base query
        var query = _context.Users
            .Include(u => u.Role)
            .AsQueryable();

        // Apply search filter if provided
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(u => 
                u.FullName.Contains(search) || 
                u.Email.Contains(search));
        }

        // Apply role filter if provided
        if (role.HasValue)
        {
            query = query.Where(u => u.RoleId == role.Value);
        }

        // Get all roles for the dropdown
        ViewBag.Roles = await _context.UserRoles.ToListAsync();

        // Get the users
        var users = await query.OrderBy(u => u.FullName).ToListAsync();
        return View(users);
    }

    // GET: Edit a user's role
    public async Task<IActionResult> EditRole(int id)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.UserId == id);

        if (user == null)
            return NotFound();

        var viewModel = new EditUserRoleViewModel
        {
            UserId = user.UserId,
            FullName = user.FullName,
            Email = user.Email,
            SelectedRoleId = user.RoleId,
            AvailableRoles = await _context.UserRoles.ToListAsync()
        };

        return View(viewModel);
    }

    // POST: Save role changes
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditRole(EditUserRoleViewModel model)
    {
        var user = await _context.Users.FindAsync(model.UserId);
        if (user == null)
            return NotFound();

        user.RoleId = model.SelectedRoleId;
        await _context.SaveChangesAsync();

        return RedirectToAction("ManageUsers");
    }

    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> ResetPassword(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        return View(new ResetUserPasswordViewModel
        {
            UserId = user.UserId,
            FullName = user.FullName,
            Email = user.Email
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> ResetPassword(ResetUserPasswordViewModel model)
    {
        if (model.NewPassword != model.ConfirmPassword)
        {
            ModelState.AddModelError("", "Passwords do not match.");
            return View(model);
        }

        var user = await _context.Users.FindAsync(model.UserId);
        if (user == null) return NotFound();

        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(model.NewPassword));
        user.PasswordHash = Convert.ToBase64String(bytes);

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Password reset successfully.";
        return RedirectToAction("ManageUsers");
    }

    // Toggle user active status (activate/deactivate)
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> ToggleUserStatus(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        // Toggle the IsActive status
        user.IsActive = !user.IsActive;
        
        // Add an audit log entry
        var logEntry = new Log
        {
            UserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value),
            Action = user.IsActive == true ? "Activate" : "Deactivate",
            Source = "User Management",
            Exception = $"User {user.FullName} ({user.Email}) {(user.IsActive == true ? "activated" : "deactivated")}",
            Timestamp = DateTime.Now
        };
        _context.Logs.Add(logEntry);

        // Save changes
        await _context.SaveChangesAsync();

        // Set success message
        TempData["Success"] = $"User {user.FullName} has been {(user.IsActive == true ? "activated" : "deactivated")} successfully.";
        
        return RedirectToAction("ManageUsers");
    }
}
