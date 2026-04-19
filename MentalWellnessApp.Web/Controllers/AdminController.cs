using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MentalWellnessApp.Web.Data;
using MentalWellnessApp.Web.Models;

namespace MentalWellnessApp.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager) : Controller
{
    public async Task<IActionResult> Index()
    {
        ViewBag.Users = await dbContext.Users.OrderBy(x => x.Email).ToListAsync();
        ViewBag.PendingPrograms = await dbContext.WellnessPrograms.Where(x => x.ApprovalStatus == ApprovalStatus.Pending).ToListAsync();
        ViewBag.PendingSessions = await dbContext.WellnessSessions.Where(x => x.ApprovalStatus == ApprovalStatus.Pending).ToListAsync();
        ViewBag.PendingResources = await dbContext.WellnessResources.Where(x => x.ApprovalStatus == ApprovalStatus.Pending).ToListAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ToggleActive(string userId)
    {
        var user = await dbContext.Users.FindAsync(userId);
        if (user is null)
        {
            return NotFound();
        }

        user.IsActive = !user.IsActive;
        await dbContext.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> SetRole(string userId, string role)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return NotFound();
        }

        var allowedRoles = new[] { "Participant", "Counsellor", "Admin" };
        if (!allowedRoles.Contains(role))
        {
            return BadRequest();
        }

        var currentRoles = await userManager.GetRolesAsync(user);
        if (currentRoles.Any())
        {
            await userManager.RemoveFromRolesAsync(user, currentRoles);
        }

        await userManager.AddToRoleAsync(user, role);
        return RedirectToAction(nameof(Index));
    }
}
