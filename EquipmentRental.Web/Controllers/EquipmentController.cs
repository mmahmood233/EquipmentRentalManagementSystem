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
    public async Task<IActionResult> Index(string search, int? categoryId)
    {
        ViewBag.Categories = await _context.Categories.ToListAsync();

        var equipmentQuery = _context.Equipment.Include(e => e.Category).AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            equipmentQuery = equipmentQuery.Where(e =>
                e.Name.Contains(search) || e.Description.Contains(search));
        }

        if (categoryId.HasValue)
        {
            equipmentQuery = equipmentQuery.Where(e => e.CategoryId == categoryId.Value);
        }

        var equipmentList = await equipmentQuery.ToListAsync();
        return View(equipmentList);
    }


    [Authorize(Roles = "Administrator,Manager")]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Categories = await _context.Categories.ToListAsync();
        return View();
    }

    [Authorize(Roles = "Administrator,Manager")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Equipment equipment)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(equipment);
        }

        equipment.CreatedAt = DateTime.Now;
        _context.Equipment.Add(equipment);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Equipment added successfully!";
        return RedirectToAction("Index");
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
    public async Task<IActionResult> Edit(Equipment equipment)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(equipment);
        }

        _context.Equipment.Update(equipment);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Equipment updated successfully!";
        return RedirectToAction("Index");
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
