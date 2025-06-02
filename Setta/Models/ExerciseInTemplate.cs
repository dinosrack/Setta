using System.Collections.ObjectModel;

/// <summary>
/// Модель для хранения упражнения в составе шаблона тренировки.
/// Хранит ссылку на сам объект упражнения, необязательный идентификатор связи с тренировкой,
/// а также коллекцию подходов (ExerciseSet), из которых рассчитывается общий объём нагрузки.
/// </summary>

namespace Setta.Models
{
    public class ExerciseInTemplate
    {
        // Само упражнение (объект Exercise)
        public Exercise Exercise { get; set; }

        // Необязательный идентификатор упражнения в тренировке (может использоваться для связи с базой данных)
        public int? WorkoutExerciseId { get; set; }

        // Коллекция подходов для данного упражнения
        public ObservableCollection<ExerciseSet> Sets { get; set; } = new();

        // Свойство для подсчёта общего объёма (сумма вес*повторения по всем подходам)
        public int TotalVolume =>
            Sets.Sum(s =>
            {
                // Пытаемся преобразовать вес и количество повторений в int
                bool parsedWeight = int.TryParse(s.Weight, out int w);
                bool parsedReps = int.TryParse(s.Reps, out int r);
                // Если оба значения корректны, добавляем их произведение к сумме
                return (parsedWeight && parsedReps) ? w * r : 0;
            });
    }
}
