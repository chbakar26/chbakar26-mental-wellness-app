using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MentalWellnessApp.Web.Models;

namespace MentalWellnessApp.Web.Controllers;

[Authorize]
public class DashboardController(UserManager<ApplicationUser> userManager) : Controller
{
    public async Task<IActionResult> Index()
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        ViewBag.FullName = user.FullName;
        return View();
    }
}
