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
            return CreatedAtAction(nameof(Create), new { levelId = levelConfig.LevelId }, levelConfig);
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
    }
}
