using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectDBClassLibrary.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using AdvancedProgrammingASPProject.Areas.Identity.Data;

namespace AdvancedProgrammingASPProject.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ProjectDBContext _context;
        private readonly UserManager<Users> _userManager;

        public DashboardController(ProjectDBContext context, UserManager<Users> userManager)
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


        public async Task<IActionResult> Index()
        {
            if (!await IsAdminOrManager()) return Forbid();
            // Request stats
            var totalRequests = await _context.RentalRequests.CountAsync();
            var pendingRequests = await _context.RentalRequests.CountAsync(r => r.Status == "Pending");
            var approvedRequests = await _context.RentalRequests.CountAsync(r => r.Status == "Approved");
            var deniedRequests = await _context.RentalRequests.CountAsync(r => r.Status == "Denied");
            var overdueRequests = await _context.RentalRequests
                .CountAsync(r => r.ReturnDate < DateTime.Now && r.Status == "Approved");

            // load Equipment & Requests first, then group in-memory
            var categories = await _context.Categories
                .Include(c => c.Equipment)
                    .ThenInclude(e => e.RentalRequests)
                .ToListAsync();

            var requestsPerCategory = categories
                .Select(c => new
                {
                    Category = c.CategoryName,
                    Count = c.Equipment.SelectMany(e => e.RentalRequests).Count()
                })
                .Where(c => c.Count > 0)
                .ToList();

            // Equipment status
            var damagedEquipment = await _context.Equipments.CountAsync(e => e.ConditionStatus == "D");
            var availableEquipment = await _context.Equipments.CountAsync(e => e.AvailabilityStatus);
            var unavailableEquipment = await _context.Equipments.CountAsync(e => !e.AvailabilityStatus);

            // Financials
            var totalIncome = await _context.RentalTransactions.SumAsync(t => t.RentalFee);
            var totalPending = await _context.RentalTransactions
                .Where(t => t.PaymentStatus != "Paid")
                .SumAsync(t => t.Deposit);

            // Fees
            var totalLateFees = await _context.ReturnInfos.SumAsync(r => r.LateReturnFees ?? 0);
            var totalExtraFees = await _context.ReturnInfos.SumAsync(r => r.AdditionalCharges ?? 0);
            var totalReturnRevenue = totalLateFees + totalExtraFees;

            var totalUsers = await _context.Users.CountAsync();

            var recentLogs = await _context.Logs
                .Include(l => l.User)
                .OrderByDescending(l => l.TimeStamp)
                .Take(10)
                .ToListAsync();

            // Pass data to view
            ViewBag.RequestStats = new { totalRequests, pendingRequests, approvedRequests, deniedRequests, overdueRequests };
            ViewBag.CategoryData = requestsPerCategory;
            ViewBag.EquipmentStats = new { damagedEquipment, availableEquipment, unavailableEquipment };
            ViewBag.FinanceStats = new { totalIncome, totalPending };
            ViewBag.FeeStats = new { totalLateFees, totalExtraFees, totalReturnRevenue };
            ViewBag.TotalUsers = totalUsers;
            ViewBag.RecentLogs = recentLogs;

            return View();
        }
    }
}
