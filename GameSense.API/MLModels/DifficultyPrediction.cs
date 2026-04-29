using System.Runtime.Intrinsics.X86;
using Microsoft.ML.Data;

namespace GameSense.API.MLModels
{
    public class DifficultyPrediction
    {
        [ColumnName("PredictedLabel")]
        public string PredictedDifficulty { get; set; } = string.Empty; //easy, medium, hard, unfair
        public float[] Score { get; set; } = Array.Empty<float>(); //confidence scores per diff. class
    }
}
