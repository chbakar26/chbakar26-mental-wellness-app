using System.ComponentModel.DataAnnotations;

namespace MentalWellnessApp.Web.Models;

public class SessionRequest
{
    public int Id { get; set; }

    [Required]
    public string ParticipantId { get; set; } = string.Empty;
    public ApplicationUser? Participant { get; set; }

    [Required]
    public string CounsellorId { get; set; } = string.Empty;
    public ApplicationUser? Counsellor { get; set; }

    public DateTime PreferredDateTime { get; set; }

    [StringLength(1000)]
    public string ParticipantNotes { get; set; } = string.Empty;

    public SessionRequestStatus Status { get; set; } = SessionRequestStatus.Pending;

    [StringLength(1000)]
    public string CounsellorResponse { get; set; } = string.Empty;
}
