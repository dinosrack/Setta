using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Модель связи между тренировкой и конкретным упражнением, входящим в состав тренировки.
/// Содержит идентификатор, ссылку на тренировку (WorkoutId), название упражнения и основную группу мышц.
/// Используется для хранения структуры тренировки в базе данных.
/// </summary>

namespace Setta.Models
{
    public class WorkoutExercise
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }          // Уникальный идентификатор записи

        public int WorkoutId { get; set; }   // Идентификатор связанной тренировки
        public string Name { get; set; }     // Название упражнения
        public string MuscleGroup { get; set; } // Основная группа мышц
    }
}
