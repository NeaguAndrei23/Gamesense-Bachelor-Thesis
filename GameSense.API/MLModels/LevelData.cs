using Microsoft.ML.Data;

namespace GameSense.API.MLModels
{
    //represents the aggregated level statistics, loading them in a .csv file here
    public class LevelData
    {
        [LoadColumn(0)]
        public float AverageDeaths { get; set; }
        [LoadColumn(1)]
        public float AverageDuration { get; set; }
        [LoadColumn(2)]
        public float CompletionRate { get; set; }

        [LoadColumn(3)]
        public float AverageAttempts { get; set; }

        [LoadColumn(4)]
        public string Label { get; set; } = string.Empty; //difficulty label: easy, medium, hard, unfair

        [NoColumn]
        public string LevelId { get; set; } = string.Empty;

    }
}
