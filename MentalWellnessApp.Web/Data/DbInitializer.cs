using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MentalWellnessApp.Web.Models;

namespace MentalWellnessApp.Web.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        if (context.Database.IsRelational())
        {
            await context.Database.MigrateAsync();
        }
        else
        {
            await context.Database.EnsureCreatedAsync();
        }

        var roles = new[] { "Admin", "Counsellor", "Participant", "Visitor" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        async Task<ApplicationUser> EnsureUserAsync(string email, string fullName, string role, string password)
        {
            var existing = await userManager.FindByEmailAsync(email);
            if (existing != null)
            {
                if (!await userManager.IsInRoleAsync(existing, role))
                {
                    await userManager.AddToRoleAsync(existing, role);
                }

                return existing;
            }

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = fullName,
                EmailConfirmed = true,
                IsActive = true
            };

            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Unable to seed user {email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            await userManager.AddToRoleAsync(user, role);
            return user;
        }

        await EnsureUserAsync("admin@wellness.local", "System Admin", "Admin", "Admin123!");
        var counsellor = await EnsureUserAsync("counsellor@wellness.local", "Sample Counsellor", "Counsellor", "Counsellor123!");
        var participant = await EnsureUserAsync("participant@wellness.local", "Sample Participant", "Participant", "Participant123!");

        participant.PreferredCounsellorId = counsellor.Id;
        await userManager.UpdateAsync(participant);

        if (!await context.WellnessPrograms.AnyAsync())
        {
            var program = new WellnessProgram
            {
                Topic = "Managing Stress at Work",
                Description = "Guided techniques for stress awareness and daily resilience.",
                AccessibilityLevel = "Beginner",
                StartDate = DateTime.UtcNow.Date,
                EndDate = DateTime.UtcNow.Date.AddDays(30),
                ApprovalStatus = ApprovalStatus.Approved,
                CounsellorId = counsellor.Id
            };

            context.WellnessPrograms.Add(program);
            await context.SaveChangesAsync();

            context.WellnessSessions.Add(new WellnessSession
            {
                WellnessProgramId = program.Id,
                Topic = "Breathing and Grounding",
                Description = "Practical session on breathing exercises.",
                ScheduledAt = DateTime.UtcNow.AddDays(2),
                GoogleMeetLink = "https://meet.google.com/sample-link",
                LifecycleStatus = SessionLifecycleStatus.Open,
                ApprovalStatus = ApprovalStatus.Approved,
                CounsellorId = counsellor.Id
            });

            context.WellnessResources.Add(new WellnessResource
            {
                WellnessProgramId = program.Id,
                Title = "Daily Reflection Journal",
                Url = "https://example.com/journal",
                Description = "A downloadable daily reflection worksheet.",
                ApprovalStatus = ApprovalStatus.Approved,
                CounsellorId = counsellor.Id
            });

            context.MoodCheckIns.Add(new MoodCheckIn
            {
                ParticipantId = participant.Id,
                MoodScore = 7,
                Notes = "Feeling balanced this week.",
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            });

            await context.SaveChangesAsync();
        }
    }
}
