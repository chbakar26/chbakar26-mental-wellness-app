using Microsoft.AspNetCore.Identity;

namespace MentalWellnessApp.Web.Models;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public string? PreferredCounsellorId { get; set; }
    public ApplicationUser? PreferredCounsellor { get; set; }
}
