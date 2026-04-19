using System.ComponentModel.DataAnnotations;

namespace MentalWellnessApp.Web.Models;

public class WellnessResource
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string Title { get; set; } = string.Empty;

    [Url, Required, StringLength(500)]
    public string Url { get; set; } = string.Empty;

    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Pending;

    [Required]
    public int WellnessProgramId { get; set; }
    public WellnessProgram? WellnessProgram { get; set; }

    [Required]
    public string CounsellorId { get; set; } = string.Empty;
    public ApplicationUser? Counsellor { get; set; }
}
