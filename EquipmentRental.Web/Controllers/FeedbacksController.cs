using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EquipmentRental.DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace EquipmentRental.Web.Controllers
{
    [Authorize]
    public class FeedbacksController : Controller
    {
        private readonly EquipmentRentalDbContext _context;
        private readonly ILogger<FeedbacksController> _logger;

        public FeedbacksController(EquipmentRentalDbContext context, ILogger<FeedbacksController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Feedbacks - View all feedback for equipment
        public async Task<IActionResult> Index(int? equipmentId)
        {
            // Get current user ID
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var isAdmin = User.IsInRole("Administrator");
            var isManager = User.IsInRole("Manager");
            
            // Base query to get all feedbacks
            var query = _context.Feedbacks
                .Include(f => f.Equipment)
                .Include(f => f.User)
                .AsQueryable();
            
            // Filter by equipment if specified
            if (equipmentId.HasValue)
            {
                query = query.Where(f => f.EquipmentId == equipmentId);
                
                // Get equipment details for the view
                var equipment = await _context.Equipment
                    .FirstOrDefaultAsync(e => e.EquipmentId == equipmentId);
                    
                if (equipment != null)
                {
                    ViewBag.Equipment = equipment;
                }
            }
            
            // For non-admin/manager users, only show visible feedback
            if (!isAdmin && !isManager)
            {
                query = query.Where(f => f.IsVisible == true);
            }
            
            // Get all feedbacks based on the filters
            var feedbacks = await query.OrderByDescending(f => f.CreatedAt).ToListAsync();
            
            // Check if the user can add feedback (has rented this equipment before)
            bool canAddFeedback = false;
            if (equipmentId.HasValue)
            {
                canAddFeedback = await _context.RentalTransactions
                    .AnyAsync(rt => rt.EquipmentId == equipmentId && rt.CustomerId == userId);
            }
            
            ViewBag.CanAddFeedback = canAddFeedback;
            ViewBag.IsAdminOrManager = isAdmin || isManager;
            
            return View(feedbacks);
        }

        // GET: Feedbacks/Equipment - View all equipment with feedback counts
        public async Task<IActionResult> Equipment()
        {
            // Get equipment with feedback counts
            var equipmentWithFeedback = await _context.Equipment
                .Select(e => new EquipmentFeedbackViewModel
                {
                    Equipment = e,
                    FeedbackCount = _context.Feedbacks.Count(f => f.EquipmentId == e.EquipmentId && f.IsVisible == true),
                    AverageRating = _context.Feedbacks
                        .Where(f => f.EquipmentId == e.EquipmentId && f.IsVisible == true && f.Rating.HasValue)
                        .Average(f => f.Rating) ?? 0
                })
                .Where(e => e.FeedbackCount > 0)  // Only show equipment with feedback
                .OrderByDescending(e => e.AverageRating)
                .ToListAsync();
                
            return View(equipmentWithFeedback);
        }

        // GET: Feedbacks/MyFeedback - View feedback left by the current user
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> MyFeedback()
        {
            // Get current user ID
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            
            // Get all feedback left by the current user
            var feedbacks = await _context.Feedbacks
                .Include(f => f.Equipment)
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
                
            return View(feedbacks);
        }

        // GET: Feedbacks/Create/5 (equipment ID)
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Create(int? id, int? equipmentId)
        {
            // Handle both parameter names for compatibility
            int? equipmentIdToUse = id ?? equipmentId;
            
            if (equipmentIdToUse == null)
            {
                _logger.LogWarning("Create feedback attempted without equipment ID");
                return NotFound();
            }
            
            // Get current user ID
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            _logger.LogInformation("User {UserId} attempting to create feedback for equipment {EquipmentId}", userId, equipmentIdToUse);
            
            // Check if the user has rented this equipment
            var hasRented = await _context.RentalTransactions
                .AnyAsync(rt => rt.EquipmentId == equipmentIdToUse && rt.CustomerId == userId);
                
            if (!hasRented)
            {
                _logger.LogWarning("User {UserId} attempted to provide feedback for equipment {EquipmentId} they haven't rented", userId, equipmentIdToUse);
                TempData["Error"] = "You can only provide feedback for equipment you have rented.";
                return RedirectToAction(nameof(Index));
            }
            
            // Check if the user has already provided feedback for this equipment
            var existingFeedback = await _context.Feedbacks
                .FirstOrDefaultAsync(f => f.EquipmentId == equipmentIdToUse && f.UserId == userId);
                
            if (existingFeedback != null)
            {
                _logger.LogInformation("User {UserId} attempted to provide duplicate feedback for equipment {EquipmentId}", userId, equipmentIdToUse);
                TempData["Error"] = "You have already provided feedback for this equipment.";
                return RedirectToAction(nameof(Index), new { equipmentId = equipmentIdToUse });
            }
            
            // Get equipment details for the view
            var equipment = await _context.Equipment
                .Include(e => e.Category)
                .FirstOrDefaultAsync(e => e.EquipmentId == equipmentIdToUse);
                
            if (equipment == null)
            {
                _logger.LogWarning("Equipment with ID {EquipmentId} not found when creating feedback", equipmentIdToUse);
                return NotFound();
            }
            
            ViewBag.Equipment = equipment;
            
            // Create a new feedback object
            var feedback = new Feedback
            {
                EquipmentId = equipmentIdToUse.Value,
                UserId = userId,
                CreatedAt = DateTime.Now,
                IsVisible = true
            };
            
            return View(feedback);
        }

        // POST: Feedbacks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Create(int equipmentId, string commentText, int? rating)
        {
            // Get current user ID
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            _logger.LogInformation("User {UserId} submitting feedback for equipment {EquipmentId}", userId, equipmentId);
            
            // Basic validation
            bool isValid = true;
            
            // Check if either comment or rating is provided
            if (string.IsNullOrWhiteSpace(commentText) && !rating.HasValue)
            {
                _logger.LogWarning("User {UserId} attempted to submit feedback without comment or rating", userId);
                TempData["Error"] = "You must provide either a comment or a rating.";
                isValid = false;
            }
            
            // Check if rating is valid
            if (rating.HasValue && (rating < 1 || rating > 5))
            {
                _logger.LogWarning("User {UserId} attempted to submit invalid rating {Rating}", userId, rating);
                TempData["Error"] = "Rating must be between 1 and 5.";
                isValid = false;
            }
            
            // Create a new feedback object with the form data
            var feedback = new Feedback
            {
                EquipmentId = equipmentId,
                UserId = userId,
                CommentText = commentText,
                Rating = rating,
                CreatedAt = DateTime.Now,
                IsVisible = true  // Default to visible
            };
            
            // Check if the user has rented this equipment before
            var hasRented = await _context.RentalTransactions
                .AnyAsync(rt => rt.EquipmentId == feedback.EquipmentId && rt.CustomerId == userId);
                
            if (!hasRented)
            {
                _logger.LogWarning("User {UserId} attempted to provide feedback for equipment {EquipmentId} they haven't rented", userId, feedback.EquipmentId);
                TempData["Error"] = "You can only provide feedback for equipment you have rented.";
                isValid = false;
            }
            
            // Check if the user has already left feedback for this equipment
            var existingFeedback = await _context.Feedbacks
                .FirstOrDefaultAsync(f => f.EquipmentId == feedback.EquipmentId && f.UserId == userId);
                
            if (existingFeedback != null)
            {
                _logger.LogWarning("User {UserId} attempted to provide duplicate feedback for equipment {EquipmentId}", userId, feedback.EquipmentId);
                TempData["Error"] = "You have already provided feedback for this equipment.";
                isValid = false;
            }
            
            if (isValid)
            {
                try
                {
                    _context.Add(feedback);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("User {UserId} successfully submitted feedback {FeedbackId} for equipment {EquipmentId}", 
                        userId, feedback.FeedbackId, feedback.EquipmentId);
                    TempData["Success"] = "Your feedback has been submitted successfully.";
                    return RedirectToAction(nameof(Index), new { equipmentId = feedback.EquipmentId });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error saving feedback for user {UserId} on equipment {EquipmentId}", userId, feedback.EquipmentId);
                    ModelState.AddModelError("", $"Error saving feedback: {ex.Message}");
                }
            }
            
            // If we got this far, something failed, redisplay form
            var equipment = await _context.Equipment
                .Include(e => e.Category)
                .FirstOrDefaultAsync(e => e.EquipmentId == feedback.EquipmentId);
                
            if (equipment == null)
            {
                _logger.LogWarning("Equipment with ID {EquipmentId} not found when redisplaying feedback form", feedback.EquipmentId);
                return NotFound();
            }
            
            ViewBag.Equipment = equipment;
            return View(feedback);
        }

        // POST: Feedbacks/ToggleVisibility/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager,Administrator")]
        public async Task<IActionResult> ToggleVisibility(int id, string returnUrl)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            _logger.LogInformation("User {UserId} attempting to toggle visibility for feedback {FeedbackId}", userId, id);
            
            var feedback = await _context.Feedbacks
                .Include(f => f.Equipment)
                .Include(f => f.User)
                .FirstOrDefaultAsync(f => f.FeedbackId == id);
                
            if (feedback == null)
            {
                _logger.LogWarning("Feedback with ID {FeedbackId} not found when toggling visibility", id);
                return NotFound();
            }
            
            // Toggle visibility
            feedback.IsVisible = !feedback.IsVisible;
            
            try
            {
                _context.Update(feedback);
                await _context.SaveChangesAsync();
                
                var action = (feedback.IsVisible.HasValue && feedback.IsVisible.Value) ? "visible" : "hidden";
                _logger.LogInformation("User {UserId} set feedback {FeedbackId} for equipment {EquipmentName} to {Action}", 
                    userId, id, feedback.Equipment.Name, action);
                    
                TempData["Success"] = $"Feedback is now {action}.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling visibility for feedback {FeedbackId} by user {UserId}", id, userId);
                TempData["Error"] = $"Error toggling visibility: {ex.Message}";
            }
            
            // Return to the previous page if specified, otherwise go to index
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            
            return RedirectToAction(nameof(Index));
        }

        // GET: Feedbacks/Delete/5
        [Authorize(Roles = "Manager,Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            
            if (id == null)
            {
                _logger.LogWarning("User {UserId} attempted to delete feedback without providing ID", userId);
                return NotFound();
            }

            _logger.LogInformation("User {UserId} viewing delete confirmation for feedback {FeedbackId}", userId, id);
            
            var feedback = await _context.Feedbacks
                .Include(f => f.Equipment)
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.FeedbackId == id);
                
            if (feedback == null)
            {
                _logger.LogWarning("Feedback with ID {FeedbackId} not found when attempting to delete", id);
                return NotFound();
            }

            return View(feedback);
        }

        // POST: Feedbacks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager,Administrator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            _logger.LogInformation("User {UserId} confirming deletion of feedback {FeedbackId}", userId, id);
            
            var feedback = await _context.Feedbacks
                .Include(f => f.Equipment)
                .FirstOrDefaultAsync(f => f.FeedbackId == id);
                
            if (feedback == null)
            {
                _logger.LogWarning("Feedback with ID {FeedbackId} not found when confirming deletion", id);
                return NotFound();
            }
            
            try
            {
                var equipmentName = feedback.Equipment?.Name ?? "Unknown";
                var equipmentId = feedback.EquipmentId;
                
                _context.Feedbacks.Remove(feedback);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("User {UserId} successfully deleted feedback {FeedbackId} for equipment {EquipmentName} (ID: {EquipmentId})", 
                    userId, id, equipmentName, equipmentId);
                    
                TempData["Success"] = "Feedback has been deleted successfully.";
                return RedirectToAction(nameof(Index), new { equipmentId = equipmentId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting feedback {FeedbackId} by user {UserId}", id, userId);
                TempData["Error"] = $"Error deleting feedback: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }
    }
    
    // View model for equipment with feedback statistics
    public class EquipmentFeedbackViewModel
    {
        public Equipment Equipment { get; set; }
        public int FeedbackCount { get; set; }
        public double AverageRating { get; set; }
    }
}
