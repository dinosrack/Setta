using SQLite;
using System.Collections.ObjectModel;
using System.Text.Json;

/// <summary>
/// Модель шаблона тренировки для хранения в базе данных.
/// Содержит уникальный идентификатор, название шаблона, флаг выбранности,
/// JSON-представление списка упражнений, а также вычисляемые свойства для получения списка упражнений
/// и их названий. Используется для создания, хранения и применения шаблонов тренировок в приложении.
/// </summary>

namespace Setta.Models
{
    public class WorkoutTemplate
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }         // Уникальный идентификатор шаблона

        public string Name { get; set; }    // Название шаблона

        public bool IsSelected { get; set; } // Флаг выбранности шаблона

        public string ExercisesJson { get; set; } // JSON-представление списка упражнений

        // Список упражнений, десериализуемый из JSON (или пустой список при ошибке)
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

        // Строка с названиями всех упражнений в шаблоне
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

    /// <summary>
    /// Модель упражнения внутри шаблона тренировки.
    /// Хранит название, основную группу мышц и список подходов.
    /// </summary>
    public class TemplateExercise
    {
        public string Name { get; set; }                // Название упражнения
        public string MuscleGroup { get; set; }         // Основная группа мышц
        public List<ExerciseSet> Sets { get; set; }     // Список подходов
    }
}
