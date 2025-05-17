using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectDBClassLibrary.Model;
using Microsoft.AspNetCore.Identity;
using AdvancedProgrammingASPProject.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;

namespace AdvancedProgrammingASPProject.Controllers
{
    public class FeedbacksController : Controller
    {
        private readonly ProjectDBContext _context;
        private readonly UserManager<Users> _userManager;

        public FeedbacksController(ProjectDBContext context, UserManager<Users> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private async Task<bool> IsAdminOrManager()
        {
            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser?.MainUserId == null) return false;

            var mainUser = await _context.Users.FindAsync(identityUser.MainUserId);
            return mainUser != null && (mainUser.RoleId == 1 || mainUser.RoleId == 3);
        }

        public async Task<IActionResult> ViewFeedback(int equipmentId)
        {
            var identityUser = await _userManager.GetUserAsync(User);
            var mainUserId = identityUser?.MainUserId;

            if (mainUserId == null) return Unauthorized();

            var feedback = await _context.FeedBacks
                .Include(f => f.Replies.Where(r => !r.IsHidden))
                .Include(f => f.Equipment)
                .FirstOrDefaultAsync(f => f.EquipmentId == equipmentId &&
                                          f.UserId == mainUserId &&
                                          f.ParentFeedbackId == null &&
                                          !f.IsHidden);

            if (feedback == null)
                return RedirectToAction("Index", "Equipments");

            return View("ViewFeedback", feedback);
        }

        [Authorize]
        public async Task<IActionResult> CreateFeedback(int equipmentId)
        {
            var identityUser = await _userManager.GetUserAsync(User);
            var mainUser = identityUser?.MainUserId != null
                ? await _context.Users.FindAsync(identityUser.MainUserId)
                : null;

            if (mainUser == null) return Unauthorized();

            // Check if this equipment was ever rented by this user
            var hasRented = await _context.RentalRequests
                .AnyAsync(r => r.UserId == mainUser.UserId && r.EquipmentId == equipmentId);

            if (!hasRented)
                return Forbid(); // Block unauthorized access

            var equipment = await _context.Equipments.FirstOrDefaultAsync(e => e.EquipmentId == equipmentId);
            if (equipment == null) return NotFound();

            ViewBag.EquipmentId = equipmentId;
            ViewBag.EquipmentName = equipment.Name;
            ViewBag.UserId = mainUser.UserId;
            ViewBag.UserName = mainUser.Fullname;

            return View("Create");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFeedback(FeedBack feedback)
        {
            ModelState.Remove("User");
            ModelState.Remove("Equipment");
            feedback.CreatedAt = DateTime.Now;

            if (ModelState.IsValid)
            {
                _context.FeedBacks.Add(feedback);
                await _context.SaveChangesAsync();
                TempData["CreateMessage"] = "Thank you for your feedback!";
                return RedirectToAction("MyCart", "RentalRequests");
            }

            var equipment = await _context.Equipments.FirstOrDefaultAsync(e => e.EquipmentId == feedback.EquipmentId);
            var user = await _context.Users.FindAsync(feedback.UserId);
            ViewBag.EquipmentId = feedback.EquipmentId;
            ViewBag.EquipmentName = equipment?.Name;
            ViewBag.UserId = user?.UserId;
            ViewBag.UserName = user?.Fullname;

            return View("Create", feedback);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFeedback(int feedbackId)
        {
            var feedback = await _context.FeedBacks
                .Include(f => f.Replies)
                .FirstOrDefaultAsync(f => f.FeedbackId == feedbackId);

            if (feedback == null) return NotFound();

            _context.FeedBacks.RemoveRange(feedback.Replies);
            _context.FeedBacks.Remove(feedback);
            await _context.SaveChangesAsync();

            TempData["DeleteMessage"] = "Feedback removed.";
            return RedirectToAction("MyCart", "RentalRequests");
        }

        public async Task<IActionResult> Index(int equipmentId, bool showHidden = false)
        {
            var identityUser = await _userManager.GetUserAsync(User);
            var mainUser = identityUser?.MainUserId != null
                ? await _context.Users.FindAsync(identityUser.MainUserId)
                : null;

            var isAdminOrManager = mainUser != null && (mainUser.RoleId == 1 || mainUser.RoleId == 3);

            //  Enforce access restriction: redirect if non-admin attempts showHidden=true
            if (showHidden && !isAdminOrManager)
            {
                return Forbid(); // or return RedirectToAction("AccessDenied", "Home");
            }

            var feedbacksQuery = _context.FeedBacks
                .Include(f => f.User)
                .Include(f => f.Replies.Where(r => !r.IsHidden))
                    .ThenInclude(r => r.User)
                .Where(f => f.EquipmentId == equipmentId && f.ParentFeedbackId == null);

            feedbacksQuery = showHidden
                ? feedbacksQuery.Where(f => f.IsHidden)
                : feedbacksQuery.Where(f => !f.IsHidden);

            var feedbacks = await feedbacksQuery.OrderByDescending(f => f.CreatedAt).ToListAsync();

            ViewBag.EquipmentId = equipmentId;
            ViewBag.EquipmentName = await _context.Equipments
                .Where(e => e.EquipmentId == equipmentId)
                .Select(e => e.Name)
                .FirstOrDefaultAsync();

            ViewBag.ShowHidden = showHidden;
            ViewBag.IsAdminOrManager = isAdminOrManager;

            return View(feedbacks);
        }


        public async Task<IActionResult> Reply(int id)
        {
            if (!await IsAdminOrManager()) return Forbid();

            var parent = await _context.FeedBacks
                .Include(f => f.User)
                .Include(f => f.Equipment)
                .FirstOrDefaultAsync(f => f.FeedbackId == id);

            if (parent == null) return NotFound();

            var replyModel = new FeedBack
            {
                ParentFeedbackId = parent.FeedbackId,
                EquipmentId = parent.EquipmentId,
                ParentFeedback = parent,
                Equipment = parent.Equipment,
                User = parent.User
            };

            return View(replyModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reply(int parentId, int equipmentId, string comment)
        {
            if (!await IsAdminOrManager()) return Forbid();

            if (string.IsNullOrWhiteSpace(comment))
            {
                TempData["ReplyError"] = "Reply cannot be empty.";
                return RedirectToAction("Index", new { equipmentId });
            }

            var parent = await _context.FeedBacks.FindAsync(parentId);
            if (parent == null) return NotFound();

            var identityUser = await _userManager.GetUserAsync(User);
            var mainUser = identityUser?.MainUserId != null
                ? await _context.Users.FindAsync(identityUser.MainUserId)
                : null;

            var reply = new FeedBack
            {
                EquipmentId = parent.EquipmentId,
                UserId = mainUser?.UserId ?? 1,
                Comment = comment,
                CreatedAt = DateTime.Now,
                ParentFeedbackId = parentId,
                IsHidden = false
            };

            _context.FeedBacks.Add(reply);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", new { equipmentId });
        }

        public async Task<IActionResult> Hide(int id)
        {
            if (!await IsAdminOrManager()) return Forbid();

            var comment = await _context.FeedBacks.FindAsync(id);
            if (comment != null)
            {
                comment.IsHidden = true;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", new { equipmentId = comment.EquipmentId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unhide(int id)
        {
            if (!await IsAdminOrManager()) return Forbid();

            var feedback = await _context.FeedBacks.FindAsync(id);
            if (feedback == null) return NotFound();

            feedback.IsHidden = false;
            _context.Update(feedback);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Feedback unhidden successfully.";
            return RedirectToAction("Index", new { equipmentId = feedback.EquipmentId });
        }
    }
}
