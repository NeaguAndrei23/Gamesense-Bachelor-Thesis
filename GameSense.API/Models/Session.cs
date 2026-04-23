namespace GameSense.API.Models
{
    //a single run through of the level, ending once the player either completes the level or quits. Session data will be
    //aggregated using levelId
    public class Session
    {
        public int Id { get; set; } //primary key (automatically in EF Core), identifier of the session
        public required string PlayerId { get; set; } //identifies who played the level
        public required string LevelId {  get; set; } //identifies the level played
        public DateTime StartTime { get; set; } //when the session was started
        public double DurationSeconds { get; set; } //how long the session lasted
        public int Deaths { get; set; } //no of deaths
        public int Attempts { get; set; } //no of attempts
        public bool IsCompleted { get; set; } //if the level was completed or not

       public ICollection<DeathEvent> DeathEvents { get; set; } = new List<DeathEvent>(); //list of all deaths recorder PER session
    }
}
