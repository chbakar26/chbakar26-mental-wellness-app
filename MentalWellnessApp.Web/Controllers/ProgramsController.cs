using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MentalWellnessApp.Web.Data;
using MentalWellnessApp.Web.Models;

namespace MentalWellnessApp.Web.Controllers;

public class ProgramsController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager) : Controller
{
    [AllowAnonymous]
    public async Task<IActionResult> Index(string? topic, string? accessibilityLevel, DateTime? startFrom, DateTime? endTo)
    {
        var query = dbContext.WellnessPrograms
            .Include(x => x.Counsellor)
            .AsQueryable();

        if (!User.IsInRole("Admin") && !User.IsInRole("Counsellor"))
        {
            query = query.Where(x => x.ApprovalStatus == ApprovalStatus.Approved);
        }

        if (!string.IsNullOrWhiteSpace(topic))
        {
            query = query.Where(x => x.Topic.Contains(topic));
        }

        if (!string.IsNullOrWhiteSpace(accessibilityLevel))
        {
            query = query.Where(x => x.AccessibilityLevel == accessibilityLevel);
        }

        if (startFrom.HasValue)
        {
            query = query.Where(x => x.StartDate >= startFrom.Value.Date);
        }

        if (endTo.HasValue)
        {
            query = query.Where(x => x.EndDate <= endTo.Value.Date);
        }

        var vm = new ProgramIndexViewModel
        {
            Programs = await query.OrderBy(x => x.StartDate).ToListAsync(),
            TopicFilter = topic,
            AccessibilityLevelFilter = accessibilityLevel,
            StartFrom = startFrom,
            EndTo = endTo,
            AverageMoodScore = await dbContext.MoodCheckIns.AnyAsync() ? Math.Round(await dbContext.MoodCheckIns.AverageAsync(x => x.MoodScore), 2) : 0
        };

        return View(vm);
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var program = await dbContext.WellnessPrograms
            .Include(x => x.Counsellor)
            .Include(x => x.Sessions)
            .Include(x => x.Resources)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (program is null)
        {
            return NotFound();
        }

        if (!User.IsInRole("Admin") && !User.IsInRole("Counsellor") && program.ApprovalStatus != ApprovalStatus.Approved)
        {
            return Forbid();
        }

        return View(program);
    }

    [Authorize(Roles = "Counsellor")]
    [HttpGet]
    public IActionResult Create() => View(new WellnessProgram { StartDate = DateTime.UtcNow.Date, EndDate = DateTime.UtcNow.Date.AddDays(7) });

    [Authorize(Roles = "Counsellor")]
    [HttpPost]
    public async Task<IActionResult> Create(WellnessProgram model)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        model.CounsellorId = user.Id;
        model.ApprovalStatus = ApprovalStatus.Pending;
        dbContext.WellnessPrograms.Add(model);
        await dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Counsellor")]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var user = await userManager.GetUserAsync(User);
        var program = await dbContext.WellnessPrograms.FirstOrDefaultAsync(x => x.Id == id && x.CounsellorId == user!.Id);
        return program is null ? NotFound() : View(program);
    }

    [Authorize(Roles = "Counsellor")]
    [HttpPost]
    public async Task<IActionResult> Edit(WellnessProgram model)
    {
        var user = await userManager.GetUserAsync(User);
        var existing = await dbContext.WellnessPrograms.FirstOrDefaultAsync(x => x.Id == model.Id && x.CounsellorId == user!.Id);
        if (existing is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        existing.Topic = model.Topic;
        existing.Description = model.Description;
        existing.AccessibilityLevel = model.AccessibilityLevel;
        existing.StartDate = model.StartDate;
        existing.EndDate = model.EndDate;
        existing.ApprovalStatus = ApprovalStatus.Pending;
        await dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id = existing.Id });
    }

    [Authorize(Roles = "Counsellor")]
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await userManager.GetUserAsync(User);
        var program = await dbContext.WellnessPrograms.FirstOrDefaultAsync(x => x.Id == id && x.CounsellorId == user!.Id);
        if (program is null)
        {
            return NotFound();
        }

        dbContext.WellnessPrograms.Remove(program);
        await dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Counsellor")]
    [HttpPost]
    public async Task<IActionResult> AddSession(int programId, string topic, string description, DateTime scheduledAt, string googleMeetLink)
    {
        var user = await userManager.GetUserAsync(User);
        var program = await dbContext.WellnessPrograms.FirstOrDefaultAsync(x => x.Id == programId && x.CounsellorId == user!.Id);
        if (program is null)
        {
            return NotFound();
        }

        dbContext.WellnessSessions.Add(new WellnessSession
        {
            WellnessProgramId = programId,
            Topic = topic,
            Description = description,
            ScheduledAt = scheduledAt,
            GoogleMeetLink = googleMeetLink,
            LifecycleStatus = SessionLifecycleStatus.Open,
            ApprovalStatus = ApprovalStatus.Pending,
            CounsellorId = user!.Id
        });

        await dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id = programId });
    }

    [Authorize(Roles = "Counsellor")]
    [HttpPost]
    public async Task<IActionResult> AddResource(int programId, string title, string url, string description)
    {
        var user = await userManager.GetUserAsync(User);
        var program = await dbContext.WellnessPrograms.FirstOrDefaultAsync(x => x.Id == programId && x.CounsellorId == user!.Id);
        if (program is null)
        {
            return NotFound();
        }

        dbContext.WellnessResources.Add(new WellnessResource
        {
            WellnessProgramId = programId,
            Title = title,
            Url = url,
            Description = description,
            ApprovalStatus = ApprovalStatus.Pending,
            CounsellorId = user!.Id
        });

        await dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id = programId });
    }

    [Authorize(Roles = "Counsellor")]
    [HttpPost]
    public async Task<IActionResult> UpdateSessionLifecycle(int sessionId, SessionLifecycleStatus status)
    {
        var user = await userManager.GetUserAsync(User);
        var session = await dbContext.WellnessSessions.Include(x => x.WellnessProgram)
            .FirstOrDefaultAsync(x => x.Id == sessionId && x.CounsellorId == user!.Id);

        if (session is null)
        {
            return NotFound();
        }

        session.LifecycleStatus = status;
        await dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id = session.WellnessProgramId });
    }

    [Authorize(Roles = "Participant")]
    [HttpPost]
    public async Task<IActionResult> BookSession(int sessionId)
    {
        var user = await userManager.GetUserAsync(User);
        var session = await dbContext.WellnessSessions.FirstOrDefaultAsync(x => x.Id == sessionId);

        if (session is null || session.ApprovalStatus != ApprovalStatus.Approved || session.LifecycleStatus != SessionLifecycleStatus.Open || !string.IsNullOrEmpty(session.ParticipantId))
        {
            return BadRequest();
        }

        session.ParticipantId = user!.Id;
        await dbContext.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = session.WellnessProgramId });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> ApproveProgram(int id, bool approved)
    {
        var program = await dbContext.WellnessPrograms.FindAsync(id);
        if (program is null)
        {
            return NotFound();
        }

        program.ApprovalStatus = approved ? ApprovalStatus.Approved : ApprovalStatus.Disapproved;
        await dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> ApproveSession(int id, bool approved)
    {
        var session = await dbContext.WellnessSessions.Include(x => x.WellnessProgram).FirstOrDefaultAsync(x => x.Id == id);
        if (session is null)
        {
            return NotFound();
        }

        session.ApprovalStatus = approved ? ApprovalStatus.Approved : ApprovalStatus.Disapproved;
        await dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id = session.WellnessProgramId });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> ApproveResource(int id, bool approved)
    {
        var resource = await dbContext.WellnessResources.Include(x => x.WellnessProgram).FirstOrDefaultAsync(x => x.Id == id);
        if (resource is null)
        {
            return NotFound();
        }

        resource.ApprovalStatus = approved ? ApprovalStatus.Approved : ApprovalStatus.Disapproved;
        await dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id = resource.WellnessProgramId });
    }
}
