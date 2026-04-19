using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MentalWellnessApp.Web.Data;
using MentalWellnessApp.Web.Models;
using System.Diagnostics;

namespace MentalWellnessApp.Web.Controllers;

public class HomeController(ApplicationDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index()
    {
        var vm = new ProgramIndexViewModel
        {
            Programs = await dbContext.WellnessPrograms
                .Where(x => x.ApprovalStatus == ApprovalStatus.Approved)
                .OrderBy(x => x.StartDate)
                .Take(5)
                .ToListAsync(),
            AverageMoodScore = await dbContext.MoodCheckIns.AnyAsync()
                ? Math.Round(await dbContext.MoodCheckIns.AverageAsync(x => x.MoodScore), 2)
                : 0
        };

        return View(vm);
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
