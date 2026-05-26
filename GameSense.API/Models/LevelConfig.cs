using System.ComponentModel.DataAnnotations;

namespace GameSense.API.Models
{
    // stores designer-defined configuration for a level
    // created before playtesting begins, defines intent and zone layout

    public class LevelConfig
    {
        public int Id { get; set; }
        public required string LevelId {  get; set; }// matches levelId sent by Unity
        public string? GameName { get; set; } // optional project/game name for separation
        public string? ExpectedDifficulty { get; set; } // designer's intended difficulty: Easy/Medium/Hard/Unfair
        // navigation property — one config has many zones
        public ICollection<LevelZone> Zones { get; set; } = new List<LevelZone>();
    }
}
