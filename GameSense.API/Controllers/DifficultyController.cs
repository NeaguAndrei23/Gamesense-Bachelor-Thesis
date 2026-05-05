using GameSense.API.Data;
using GameSense.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameSense.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DifficultyController : ControllerBase
    {
        private readonly DifficultyService _difficultyService;
        private readonly GameSenseDbContext _context;

        public DifficultyController(DifficultyService difficultyService, GameSenseDbContext context)
        {
            _difficultyService = difficultyService;
            _context = context;
        }


        //GET /api/difficulty/{levelId}
        //— aggregates session data for a level, trains the model, and returns a difficulty prediction

        [HttpGet("{levelId}")]
        public IActionResult Get(string levelId)
        {
            var model = _difficultyService.GetOrTrainModel(_context);

            var levelData = _difficultyService.GetLevelData(_context, levelId);
            if (levelData == null)
                return NotFound("No session data for this level.");

            var prediction = _difficultyService.Predict(levelData, model);
            return Ok(prediction);
        }
    }
}
