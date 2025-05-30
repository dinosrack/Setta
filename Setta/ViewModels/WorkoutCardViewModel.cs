using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Setta.Models;

namespace Setta.ViewModels
{
    public class WorkoutCardViewModel
    {
        public string Title { get; set; } = "Тренировка";
        public string RightText { get; set; }
        public string Details { get; set; }
        public Color LineColor { get; set; }
        public Color BorderColor { get; set; }
        public bool IsActive { get; set; }
        public DateTime StartTime { get; set; }

        public WorkoutCardViewModel(Workout workout)
        {
            Title = string.IsNullOrWhiteSpace(workout.TemplateName) ? "Тренировка" : workout.TemplateName;

            StartTime = workout.StartTime;

            if (workout.IsActive)
            {
                // Активная тренировка
                RightText = "Сейчас";
                Details = ""; // Не выводим
                LineColor = Color.FromArgb("#36E36B");
                BorderColor = Color.FromArgb("#36E36B");
                IsActive = true;
            }
            else
            {
                // Завершенная тренировка
                RightText = $"{workout.StartTime:HH:mm} - {workout.EndTime:HH:mm}";
                Details = $"{(int)(workout.Duration.TotalMinutes)} мин – {workout.TotalWeight} кг";
                LineColor = Color.FromArgb("#36E36B");
                BorderColor = Color.FromArgb("#222326"); // или прозрачный, если без обводки
                IsActive = false;
            }
        }
    }

}
