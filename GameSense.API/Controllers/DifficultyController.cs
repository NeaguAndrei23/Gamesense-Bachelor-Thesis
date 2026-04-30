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

        public DifficultyController(DifficultyService difficultyService)
        {
            _difficultyService = difficultyService;
        }
        //GET /api/difficulty/{levelId}
        //— aggregates session data for a level, trains the model, and returns a difficulty prediction

        [HttpGet("{levelId}")]
        public IActionResult Get(string levelId)
        {
            var trainingData = _difficultyService.GenerateTrainingData();
            if(trainingData.Count < 4)
            {
                return BadRequest("Not enough data to train the model. Please add more sessions.");
            }
            var model = _difficultyService.TrainModel(trainingData);

            var levelData = trainingData.FirstOrDefault(l => l.LevelId == levelId);
            if (levelData == null)
            {
                return NotFound("No session data for this level.");
            }
            var prediction = _difficultyService.Predict(levelData, model);
            return Ok(prediction);
        }
    }
}
