using EquipmentRental.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Cryptography;
using System.Text;
using EquipmentRental.DataAccess.Models;

public class AccountController : Controller
{
    private readonly EquipmentRentalDbContext _context;

    public AccountController(EquipmentRentalDbContext context)
    {
        _context = context;
    }

    // GET: /Account/Register
    public IActionResult Register() => View();

    [HttpPost]
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
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(UserLoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var hashed = HashPassword(model.Password);
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == model.Email && u.PasswordHash == hashed);

        if (user == null)
        {
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

        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("Login");
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}
