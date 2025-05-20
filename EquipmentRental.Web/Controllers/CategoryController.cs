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

        // Get categories with equipment included for counting
        var categoryList = await categories.Include(c => c.Equipment).ToListAsync();

        ViewBag.Search = search;
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
            TempData["Success"] = "Category created successfully!";
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
            try
            {
                _context.Categories.Update(category);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Category updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error updating category: {ex.Message}");
                return View(category);
            }
        }
        return View(category);
    }

    // VIEW EQUIPMENT BY CATEGORY
    public async Task<IActionResult> ViewEquipment(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return NotFound();

        var equipment = await _context.Equipment
            .Where(e => e.CategoryId == id)
            .ToListAsync();

        ViewBag.Category = category;
        return View(equipment);
    }

    // DELETE: only admins
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return NotFound();

        try
        {
            // Check if category is in use by any equipment
            var equipmentUsingCategory = await _context.Equipment.AnyAsync(e => e.CategoryId == id);
            if (equipmentUsingCategory)
            {
                TempData["Error"] = "Cannot delete this category because it is being used by equipment items.";
                return RedirectToAction(nameof(Index));
            }
            
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Category deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error deleting category: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }
}
