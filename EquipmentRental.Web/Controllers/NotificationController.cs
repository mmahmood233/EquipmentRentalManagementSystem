using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectDBClassLibrary.Model;
using AdvancedProgrammingASPProject.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace AdvancedProgrammingASPProject.Controllers
{
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly ProjectDBContext _context;
        private readonly UserManager<Users> _userManager;

        public NotificationController(ProjectDBContext context, UserManager<Users> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Create(int customerId = 0)
        {
            var identityUser = await _userManager.GetUserAsync(User);
            var mainUser = identityUser?.MainUserId != null ? await _context.Users.FindAsync(identityUser.MainUserId) : null;

            if (mainUser == null || !(mainUser.RoleId == 1 || mainUser.RoleId == 3))
                return Forbid();

            ViewBag.FromCustomerList = customerId != 0;

            if (customerId != 0)
            {
                var customer = await _context.Users.FindAsync(customerId);
                ViewBag.Customers = new SelectList(new List<User> { customer }, "UserId", "Fullname", customer.UserId);
            }
            else
            {
                ViewBag.Customers = new SelectList(_context.Users.Where(u => u.RoleId == 2).ToList(), "UserId", "Fullname");
            }

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string message, string type, int? specificUserId, string? sendToAll)
        {
            // Handle checkbox manually: checkbox = "on" when checked, null otherwise
            bool isSendToAll = !string.IsNullOrWhiteSpace(sendToAll);

            // Validate message and type
            if (string.IsNullOrWhiteSpace(message) || string.IsNullOrWhiteSpace(type))
            {
                ModelState.AddModelError("", "Message and Type are required.");
                ViewBag.Customers = new SelectList(_context.Users.Where(u => u.RoleId == 2).ToList(), "UserId", "Fullname");
                return View();
            }

            // Either send to all or pick one user
            if (!isSendToAll && !specificUserId.HasValue)
            {
                ModelState.AddModelError("", "Please select a specific customer or check 'Send to All Customers'.");
                ViewBag.Customers = new SelectList(_context.Users.Where(u => u.RoleId == 2).ToList(), "UserId", "Fullname");
                return View();
            }

            var notification = new Notification
            {
                Message = message,
                Type = isSendToAll ? "General" : type,
                Status = false
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync(); // Save to get NotificationId

            // Get all users to notify
            List<User> usersToNotify = new();

            if (isSendToAll)
            {
                usersToNotify = await _context.Users.Where(u => u.RoleId == 2).ToListAsync(); // All customers
            }
            else if (specificUserId.HasValue)
            {
                var user = await _context.Users.FindAsync(specificUserId.Value);
                if (user != null)
                    usersToNotify.Add(user);
            }

            // Create NotificationUser links
            foreach (var user in usersToNotify)
            {
                _context.NotificationUsers.Add(new NotificationUser
                {
                    NotificationId = notification.NotificationId,
                    UserId = user.UserId
                });
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Notification sent!";
            return RedirectToAction("Create");
        }




        public async Task<IActionResult> MyNotifications()
        {
            var identityUser = await _userManager.GetUserAsync(User);
            var mainUser = identityUser?.MainUserId != null ? await _context.Users.FindAsync(identityUser.MainUserId) : null;

            if (mainUser == null || mainUser.RoleId != 2)
                return Forbid();

            var notifications = await _context.NotificationUsers
                .Include(nu => nu.Notification)
                .Where(nu => nu.UserId == mainUser.UserId)
                .OrderByDescending(nu => nu.Notification.NotificationId)
                .ToListAsync();

            return View(notifications);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int notificationUserId)
        {
            var notificationUser = await _context.NotificationUsers
                .Include(nu => nu.Notification)
                .FirstOrDefaultAsync(nu => nu.NotificationUserId == notificationUserId);

            if (notificationUser != null)
            {
                notificationUser.Notification.Status = true;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("MyNotifications");
        }


    }
}
