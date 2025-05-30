using SQLite;
using System;

namespace Setta.Models
{
    public class Workout
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public DateTime Date { get; set; } // Дата тренировки
        public bool IsActive { get; set; } // Активная или завершенная
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int TotalWeight { get; set; } // Общий вес (кг)
        public int TotalDuration { get; set; } // Длительность в минутах

        // Список упражнений в сериализованном виде (с подходами)
        public string ExercisesJson { get; set; }
    }
}
