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

        // JSON-представление списка упражнений
        public string ExercisesJson { get; set; }

        // Вычисляемое свойство — список имён упражнений
        public string ExerciseNames
        {
            get
            {
                try
                {
                    var list = JsonSerializer.Deserialize<List<TemplateExercise>>(ExercisesJson);
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
