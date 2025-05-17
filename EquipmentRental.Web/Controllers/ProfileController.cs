using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectDBClassLibrary.Model;
using AdvancedProgrammingASPProject.Areas.Identity.Data;

namespace AdvancedProgrammingASPProject.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<Users> _userManager;
        private readonly ProjectDBContext _mainDb;

        public ProfileController(UserManager<Users> userManager, ProjectDBContext mainDb)
        {
            _userManager = userManager;
            _mainDb = mainDb;
        }

        public async Task<IActionResult> Index()
        {
            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null || identityUser.MainUserId == null)
                return NotFound();

            var mainUser = await _mainDb.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == identityUser.MainUserId);

            if (mainUser == null)
                return NotFound();

            return View(mainUser);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(User model)
        {
            var identityUser = await _userManager.GetUserAsync(User);
            if (identityUser == null || identityUser.MainUserId != model.UserId)
                return Unauthorized();

            var userToUpdate = await _mainDb.Users.FindAsync(model.UserId);
            if (userToUpdate == null)
                return NotFound();

            // Update names only
            userToUpdate.FirstName = model.FirstName;
            userToUpdate.LastName = model.LastName;
            identityUser.FullName = $"{model.FirstName} {model.LastName}";

            // Do NOT update password here — since you removed the password field from the form

            await _mainDb.SaveChangesAsync();
            await _userManager.UpdateAsync(identityUser);

            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction("Index");
        }
    }
}
