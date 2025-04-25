using System.Text;
using EquipmentRental.DataAccess.Models;
using EquipmentRental.Web.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;


[Authorize(Roles = "Administrator")]
public class AdminController : Controller
{
    private readonly EquipmentRentalDbContext _context;

    public AdminController(EquipmentRentalDbContext context)
    {
        _context = context;
    }

    // Admin dashboard (optional)
    public IActionResult Dashboard()
    {
        return View();
    }

    // View audit logs
    public IActionResult Logs()
    {
        var logs = _context.Logs
            .Include(l => l.User)
            .OrderByDescending(l => l.Timestamp)
            .ToList();

        return View(logs);
    }

    // Manage user list
    public IActionResult ManageUsers()
    {
        var users = _context.Users.Include(u => u.Role).ToList();
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

}
