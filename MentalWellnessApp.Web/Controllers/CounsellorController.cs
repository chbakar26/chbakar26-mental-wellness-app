using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MentalWellnessApp.Web.Data;
using MentalWellnessApp.Web.Models;

namespace MentalWellnessApp.Web.Controllers;

[Authorize(Roles = "Counsellor")]
public class CounsellorController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager) : Controller
{
    public async Task<IActionResult> Index()
    {
        var counsellor = await userManager.GetUserAsync(User);
        if (counsellor is null)
        {
            return Challenge();
        }

        ViewBag.Requests = await dbContext.SessionRequests
            .Include(x => x.Participant)
            .Where(x => x.CounsellorId == counsellor.Id)
            .OrderByDescending(x => x.PreferredDateTime)
            .ToListAsync();

        ViewBag.AssignedParticipants = await dbContext.Users
            .Where(x => x.PreferredCounsellorId == counsellor.Id)
            .ToListAsync();

        ViewBag.MoodCheckIns = await dbContext.MoodCheckIns
            .Include(x => x.Participant)
            .Where(x => x.Participant != null && x.Participant.PreferredCounsellorId == counsellor.Id)
            .OrderByDescending(x => x.CreatedAt)
            .Take(50)
            .ToListAsync();

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> RespondToRequest(int id, bool approve, string counsellorResponse)
    {
        var counsellor = await userManager.GetUserAsync(User);
        if (counsellor is null)
        {
            return Challenge();
        }

        var request = await dbContext.SessionRequests.FirstOrDefaultAsync(x => x.Id == id && x.CounsellorId == counsellor.Id);

        if (request is null)
        {
            return NotFound();
        }

        request.Status = approve ? SessionRequestStatus.Approved : SessionRequestStatus.Denied;
        request.CounsellorResponse = counsellorResponse;
        await dbContext.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
