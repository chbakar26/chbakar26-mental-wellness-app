using System.ComponentModel.DataAnnotations;

namespace MentalWellnessApp.Web.Models;

public class WellnessSession
{
    public int Id { get; set; }

    [Required]
    public int WellnessProgramId { get; set; }
    public WellnessProgram? WellnessProgram { get; set; }

    [Required, StringLength(120)]
    public string Topic { get; set; } = string.Empty;

    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    public DateTime ScheduledAt { get; set; }

    [Url, StringLength(500)]
    public string GoogleMeetLink { get; set; } = string.Empty;

    public SessionLifecycleStatus LifecycleStatus { get; set; } = SessionLifecycleStatus.Draft;
    public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Pending;

    [Required]
    public string CounsellorId { get; set; } = string.Empty;
    public ApplicationUser? Counsellor { get; set; }

    public string? ParticipantId { get; set; }
    public ApplicationUser? Participant { get; set; }
}
