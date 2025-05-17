using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EquipmentRental.DataAccess.Models;
using Microsoft.AspNetCore.Authorization;

namespace EquipmentRental.Web.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly EquipmentRentalDbContext _context;

        public NotificationsController(EquipmentRentalDbContext context)
        {
            _context = context;
        }

        // GET: Notifications
        public async Task<IActionResult> Index()
        {
            var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail)) return Unauthorized();

            var notifications = await _context.Notifications
                .Include(n => n.User)
                .Where(n => n.User.Email == userEmail)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return View(notifications);
        }

        // Used by system to generate notifications (NOT an action)
        public async Task CreateSystemNotification(int userId, string message, string type)
        {
            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                Type = type,
                IsRead = false,
                CreatedAt = DateTime.Now
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        // GET: Notifications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Notifications == null)
                return NotFound();

            var notification = await _context.Notifications
                .Include(n => n.User)
                .FirstOrDefaultAsync(m => m.NotificationId == id);

            // User can only view their own notifications
            var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            if (notification == null || notification.User.Email != userEmail)
                return Unauthorized();

            // Mark as read if unread
            if (!notification.IsRead)
            {
                notification.IsRead = true;
                _context.Update(notification);
                await _context.SaveChangesAsync();
            }

            return View(notification);
        }
    }
}
