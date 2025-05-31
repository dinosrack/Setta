using SQLite;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace Setta.Models
{
    public class WorkoutTemplate
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsSelected { get; set; }

        // JSON-представление списка упражнений
        public string ExercisesJson { get; set; }

        // Вычисляемое свойство — список упражнений
        public List<TemplateExercise> Exercises
        {
            get
            {
                try
                {
                    return JsonSerializer.Deserialize<List<TemplateExercise>>(ExercisesJson) ?? new();
                }
                catch
                {
                    return new();
                }
            }
        }

        // Вычисляемое свойство — строка с названиями упражнений
        public string ExerciseNames
        {
            get
            {
                try
                {
                    var list = Exercises;
                    return string.Join(", ", list.Select(e => e.Name));
                }
                catch
                {
                    return string.Empty;
                }
            }
        }
    }

    public class TemplateExercise
    {
        public string Name { get; set; }
        public string MuscleGroup { get; set; }
        public List<ExerciseSet> Sets { get; set; }
    }
}
