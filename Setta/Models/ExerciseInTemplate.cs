using System.Collections.ObjectModel;

namespace Setta.Models
{
    public class ExerciseInTemplate
    {
        public Exercise Exercise { get; set; }
        public int? WorkoutExerciseId { get; set; }
        public ObservableCollection<ExerciseSet> Sets { get; set; } = new();

        public int TotalVolume =>
            Sets.Sum(s =>
            {
                bool parsedWeight = int.TryParse(s.Weight, out int w);
                bool parsedReps = int.TryParse(s.Reps, out int r);
                return (parsedWeight && parsedReps) ? w * r : 0;
            });
    }
}
