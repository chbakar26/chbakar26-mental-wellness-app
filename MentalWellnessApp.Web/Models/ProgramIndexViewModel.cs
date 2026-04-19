namespace MentalWellnessApp.Web.Models;

public class ProgramIndexViewModel
{
    public List<WellnessProgram> Programs { get; set; } = [];
    public string? TopicFilter { get; set; }
    public string? AccessibilityLevelFilter { get; set; }
    public DateTime? StartFrom { get; set; }
    public DateTime? EndTo { get; set; }
    public double AverageMoodScore { get; set; }
}
