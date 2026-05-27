namespace GameSense.Web.Models
{
    // represents level configuration set by the designer
    public class LevelConfigViewModel
    {
        public int Id { get; set; }
        public string LevelId { get; set; } = string.Empty;
        public string? GameName { get; set; }
        public string? ExpectedDifficulty { get; set; }
        public List<ZoneViewModel> Zones { get; set; } = new List<ZoneViewModel>();
    }

    // represents a single zone within a level config
    public class ZoneViewModel
    {
        public int Id { get; set; }
        public string ZoneName { get; set; } = string.Empty;
        public string? ZoneDescription { get; set; }
    }
}
