namespace GameSense.API.Models
{
    // represents a single designer-defined zone within a level
    // zone names must match the GameObject names used in Unity
    public class LevelZone
    {
        public int Id { get; set; } // primary key
        public int LevelConfigId { get; set; } // foreign key to LevelConfig
        public required string ZoneName { get; set; } // must match Unity GameObject name exactly
        public string? ZoneDescription { get; set; } // optional designer note about this zone


        // reverse navigation property
        public LevelConfig LevelConfig { get; set; } = null!;
    }
}