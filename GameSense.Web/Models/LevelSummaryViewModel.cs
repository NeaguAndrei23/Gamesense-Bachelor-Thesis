namespace GameSense.Web.Models
{
    // represents aggregated statistics for a single level, used on the main dashboard page
    public class LevelSummaryViewModel
    {
        public string LevelId { get; set; } = string.Empty;
        public int SessionCount { get; set; }
        public double AverageDeaths { get; set; }
        public double AverageDuration { get; set; }
        public int CompletionRate { get; set; }
    }
}