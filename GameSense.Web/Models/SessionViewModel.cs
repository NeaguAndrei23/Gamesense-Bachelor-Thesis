namespace GameSense.Web.Models
{
    public class SessionViewModel
    {
        public int Id { get; set; } //primary key (automatically in EF Core), identifier of the session
        public string PlayerId { get; set; } //identifies who played the level
        public string LevelId { get; set; } //identifies the level played
        public DateTime StartTime { get; set; } //when the session was started
        public double DurationSeconds { get; set; } //how long the session lasted
        public int Deaths { get; set; } //no of deaths
        public int Attempts { get; set; } //no of attempts
        public bool IsCompleted { get; set; } = false; //if the level was completed or not
    
    }
}
