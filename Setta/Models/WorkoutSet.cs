using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Модель подхода внутри упражнения тренировки для хранения в базе данных.
/// Содержит уникальный идентификатор, ссылку на упражнение (ExerciseId), количество повторений и вес.
/// Используется для хранения структуры тренировки и последующего анализа.
/// </summary>

namespace Setta.Models
{
    public class WorkoutSet
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }        // Уникальный идентификатор подхода

        public int ExerciseId { get; set; } // Ссылка на упражнение внутри тренировки
        public int Reps { get; set; }       // Количество повторений
        public int Weight { get; set; }     // Вес снаряда
    }
}
