using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Модель тренировки для хранения и обработки информации о тренировочном занятии.
/// Содержит уникальный идентификатор, время начала и окончания тренировки, 
/// общий перемещённый вес, а также вычисляемые свойства для статуса (активна/завершена)
/// и длительности тренировки.
/// Используется для хранения истории и отображения в интерфейсе мобильного приложения.
/// </summary>

namespace Setta.Models
{
    public class Workout
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; } // Уникальный идентификатор тренировки

        public DateTime StartDateTime { get; set; }      // Время начала тренировки
        public DateTime? EndDateTime { get; set; }       // Время окончания (null, если тренировка активна)

        public bool IsActive => EndDateTime == null;     // Активна, если не задано время окончания

        public int TotalWeight { get; set; }             // Общий вес за тренировку

        public TimeSpan Duration => (EndDateTime ?? DateTime.Now) - StartDateTime; // Длительность тренировки
    }
}
