using GameSense.API.Data;
using GameSense.API.MLModels;
using Microsoft.ML;

namespace GameSense.API.Services
{
    public class DifficultyService
    {
        private readonly MLContext _mlContext;
        private ITransformer? _cachedModel;
        public DifficultyService()
        {
            _mlContext = new MLContext(42); //seed given for repeatability

        }
        // Trains a multiclass classification model using SDCA linear classifier(Stochastic Dual Coordinate Ascent).

        public ITransformer TrainModel(List<LevelData> trainingData)
        {
            //take a c# collection and convert into IDataView, fundamental data pipeline for ml.net
            var dataView = _mlContext.Data.LoadFromEnumerable(trainingData);

            var pipeline = _mlContext.Transforms.Conversion
                .MapValueToKey("Label")
                .Append(_mlContext.Transforms.Concatenate("Features",
                nameof(LevelData.AverageDeaths), //avoid variable name change errors
                nameof(LevelData.AverageDuration),
                nameof(LevelData.CompletionRate),
                nameof(LevelData.AverageAttempts)))
                .Append(_mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy()) //maximum entropy to avoid overconfidence
                .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            var model = pipeline.Fit(dataView); //execute training -> return ITransformer
            return model;
        }


        // Generates a single difficulty prediction for a level given its aggregated statistics.
        //   model (ITransformer) — the trained ML.NET model produced by TrainModel(), represents
        //                          the learned mapping from level statistics to difficulty labels.
        //   levelData (LevelData) — aggregated stats for the level to predict (AverageDeaths,
        //                           AverageDuration, CompletionRate, AverageAttempts).
        // Returns DifficultyPrediction containing:
        //     - PredictedDifficulty: the winning label string ("Easy", "Medium", "Hard", "Unfair")
        //     - Score[]: confidence array per class (e.g. [0.05, 0.10, 0.75, 0.10] = 75% Hard)
        // Internally creates a PredictionEngine — ML.NET's optimized single-prediction wrapper that
        // runs the input through all pipeline transformations (key mapping, feature concatenation,
        // SDCA inference) and produces the output.
        public DifficultyPrediction Predict(LevelData levelData, ITransformer model)
        {
            var predictionEngine = _mlContext.Model
                .CreatePredictionEngine<LevelData, DifficultyPrediction>(model);

            return predictionEngine.Predict(levelData); //different Predict() method
        }

        // assigns a difficulty label using predefined thresholds as fallback
        // used when designer has not configured expected difficulty for a level
        private string AssignThresholdLabel(float completionRate, float avgDeaths)
        {
            if (completionRate < 20 || avgDeaths > 10) return "Unfair";
            if (completionRate < 40 || avgDeaths > 5) return "Hard";
            if (completionRate < 70 || avgDeaths > 2) return "Medium";
            return "Easy";
        }
        private LevelData AggregateSessions(string levelId, List<GameSense.API.Models.Session> sessions, 
            string label)
        {
            return new LevelData
            {
                LevelId = levelId,
                AverageDeaths = (float)sessions.Average(s => s.Deaths),
                AverageDuration = (float)sessions.Average(s => s.DurationSeconds),
                CompletionRate = (float)sessions.Count(s => s.IsCompleted) / sessions.Count() * 100,
                AverageAttempts = (float)sessions.Average(s => s.Attempts),
                Label = label
            };
        }
        public List<LevelData> GenerateTrainingData(GameSenseDbContext context)
        {
            var result = new List<LevelData>();

            // get all configs with expected difficulty set — primary training source
            var configs = context.LevelConfigs
                .Where(c => c.ExpectedDifficulty != null)
                .ToList();

            var configuredLevelIds = configs.Select(c => c.LevelId).ToList();

            // Phase 1 — designer configured levels (preferred)
            foreach (var config in configs)
            {
                var sessions = context.Sessions
                    .Where(s => s.LevelId == config.LevelId)
                    .ToList();

                if (!sessions.Any()) continue;

                result.Add(AggregateSessions(config.LevelId, sessions, config.ExpectedDifficulty!));
            }

            // Phase 2 — unconfigured levels, use threshold labels as fallback
            var unconfiguredLevelIds = context.Sessions
                .Select(s => s.LevelId)
                .Distinct()
                .Where(id => !configuredLevelIds.Contains(id))
                .ToList();

            foreach (var levelId in unconfiguredLevelIds)
            {
                var sessions = context.Sessions
                    .Where(s => s.LevelId == levelId)
                    .ToList();

                if (!sessions.Any()) continue;

                var avgDeaths = (float)sessions.Average(s => s.Deaths);
                var completionRate = (float)sessions.Count(s => s.IsCompleted) / sessions.Count() * 100;
                result.Add(AggregateSessions(levelId, sessions, AssignThresholdLabel(completionRate, avgDeaths)));
            }

            return result;
        }

        public LevelData? GetLevelData(GameSenseDbContext context, string levelId)
        {
            var sessions = context.Sessions.Where(s => s.LevelId == levelId).ToList();
            if (!sessions.Any()) return null;

            return AggregateSessions(levelId, sessions, ""); // empty label — predicting, not training
        }
        // returns cached model if available, otherwise trains and caches a new one
        // training only happens once per API lifetime or per level, subsequent calls should return quickly
        public ITransformer GetOrTrainModel(GameSenseDbContext context)
        {
            if (_cachedModel == null)
            {
                var trainingData = GenerateTrainingData(context);
                _cachedModel = TrainModel(trainingData);
            }
            return _cachedModel;
        }
    }
}
