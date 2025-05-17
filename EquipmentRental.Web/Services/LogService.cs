using Microsoft.EntityFrameworkCore;
using ProjectDBClassLibrary.Model;
using Microsoft.AspNetCore.Identity;
using AdvancedProgrammingASPProject.Areas.Identity.Data;

public class LogService
{
    private readonly ProjectDBContext _context;
    private readonly UserManager<Users> _userManager;

    public LogService(ProjectDBContext context, UserManager<Users> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task LogAsync(string identityUserId, string action, string source)
    {
        int resolvedUserId = 0;

        if (!string.IsNullOrEmpty(identityUserId) && identityUserId != "Anonymous")
        {
            var identityUser = await _userManager.FindByIdAsync(identityUserId);
            if (identityUser != null && identityUser.MainUserId.HasValue)
            {
                resolvedUserId = identityUser.MainUserId.Value;
            }
        }

        var log = new Log
        {
            UserId = resolvedUserId,
            Action = action,
            Source = source,
            TimeStamp = DateTime.Now
        };

        _context.Logs.Add(log);
        await _context.SaveChangesAsync();
    }
}
