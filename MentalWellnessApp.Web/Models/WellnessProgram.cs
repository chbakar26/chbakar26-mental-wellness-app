using System.ComponentModel.DataAnnotations;

namespace MentalWellnessApp.Web.Models;

public class WellnessProgram
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string Topic { get; set; } = string.Empty;

    [Required, StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string AccessibilityLevel { get; set; } = "General";

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Pending;

    [Required]
    public string CounsellorId { get; set; } = string.Empty;

    public ApplicationUser? Counsellor { get; set; }
    public List<WellnessSession> Sessions { get; set; } = [];
    public List<WellnessResource> Resources { get; set; } = [];
}
