namespace GameSense.Web.Models
{
    public class DifficultyPredictionViewModel
    {
        public string PredictedDifficulty {  get; set; } = string.Empty;
        public float[] Score {  get; set; } = Array.Empty<float>();
    }
}
