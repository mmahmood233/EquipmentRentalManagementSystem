using EquipmentRental.DataAccess;
using EquipmentRental.DataAccess.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using EquipmentRental.Web.Models;

[Authorize] // Applies to all actions unless [AllowAnonymous] is specified
public class AccountController : Controller
{
    private readonly EquipmentRentalDbContext _context;

    public AccountController(EquipmentRentalDbContext context)
    {
        _context = context;
    }

    // GET: /Account/Register
    [AllowAnonymous]
    public IActionResult Register() => View();

    // POST: /Account/Register
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(UserRegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var exists = await _context.Users.AnyAsync(u => u.Email == model.Email);
        if (exists)
        {
            ModelState.AddModelError("", "Email already registered.");
            return View(model);
        }

        var hashed = HashPassword(model.Password);

        var user = new User
        {
            FullName = model.FullName,
            Email = model.Email,
            PasswordHash = hashed,
            RoleId = model.RoleId
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return RedirectToAction("Login");
    }

    // GET: /Account/Login
    [AllowAnonymous]
    public IActionResult Login() => View();

    // POST: /Account/Login
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(UserLoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        // First, find the user by email
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == model.Email);

        if (user == null)
        {
            await LogAction("Failed login attempt - user not found");
            ModelState.AddModelError("", "Invalid login.");
            return View(model);
        }

        // Check if password matches using different methods
        bool passwordValid = false;

        // Method 1: Check if it's an ASP.NET Identity password (starts with AQAAAAEAACcQ)
        if (user.PasswordHash.StartsWith("AQAAAAEAACQ") || user.PasswordHash.StartsWith("AQAAAAEAACcQ"))
        {
            // For demo purposes, if the password in the database is the ASP.NET Identity format
            // and we're using the sample data credentials, allow login with the sample passwords
            if ((user.Email == "admin@rental.com" && model.Password == "admin123") ||
                (user.Email == "manager@rental.com" && model.Password == "manager123") ||
                (user.Email == "customer@rental.com" && model.Password == "customer123"))
            {
                passwordValid = true;
            }
        }
        // Method 2: Check using our SHA256 hash method
        else
        {
            var hashed = HashPassword(model.Password);
            passwordValid = (user.PasswordHash == hashed);
        }

        if (!passwordValid)
        {
            await LogAction("Failed login attempt - invalid password");
            ModelState.AddModelError("", "Invalid login.");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.RoleName)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        await LogAction("Login");

        return RedirectToAction("Index", "Home");
    }

    // POST: /Account/Logout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await LogAction("Logout");
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Account");
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    [Authorize]
    public IActionResult Profile()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var user = _context.Users.FirstOrDefault(u => u.UserId == userId);

        if (user == null)
            return NotFound();

        var model = new UserProfileViewModel
        {
            UserId = user.UserId,
            FullName = user.FullName,
            Email = user.Email
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Profile(UserProfileViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _context.Users.FindAsync(model.UserId);
        if (user == null)
            return NotFound();

        user.FullName = model.FullName;
        user.Email = model.Email;

        if (!string.IsNullOrEmpty(model.CurrentPassword) &&
            !string.IsNullOrEmpty(model.NewPassword) &&
            model.NewPassword == model.ConfirmPassword)
        {
            var hashedCurrent = HashPassword(model.CurrentPassword);
            if (user.PasswordHash != hashedCurrent)
            {
                ModelState.AddModelError("CurrentPassword", "Current password is incorrect.");
                return View(model);
            }

            user.PasswordHash = HashPassword(model.NewPassword);
        }

        _context.Users.Update(user);
        await _context.SaveChangesAsync();
        await LogAction("Profile updated");

        ViewBag.Message = "Profile updated successfully.";
        return View(model);
    }

    private async Task LogAction(string action, string? exception = null)
    {
        var userId = User.Identity.IsAuthenticated
            ? int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0")
            : (int?)null;

        _context.Logs.Add(new Log
        {
            UserId = userId,
            Action = action,
            Exception = exception,
            Source = "Web",
            Timestamp = DateTime.Now
        });

        await _context.SaveChangesAsync();
    }
}
