using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EquipmentRental.DataAccess.Models;
using Microsoft.AspNetCore.Authorization;

namespace EquipmentRental.Web.Controllers
{
    [Authorize(Roles = "Customer,Manager,Administrator")]
    public class FeedbacksController : Controller
    {
        private readonly EquipmentRentalDbContext _context;

        public FeedbacksController(EquipmentRentalDbContext context)
        {
            _context = context;
        }

        // GET: Feedbacks
        public async Task<IActionResult> Index()
        {
            var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail)) return Unauthorized();

            var user = await _context.Users.Include(u => u.Role)
                                           .FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null) return Unauthorized();

            var feedbacks = _context.Feedbacks
                .Include(f => f.Equipment)
                .Include(f => f.User)
                .AsQueryable(); 

            if (user.Role.RoleName == "Customer")
            {
                feedbacks = feedbacks.Where(f => f.IsVisible == true);
            }

            return View(await feedbacks.ToListAsync());
        }


        // GET: Feedbacks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Feedbacks == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedbacks
                .Include(f => f.Equipment)
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.FeedbackId == id);
            if (feedback == null)
            {
                return NotFound();
            }

            return View(feedback);
        }

        // GET: Feedbacks/Create
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Create()
        {
            var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
                return Unauthorized();

            // Equipment rented by user
            var rentedEquipmentIds = await _context.RentalTransactions
                .Where(rt => rt.CustomerId == user.UserId)
                .Select(rt => rt.EquipmentId)
                .Distinct()
                .ToListAsync();

            // Equipment already reviewed by user
            var feedbackEquipmentIds = await _context.Feedbacks
                .Where(f => f.UserId == user.UserId)
                .Select(f => f.EquipmentId)
                .Distinct()
                .ToListAsync();

            // Only show rented equipment NOT already reviewed
            var equipmentList = _context.Equipment
                .Where(eq => rentedEquipmentIds.Contains(eq.EquipmentId) && !feedbackEquipmentIds.Contains(eq.EquipmentId))
                .Select(eq => new { eq.EquipmentId, eq.Name })
                .ToList();

            ViewBag.EquipmentList = new SelectList(equipmentList, "EquipmentId", "Name");

            return View();
        }



        // POST: Feedbacks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Create([Bind("EquipmentId,CommentText,Rating")] Feedback feedback)
        {
            var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
                return Unauthorized();

            // Validate: Only allow feedback for rented equipment not already reviewed
            bool hasRented = await _context.RentalTransactions
                .AnyAsync(rt => rt.CustomerId == user.UserId && rt.EquipmentId == feedback.EquipmentId);

            bool alreadyReviewed = await _context.Feedbacks
                .AnyAsync(f => f.UserId == user.UserId && f.EquipmentId == feedback.EquipmentId);

            if (!hasRented || alreadyReviewed)
            {
                ModelState.AddModelError("EquipmentId", "You can only leave feedback for equipment you have rented and not already reviewed.");
            }

            if (ModelState.IsValid)
            {
                feedback.UserId = user.UserId;
                feedback.CreatedAt = DateTime.Now;
                feedback.IsVisible = true;

                _context.Add(feedback);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Repopulate dropdown if error
            var rentedEquipmentIds = await _context.RentalTransactions
                .Where(rt => rt.CustomerId == user.UserId)
                .Select(rt => rt.EquipmentId)
                .Distinct()
                .ToListAsync();

            var feedbackEquipmentIds = await _context.Feedbacks
                .Where(f => f.UserId == user.UserId)
                .Select(f => f.EquipmentId)
                .Distinct()
                .ToListAsync();

            var equipmentList = _context.Equipment
                .Where(eq => rentedEquipmentIds.Contains(eq.EquipmentId) && !feedbackEquipmentIds.Contains(eq.EquipmentId))
                .Select(eq => new { eq.EquipmentId, eq.Name })
                .ToList();

            ViewBag.EquipmentList = new SelectList(equipmentList, "EquipmentId", "Name", feedback.EquipmentId);

            return View(feedback);
        }


        // GET: Feedbacks/Edit/5
        [Authorize(Roles = "Manager,Administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Feedbacks == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedbacks
                .Include(f => f.Equipment)
                .Include(f => f.User)
                .FirstOrDefaultAsync(f => f.FeedbackId == id);
            if (feedback == null)
            {
                return NotFound();
            }
            return View(feedback);
        }

        // POST: Feedbacks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Manager,Administrator")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FeedbackId,EquipmentId,UserId,CommentText,Rating,CreatedAt,IsVisible")] Feedback feedback)
        {
            if (id != feedback.FeedbackId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(feedback);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FeedbackExists(feedback.FeedbackId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EquipmentId"] = new SelectList(_context.Equipment, "EquipmentId", "AvailabilityStatus", feedback.EquipmentId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", feedback.UserId);
            return View(feedback);
        }

        // GET: Feedbacks/Delete/5
        [Authorize(Roles = "Manager,Administrator")]

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Feedbacks == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedbacks
                .Include(f => f.Equipment)
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.FeedbackId == id);
            if (feedback == null)
            {
                return NotFound();
            }

            return View(feedback);
        }

        // POST: Feedbacks/Delete/5
        [Authorize(Roles = "Manager,Administrator")]

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Feedbacks == null)
            {
                return Problem("Entity set 'EquipmentRentalDbContext.Feedbacks'  is null.");
            }
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback != null)
            {
                _context.Feedbacks.Remove(feedback);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FeedbackExists(int id)
        {
          return (_context.Feedbacks?.Any(e => e.FeedbackId == id)).GetValueOrDefault();
        }
        
        // GET: Feedbacks/MyFeedback
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> MyFeedback()
        {
            try
            {
                // Get current user ID
                var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
                
                // Get all feedback submitted by this user
                var feedbacks = await _context.Feedbacks
                    .Include(f => f.Equipment)
                    .ThenInclude(e => e.Category)
                    .Include(f => f.User)
                    .Where(f => f.UserId == userId)
                    .OrderByDescending(f => f.CreatedAt)
                    .ToListAsync();
                
                ViewBag.IsCustomerView = true;
                return View(feedbacks);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred: {ex.Message}";
                return View(new List<Feedback>());
            }
        }
    }
}
