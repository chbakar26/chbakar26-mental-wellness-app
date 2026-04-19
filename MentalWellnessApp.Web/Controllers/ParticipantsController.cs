using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MentalWellnessApp.Web.Data;
using MentalWellnessApp.Web.Models;

namespace MentalWellnessApp.Web.Controllers;

[Authorize(Roles = "Participant")]
public class ParticipantsController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager) : Controller
{
    public async Task<IActionResult> Index()
    {
        var user = await userManager.GetUserAsync(User);
        var counsellors = await userManager.GetUsersInRoleAsync("Counsellor");
        ViewBag.Counsellors = counsellors.Where(x => x.IsActive).ToList();
        ViewBag.MyRequests = await dbContext.SessionRequests
            .Include(x => x.Counsellor)
            .Where(x => x.ParticipantId == user!.Id)
            .OrderByDescending(x => x.PreferredDateTime)
            .ToListAsync();

        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> SelectCounsellor(string counsellorId)
    {
        var user = await userManager.GetUserAsync(User);
        user!.PreferredCounsellorId = counsellorId;
        await userManager.UpdateAsync(user);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> MoodCheckIns()
    {
        var user = await userManager.GetUserAsync(User);
        var checkIns = await dbContext.MoodCheckIns
            .Where(x => x.ParticipantId == user!.Id)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return View(checkIns);
    }

    [HttpPost]
    public async Task<IActionResult> AddMoodCheckIn(int moodScore, string notes)
    {
        var user = await userManager.GetUserAsync(User);
        dbContext.MoodCheckIns.Add(new MoodCheckIn
        {
            ParticipantId = user!.Id,
            MoodScore = moodScore,
            Notes = notes,
            CreatedAt = DateTime.UtcNow
        });

        await dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(MoodCheckIns));
    }

    [HttpGet]
    public IActionResult RequestSession() => View();

    [HttpPost]
    public async Task<IActionResult> RequestSession(DateTime preferredDateTime, string participantNotes)
    {
        var user = await userManager.GetUserAsync(User);
        if (string.IsNullOrWhiteSpace(user?.PreferredCounsellorId))
        {
            ModelState.AddModelError(string.Empty, "Please select a preferred counsellor first.");
            return View();
        }

        dbContext.SessionRequests.Add(new SessionRequest
        {
            ParticipantId = user.Id,
            CounsellorId = user.PreferredCounsellorId,
            PreferredDateTime = preferredDateTime,
            ParticipantNotes = participantNotes,
            Status = SessionRequestStatus.Pending
        });

        await dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
