using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Вспомогательная модель для отображения тренировки в списке интерфейса.
/// Содержит данные о времени начала и окончания, общем весе, а также вычисляемые свойства для статуса активности,
/// строкового отображения времени и краткой сводки (длительность и общий вес).
/// Используется для формирования элементов списка на странице тренировок.
/// </summary>

namespace Setta.Models
{
    public class WorkoutViewItem
    {
        public int Id { get; set; }                       // Уникальный идентификатор тренировки
        public DateTime StartDateTime { get; set; }       // Время начала тренировки
        public DateTime? EndDateTime { get; set; }        // Время окончания (null — активная тренировка)
        public int TotalWeight { get; set; }              // Общий вес

        public bool IsActive => EndDateTime == null;      // Признак, что тренировка сейчас активна

        // Строка с временным диапазоном ("Сейчас" или время начала - время конца)
        public string TimeDisplay => IsActive ? "Сейчас" : $"{StartDateTime:HH:mm} - {EndDateTime?.ToString("HH:mm")}";

        // Сводка: длительность (в минутах) и общий вес, только для завершённых тренировок
        public string SummaryText =>
            !IsActive && EndDateTime.HasValue
                ? $"{(int)(EndDateTime.Value - StartDateTime).TotalMinutes} мин | {TotalWeight} кг"
                : "";
    }
}
