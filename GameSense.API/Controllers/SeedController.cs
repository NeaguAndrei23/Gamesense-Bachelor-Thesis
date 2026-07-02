using GameSense.API.Data;
using Microsoft.AspNetCore.Mvc;

namespace GameSense.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeedController : ControllerBase
    {
        private readonly GameSenseDbContext _context;

        public SeedController(GameSenseDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Seed()
        {
            if (_context.LevelConfigs.Any(c => c.LevelId.StartsWith("TrainingLevel_")))
            {
                return Ok("Database already contains training data pool");
            }
            var rng = new Random();
            var zones = new[] { "Zone_A", "Zone_B", "Zone_C", "Zone_D", "Zone_E" };
            int levelsToGenerate = 100;
            int sessionsPerLevel = 25;

            var newConfigs = new List<GameSense.API.Models.LevelConfig>();
            var newSessions = new List<GameSense.API.Models.Session>();

            for (int levelNum = 1; levelNum <= levelsToGenerate; levelNum++)
            {
                string levelId = $"TrainingLevel_{levelNum}";
                double difficultyPoint = rng.NextDouble();

                double avgDeathsTarget = 0.5 + difficultyPoint * 17.0;
                double completionTarget = 98.0 - difficultyPoint * 90.0;
                double durationTarget = 50.0 + difficultyPoint * 160.0;
                

                string label =
                    completionTarget < 20 || avgDeathsTarget > 10 ? "Unfair" :
                    completionTarget < 40 || avgDeathsTarget > 5 ? "Hard" :
                    completionTarget < 70 || avgDeathsTarget > 2 ? "Medium" :
                    "Easy";

                newConfigs.Add(new GameSense.API.Models.LevelConfig
                {
                    LevelId = levelId,
                    GameName = "GameSense Prototype",
                    ExpectedDifficulty = label
                });

                for (int i = 0; i < sessionsPerLevel; i++)
                {
                    int deaths = Math.Max(0, (int)Math.Round(avgDeathsTarget + (rng.NextDouble() - 0.5) * avgDeathsTarget * 0.6));
                    double duration = Math.Max(5, durationTarget + (rng.NextDouble() - 0.5) * durationTarget * 0.4);
                    int attempts = deaths + 1;
                    bool isCompleted = rng.NextDouble() * 100 < completionTarget;
                    newSessions.Add(new GameSense.API.Models.Session
                    {
                        PlayerId = $"TrainingPlayer_{levelNum}_{i}",
                        LevelId = levelId,
                        StartTime = DateTime.Now.AddMinutes(-rng.Next(0, 5000)),
                        DurationSeconds = duration,
                        Deaths = deaths,
                        Attempts = attempts,
                        IsCompleted = isCompleted
                    });

                }

            }
            _context.LevelConfigs.AddRange(newConfigs);
            _context.Sessions.AddRange(newSessions);
            _context.SaveChanges();

            var savedSessions = _context.Sessions
                .Where(s => s.LevelId.StartsWith("TrainingLevel_"))
                .ToList();

            var deathEvents = new List<GameSense.API.Models.DeathEvent>();

            foreach ( var session in savedSessions )
            {
                string hotspotZone = zones[rng.Next(zones.Length)];
                for ( int i = 0; i < session.Deaths; i++)
                {
                    string zone = rng.NextDouble() < 0.4 ? hotspotZone : zones[rng.Next(zones.Length)];
                    deathEvents.Add(new() { SessionId = session.Id, ZoneId = zone });
                }
            }

            _context.DeathEvents.AddRange(deathEvents);
            _context.SaveChanges();

            return Ok("Database seeded successfully");
        }
    }

}
