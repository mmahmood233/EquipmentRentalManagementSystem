using EquipmentRental.DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Administrator")]
public class AdminController : Controller
{
    private readonly EquipmentRentalDbContext _context;

    public AdminController(EquipmentRentalDbContext context)
    {
        _context = context;
    }

    public IActionResult Dashboard()
    {
        return View();
    }

    public IActionResult Logs()
    {
        var logs = _context.Logs
            .Include(l => l.User)
            .OrderByDescending(l => l.Timestamp)
            .ToList();

        return View(logs);
    }

    public IActionResult ManageUsers()
    {
        var users = _context.Users.Include(u => u.Role).ToList();
        return View(users);
    }


}
