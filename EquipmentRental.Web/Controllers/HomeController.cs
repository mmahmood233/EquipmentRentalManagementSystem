using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AdvancedProgrammingASPProject.Models;
using ProjectDBClassLibrary.Model;
using Microsoft.EntityFrameworkCore;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ProjectDBContext _context;

    public HomeController(ILogger<HomeController> logger, ProjectDBContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        var feedbacks = _context.FeedBacks
            .Include(f => f.User)
            .Include(f => f.Equipment)
            .Where(f => !f.IsHidden && f.ParentFeedbackId == null) 
            .ToList();

        var avgRatings = feedbacks
            .GroupBy(f => f.Equipment.Name)
            .ToDictionary(g => g.Key, g => Math.Round(g.Average(f => f.Rating), 2));

        ViewBag.AvgRatings = avgRatings;

        return View(feedbacks);
    }

    public IActionResult AllFeedback()
    {
        var feedbacks = _context.FeedBacks
            .Include(f => f.User)
            .Include(f => f.Equipment)
            .Where(f => !f.IsHidden && f.ParentFeedbackId == null) 
            .OrderByDescending(f => f.CreatedAt)
            .ToList();

        return View(feedbacks);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
