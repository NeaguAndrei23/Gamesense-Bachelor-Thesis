namespace GameSense.API.Models
{
    //recording death from a given session, and where it 
    public class DeathEvent
    {
        public int Id { get; set; } //primary key
        public int SessionId { get; set; } //foreign key linking sessions with DeathEvents
        public required string ZoneId { get; set; } //in which hotspot the death occured
        [System.Text.Json.Serialization.JsonIgnore]
        public Session? Session { get; set; } //parent Session reference
    }
}
