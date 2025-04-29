using EquipmentRental.DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize] // Only logged-in users can see categories
public class CategoryController : Controller
{
    private readonly EquipmentRentalDbContext _context;

    public CategoryController(EquipmentRentalDbContext context)
    {
        _context = context;
    }

    // LIST + SEARCH
    public async Task<IActionResult> Index(string? search)
    {
        var categories = from c in _context.Categories select c;

        if (!string.IsNullOrEmpty(search))
        {
            categories = categories.Where(c => c.CategoryName.Contains(search));
        }

        var categoryList = await categories.ToListAsync();
        return View(categoryList);
    }

    // CREATE: only admins
    [Authorize(Roles = "Administrator")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Category category)
    {
        if (ModelState.IsValid)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(category);
    }

    // EDIT: only admins
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Edit(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return NotFound();
        return View(category);
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Category category)
    {
        if (id != category.CategoryId) return NotFound();

        if (ModelState.IsValid)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(category);
    }

    // DELETE: only admins
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return NotFound();

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
