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
            if (_context.Sessions.Any())
                return Ok("Database already contains data, skipping seed.");
            //fake seeder data for testing only

            var sessions = new List<GameSense.API.Models.Session>
            {
                // Level_1 — Easy (tutorial level)
                new() { PlayerId = "Player_1", LevelId = "Level_1", StartTime = DateTime.Now.AddDays(-7), DurationSeconds = 45.0, Deaths = 1, Attempts = 1, IsCompleted = true },
                new() { PlayerId = "Player_2", LevelId = "Level_1", StartTime = DateTime.Now.AddDays(-6), DurationSeconds = 52.0, Deaths = 0, Attempts = 1, IsCompleted = true },
                new() { PlayerId = "Player_3", LevelId = "Level_1", StartTime = DateTime.Now.AddDays(-6), DurationSeconds = 60.0, Deaths = 2, Attempts = 2, IsCompleted = true },
                new() { PlayerId = "Player_4", LevelId = "Level_1", StartTime = DateTime.Now.AddDays(-5), DurationSeconds = 48.0, Deaths = 1, Attempts = 1, IsCompleted = true },
                new() { PlayerId = "Player_1", LevelId = "Level_1", StartTime = DateTime.Now.AddDays(-4), DurationSeconds = 40.0, Deaths = 0, Attempts = 1, IsCompleted = true },
                new() { PlayerId = "Player_2", LevelId = "Level_1", StartTime = DateTime.Now.AddDays(-3), DurationSeconds = 55.0, Deaths = 1, Attempts = 1, IsCompleted = true },

                // Level_2 — Medium
                new() { PlayerId = "Player_1", LevelId = "Level_2", StartTime = DateTime.Now.AddDays(-7), DurationSeconds = 120.0, Deaths = 3, Attempts = 2, IsCompleted = true },
                new() { PlayerId = "Player_2", LevelId = "Level_2", StartTime = DateTime.Now.AddDays(-6), DurationSeconds = 145.0, Deaths = 4, Attempts = 3, IsCompleted = true },
                new() { PlayerId = "Player_3", LevelId = "Level_2", StartTime = DateTime.Now.AddDays(-5), DurationSeconds = 160.0, Deaths = 5, Attempts = 3, IsCompleted = false },
                new() { PlayerId = "Player_4", LevelId = "Level_2", StartTime = DateTime.Now.AddDays(-4), DurationSeconds = 130.0, Deaths = 3, Attempts = 2, IsCompleted = true },
                new() { PlayerId = "Player_1", LevelId = "Level_2", StartTime = DateTime.Now.AddDays(-3), DurationSeconds = 110.0, Deaths = 2, Attempts = 2, IsCompleted = true },
                new() { PlayerId = "Player_3", LevelId = "Level_2", StartTime = DateTime.Now.AddDays(-2), DurationSeconds = 155.0, Deaths = 4, Attempts = 3, IsCompleted = true },

                // Level_3 — Hard
                new() { PlayerId = "Player_1", LevelId = "Level_3", StartTime = DateTime.Now.AddDays(-7), DurationSeconds = 200.0, Deaths = 7, Attempts = 5, IsCompleted = false },
                new() { PlayerId = "Player_2", LevelId = "Level_3", StartTime = DateTime.Now.AddDays(-6), DurationSeconds = 240.0, Deaths = 8, Attempts = 6, IsCompleted = true },
                new() { PlayerId = "Player_3", LevelId = "Level_3", StartTime = DateTime.Now.AddDays(-5), DurationSeconds = 180.0, Deaths = 6, Attempts = 4, IsCompleted = false },
                new() { PlayerId = "Player_4", LevelId = "Level_3", StartTime = DateTime.Now.AddDays(-4), DurationSeconds = 220.0, Deaths = 9, Attempts = 6, IsCompleted = false },
                new() { PlayerId = "Player_1", LevelId = "Level_3", StartTime = DateTime.Now.AddDays(-3), DurationSeconds = 260.0, Deaths = 8, Attempts = 7, IsCompleted = true },
                new() { PlayerId = "Player_2", LevelId = "Level_3", StartTime = DateTime.Now.AddDays(-2), DurationSeconds = 190.0, Deaths = 7, Attempts = 5, IsCompleted = false },

                // Level_4 — Unfair
                new() { PlayerId = "Player_1", LevelId = "Level_4", StartTime = DateTime.Now.AddDays(-7), DurationSeconds = 300.0, Deaths = 15, Attempts = 10, IsCompleted = false },
                new() { PlayerId = "Player_2", LevelId = "Level_4", StartTime = DateTime.Now.AddDays(-6), DurationSeconds = 280.0, Deaths = 12, Attempts = 8, IsCompleted = false },
                new() { PlayerId = "Player_3", LevelId = "Level_4", StartTime = DateTime.Now.AddDays(-5), DurationSeconds = 320.0, Deaths = 18, Attempts = 12, IsCompleted = false },
                new() { PlayerId = "Player_4", LevelId = "Level_4", StartTime = DateTime.Now.AddDays(-4), DurationSeconds = 350.0, Deaths = 20, Attempts = 14, IsCompleted = false },
                new() { PlayerId = "Player_1", LevelId = "Level_4", StartTime = DateTime.Now.AddDays(-3), DurationSeconds = 290.0, Deaths = 14, Attempts = 9, IsCompleted = false },
                new() { PlayerId = "Player_2", LevelId = "Level_4", StartTime = DateTime.Now.AddDays(-2), DurationSeconds = 310.0, Deaths = 16, Attempts = 11, IsCompleted = false },

                // Level_5 — Medium (different characteristics than Level_2)
                new() { PlayerId = "Player_1", LevelId = "Level_5", StartTime = DateTime.Now.AddDays(-7), DurationSeconds = 100.0, Deaths = 4, Attempts = 3, IsCompleted = true },
                new() { PlayerId = "Player_2", LevelId = "Level_5", StartTime = DateTime.Now.AddDays(-6), DurationSeconds = 115.0, Deaths = 3, Attempts = 2, IsCompleted = true },
                new() { PlayerId = "Player_3", LevelId = "Level_5", StartTime = DateTime.Now.AddDays(-5), DurationSeconds = 130.0, Deaths = 5, Attempts = 4, IsCompleted = false },
                new() { PlayerId = "Player_4", LevelId = "Level_5", StartTime = DateTime.Now.AddDays(-4), DurationSeconds = 95.0, Deaths = 3, Attempts = 2, IsCompleted = true },
                new() { PlayerId = "Player_1", LevelId = "Level_5", StartTime = DateTime.Now.AddDays(-3), DurationSeconds = 120.0, Deaths = 4, Attempts = 3, IsCompleted = true },
                new() { PlayerId = "Player_3", LevelId = "Level_5", StartTime = DateTime.Now.AddDays(-2), DurationSeconds = 110.0, Deaths = 3, Attempts = 2, IsCompleted = true },
            };

            _context.Sessions.AddRange(sessions);
            _context.SaveChanges();


            // seed level configs with expected difficulties
            var levelConfigs = new List<GameSense.API.Models.LevelConfig>
            {
                new() { LevelId = "Level_1", GameName = "GameSense Prototype", ExpectedDifficulty = "Easy" },
                new() { LevelId = "Level_2", GameName = "GameSense Prototype", ExpectedDifficulty = "Medium" },
                new() { LevelId = "Level_3", GameName = "GameSense Prototype", ExpectedDifficulty = "Hard" },
                new() { LevelId = "Level_4", GameName = "GameSense Prototype", ExpectedDifficulty = "Unfair" },
                new() { LevelId = "Level_5", GameName = "GameSense Prototype", ExpectedDifficulty = "Medium" },
            };
            _context.LevelConfigs.AddRange(levelConfigs);
            _context.SaveChanges();
            // fetch the saved sessions to get their assigned IDs
            var savedSessions = _context.Sessions.ToList();

            var deathEvents = new List<GameSense.API.Models.DeathEvent>();

            foreach (var session in savedSessions)
            {
                if (session.LevelId == "Level_1")
                {
                    for (int i = 0; i < session.Deaths; i++)
                        deathEvents.Add(new() { SessionId = session.Id, ZoneId = i % 2 == 0 ? "Zone_A" : "Zone_B" });
                }
                else if (session.LevelId == "Level_2")
                {
                    for (int i = 0; i < session.Deaths; i++)
                        deathEvents.Add(new() { SessionId = session.Id, ZoneId = i % 3 == 0 ? "Zone_A" : i % 3 == 1 ? "Zone_B" : "Zone_C" });
                }
                else if (session.LevelId == "Level_3")
                {
                    for (int i = 0; i < session.Deaths; i++)
                        deathEvents.Add(new() { SessionId = session.Id, ZoneId = i % 3 == 0 ? "Zone_B" : i % 3 == 1 ? "Zone_C" : "Zone_D" });
                }
                else if (session.LevelId == "Level_4")
                {
                    for (int i = 0; i < session.Deaths; i++)
                        deathEvents.Add(new() { SessionId = session.Id, ZoneId = i % 4 == 0 ? "Zone_A" : i % 4 == 1 ? "Zone_B" : i % 4 == 2 ? "Zone_C" : "Zone_D" });
                }
                else if (session.LevelId == "Level_5")
                {
                    for (int i = 0; i < session.Deaths; i++)
                        deathEvents.Add(new() { SessionId = session.Id, ZoneId = i % 2 == 0 ? "Zone_A" : "Zone_C" });
                }
            }
            _context.DeathEvents.AddRange(deathEvents);
            _context.SaveChanges();
            return Ok("Database seeded successfully.");
        }
    }

}
