using SQLite;
using System;

namespace Setta.Models
{
    public class Workout
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public DateTime Date { get; set; }
        public bool IsActive { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int TotalWeight { get; set; }
        public string TemplateName { get; set; }
        public string ExercisesJson { get; set; }  
        public string TotalDuration { get; set; } 

        // ДЛЯ удобства: вычисляемое свойство для продолжительности тренировки
        [Ignore]
        public TimeSpan Duration => EndTime.HasValue ? EndTime.Value - StartTime : TimeSpan.Zero;
    }
}
