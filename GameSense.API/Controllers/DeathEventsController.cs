using GameSense.API.Data;
using GameSense.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace GameSense.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeathEventsController : ControllerBase
    {
        private readonly GameSenseDbContext _context;

        public DeathEventsController(GameSenseDbContext context)
        {
            _context = context;
        }
        // GET api/deathevents/session/{sessionId} — retrieve all death events for a specific session
        [HttpGet("session/{sessionId}")]
        public IActionResult Get(int sessionId)
        {
            var sessionDeath = _context.DeathEvents.Where(d => d.SessionId == sessionId).ToList();
            if (!sessionDeath.Any())
            {
                return NotFound();
            }
            return Ok(sessionDeath);
        }
        // GET api/deathevents/zones/{levelId} — retrieve death counts grouped by zone for a specific level, used for hotspot visualization on the dashboard
        [HttpGet("zones/{levelId}")]
        public IActionResult GetZoneStats(string levelId)
        {
            var sessionIds = _context.Sessions
                .Where(s => s.LevelId == levelId)       // filter sessions by level
                .Select(s => s.Id)                      // from those sessions, take only the Id
                .ToList();                              // execute and store as a list of integers

            if (!sessionIds.Any())
                return NotFound();

            var zoneStats = _context.DeathEvents
                .Where(d => sessionIds.Contains(d.SessionId))               // only deaths from level's sessions
                .GroupBy(d => d.ZoneId)                                     // group deaths together by zone name
                .Select(g => new { zone = g.Key, deathCount = g.Count() })  // for each group, take the zone name and count deaths
                .ToList();

            return Ok(zoneStats);
        }
        // POST api/deathevents — record a single death event during a session, called by Unity when player dies in a zone
        [HttpPost]
        public IActionResult Create([FromBody] DeathEvent deathEvent)
        {
            var session = _context.Sessions.FirstOrDefault(s => s.Id == deathEvent.SessionId);
            if (session == null)
                return NotFound();

            _context.DeathEvents.Add(deathEvent);
            _context.SaveChanges();
            return CreatedAtAction(nameof(Get), new { sessionId = deathEvent.SessionId }, deathEvent);
        }
    }
}
