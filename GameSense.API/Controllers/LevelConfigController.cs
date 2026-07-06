using GameSense.API.Data;
using GameSense.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameSense.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class LevelConfigController : ControllerBase
    {
        private readonly GameSenseDbContext _context;
        public LevelConfigController(GameSenseDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Create([FromBody] LevelConfig levelConfig)
        {
            if (string.IsNullOrEmpty(levelConfig.LevelId))
                return BadRequest("Level Id is required");
            // check if config already exists for this level
            var existing = _context.LevelConfigs.FirstOrDefault(l => l.LevelId == levelConfig.LevelId);
            if (existing != null)
                return Ok(existing);
            // create new config
            _context.LevelConfigs.Add(levelConfig);
            _context.SaveChanges();
            return CreatedAtAction(nameof(Get), new { levelId = levelConfig.LevelId }, levelConfig);
        }


        // GET /api/levelconfig/{levelId} — retrieve level config with zones
        [HttpGet("{levelId}")]
        public IActionResult Get(string levelId)
        {
            var config = _context.LevelConfigs
                .Include(l => l.Zones)
                .FirstOrDefault(l => l.LevelId == levelId);

            if (config == null)
                return NotFound($"No configuration found for level: {levelId}");

            return Ok(config);
        }

        //PUT api/levelconfig/{levelId} - update expected difficulty or/and game name
        [HttpPut("{levelId}")]
        public IActionResult Update(string levelId, [FromBody] UpdateLevelConfigRequest updated)
        {
            
            var config = _context.LevelConfigs.FirstOrDefault(l => l.LevelId == levelId);
            if (config == null)
                return NotFound($"No configuration found for level: {levelId}");

            config.ExpectedDifficulty = updated.ExpectedDifficulty;
            config.GameName = updated.GameName;
            _context.SaveChanges();
            return Ok(config);
        }

        // POST /api/levelconfig/{levelId}/zones — add a zone to a level config
        [HttpPost("{levelId}/zones")]
        public IActionResult AddZone(string levelId, [FromBody] LevelZone zone)
        {
            var config = _context.LevelConfigs.FirstOrDefault(l => l.LevelId == levelId);
            if (config == null)
                return NotFound($"No configuration found for level: {levelId}");

            // prevent duplicate zone names for the same level
            var existing = _context.LevelZones
                .FirstOrDefault(z => z.LevelConfigId == config.Id && z.ZoneName == zone.ZoneName);
            if (existing != null)
            {
                // update description in case the designer changed it in Unity
                existing.ZoneDescription = zone.ZoneDescription;
                _context.SaveChanges();
                return Ok(existing);
            }

            zone.LevelConfigId = config.Id;
            _context.LevelZones.Add(zone);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetZones), new { levelId = levelId }, zone);
        }

        // GET /api/levelconfig/{levelId}/zones — get all zones for a level
        [HttpGet("{levelId}/zones")]
        public IActionResult GetZones(string levelId)
        {
            var config = _context.LevelConfigs.FirstOrDefault(l => l.LevelId == levelId);
            if (config == null)
                return NotFound($"No configuration found for level: {levelId}");

            var zones = _context.LevelZones
                .Where(z => z.LevelConfigId == config.Id)
                .ToList();

            return Ok(zones);
        }

        // DELETE /api/levelconfig/{levelId}/zones/{zoneName} — remove a zone
        [HttpDelete("{levelId}/zones/{zoneName}")]
        public IActionResult DeleteZone(string levelId, string zoneName)
        {
            var config = _context.LevelConfigs.FirstOrDefault(l => l.LevelId == levelId);
            if (config == null)
                return NotFound($"No configuration found for level: {levelId}");

            var zone = _context.LevelZones
                .FirstOrDefault(z => z.LevelConfigId == config.Id && z.ZoneName == zoneName);
            if (zone == null)
                return NotFound($"Zone {zoneName} not found for level: {levelId}");

            _context.LevelZones.Remove(zone);
            _context.SaveChanges();
            return NoContent();
        }

        // DELETE /api/levelconfig/{levelId} — delete entire level config and its zones (cascade)
        [HttpDelete("{levelId}")]
        public IActionResult Delete(string levelId)
        {
            var config = _context.LevelConfigs.FirstOrDefault(l => l.LevelId == levelId);
            if (config == null)
                return NotFound($"No configuration found for level: {levelId}");

            _context.LevelConfigs.Remove(config);
            _context.SaveChanges();
            return NoContent();
        }
    }

    public class UpdateLevelConfigRequest
    {
        public string? ExpectedDifficulty { get; set; }
        public string? GameName { get; set; }
    }
}
