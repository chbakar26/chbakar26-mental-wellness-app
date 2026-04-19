using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MentalWellnessApp.Web.Models;

namespace MentalWellnessApp.Web.Controllers;

[AllowAnonymous]
public class AccountController(UserManager<ApplicationUser> userManager) : Controller
{
    [HttpGet]
    public IActionResult RegisterParticipant() => View(new RegisterRoleViewModel());

    [HttpPost]
    public async Task<IActionResult> RegisterParticipant(RegisterRoleViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FullName = model.FullName,
            IsActive = true,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        await userManager.AddToRoleAsync(user, "Participant");
        return RedirectToAction("Login", "Account", new { area = "Identity" });
    }

    [HttpGet]
    public IActionResult RegisterCounsellor() => View(new RegisterRoleViewModel());

    [HttpPost]
    public async Task<IActionResult> RegisterCounsellor(RegisterRoleViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FullName = model.FullName,
            IsActive = true,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        await userManager.AddToRoleAsync(user, "Counsellor");
        return RedirectToAction("Login", "Account", new { area = "Identity" });
    }
}
