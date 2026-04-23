namespace GameSense.API.Models
{
    //recording death from a given session, and where it occured
    public class DeathEvent
    {
        public int Id { get; set; } //primary key
        public int SessionId { get; set; } //foreign key linking sessions with DeathEvents
        public required string ZoneId { get; set; } //in which hotspot the death occured
        public Session Session { get; set; } = null!; //parent Session reference( reverse navigation property) 
    }
}
