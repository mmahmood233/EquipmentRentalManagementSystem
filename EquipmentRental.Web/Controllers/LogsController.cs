using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectDBClassLibrary.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using AdvancedProgrammingASPProject.Areas.Identity.Data;

namespace AdvancedProgrammingASPProject.Controllers
{
    [Authorize] 
    public class LogsController : Controller
    {
        private readonly ProjectDBContext _context;
        private readonly UserManager<Users> _userManager;

        public LogsController(ProjectDBContext context, UserManager<Users> userManager)
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

            var logs = await _context.Logs
                .Include(l => l.User)
                .OrderByDescending(l => l.TimeStamp)
                .ToListAsync();

            return View(logs);
        }
    }

}
