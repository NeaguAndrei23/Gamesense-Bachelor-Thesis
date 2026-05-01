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

        public List<LevelData> GenerateTrainingData(GameSenseDbContext context)
        {
            var levelIds = context.Sessions.Select(s => s.LevelId)
                .Distinct()
                .ToList();
            var result = new List<LevelData>();

            foreach (var levelId in levelIds)
            {
                var sessions = context.Sessions.Where(s => s.LevelId == levelId).ToList();
                var avgDeaths = (float)sessions.Average(s => s.Deaths);
                var avgDuration = (float)sessions.Average(s => s.DurationSeconds);
                var completionRate = (float)sessions.Count(s => s.IsCompleted)/sessions.Count() * 100;
                var avgAttempts = (float)sessions.Average(s => s.Attempts);

                // assign label using predefined thresholds
                string label;
                if (completionRate < 20 || avgDeaths > 10)
                    label = "Unfair";
                else if (completionRate < 40 || avgDeaths > 5)
                    label = "Hard";
                else if (completionRate < 70 || avgDeaths > 2)
                    label = "Medium";
                else
                    label = "Easy";

                var levelData = new LevelData
                {
                    LevelId = levelId,
                    AverageDeaths = avgDeaths,
                    AverageDuration = avgDuration,
                    CompletionRate = completionRate,
                    AverageAttempts = avgAttempts,
                    Label = label
                };
                result.Add(levelData);
            }
            return result;
        }
        // returns cached model if available, otherwise trains and caches a new one
        // training only happens once per API lifetime, subsequent calls should return quickly
        public ITransformer GetOrTrainModel(GameSenseDbContext context)
        {
            if(_cachedModel == null)
            {
                var trainingData = GenerateTrainingData(context);
                _cachedModel = TrainModel(trainingData);
            }
            return _cachedModel;
        }

    }
}
