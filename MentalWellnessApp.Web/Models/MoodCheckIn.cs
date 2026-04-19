using System.ComponentModel.DataAnnotations;

namespace MentalWellnessApp.Web.Models;

public class MoodCheckIn
{
    public int Id { get; set; }

    [Required]
    public string ParticipantId { get; set; } = string.Empty;
    public ApplicationUser? Participant { get; set; }

    [Range(1, 10)]
    public int MoodScore { get; set; }

    [StringLength(1000)]
    public string Notes { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
