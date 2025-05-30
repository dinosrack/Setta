using SQLite;
using System;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace Setta.Models
{
    public enum WorkoutStatus
    {
        Active,
        Completed
    }

    public class Workout
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public WorkoutStatus Status { get; set; }
        public DateTime Date { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public double? TotalWeight { get; set; }
        public string? TotalDuration { get; set; }

        // Добавлено: JSON для хранения упражнений
        public string ExercisesJson { get; set; }

        // Не сохраняется в базе: реальные упражнения для работы в UI
        [Ignore]
        public ObservableCollection<ExerciseInTemplate> Exercises
        {
            get
            {
                if (string.IsNullOrEmpty(ExercisesJson))
                    return new ObservableCollection<ExerciseInTemplate>();
                try
                {
                    return JsonSerializer.Deserialize<ObservableCollection<ExerciseInTemplate>>(ExercisesJson)
                        ?? new ObservableCollection<ExerciseInTemplate>();
                }
                catch
                {
                    return new ObservableCollection<ExerciseInTemplate>();
                }
            }
            set
            {
                ExercisesJson = JsonSerializer.Serialize(value ?? new ObservableCollection<ExerciseInTemplate>());
            }
        }

        [Ignore]
        public string Info
        {
            get
            {
                string status = Status == WorkoutStatus.Active ? "В процессе" : "Завершена";
                if (Status == WorkoutStatus.Completed && TotalWeight.HasValue && TotalDuration != null)
                {
                    return $"{status}: {TotalWeight.Value} кг, {TotalDuration}";
                }
                return status;
            }
        }

        [Ignore]
        public string DisplayTime
        {
            get
            {
                if (Date.Date == DateTime.Today)
                    return "Сейчас";
                else
                    return StartTime.ToString("HH:mm");
            }
        }
    }
}
