using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectDBClassLibrary.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using AdvancedProgrammingASPProject.Areas.Identity.Data;


namespace AdvancedProgrammingASPProject.Controllers
{
    [Authorize]
    public class CustomersController : Controller
    {
        private readonly ProjectDBContext _context;
        private readonly UserManager<Users> _userManager;

        public CustomersController(ProjectDBContext context, UserManager<Users> userManager)
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



        public async Task<IActionResult> Index(string searchName, int page = 1)
        {
            if (!await IsAdminOrManager()) return Forbid();
            int pageSize = 20;

            IQueryable<User> customersQuery = _context.Users
                .Where(u => u.RoleId == 2);

            if (!string.IsNullOrWhiteSpace(searchName))
            {
                customersQuery = customersQuery.Where(u => (u.FirstName + " " + u.LastName).Contains(searchName));
            }

            var totalItems = await customersQuery.CountAsync();
            var items = await customersQuery
                .OrderBy(u => u.UserId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.SearchName = searchName;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return View(items);
        }

    }
}
