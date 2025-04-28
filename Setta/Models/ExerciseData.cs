using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Setta.Models
{
    public static class ExerciseData
    {
        public static ObservableCollection<Models.Exercise> Exercises { get; }

        static ExerciseData()
        {
            // Список упражнений, которые есть в приложении
            Exercises = new ObservableCollection<Models.Exercise>
            {
                new Models.Exercise { ExerciseName = "Жим лежа", MuscleGroup = "Грудь" },
                new Models.Exercise { ExerciseName = "Жим лежа узким хватом", MuscleGroup = "Грудь" },
                new Models.Exercise { ExerciseName = "Тяга верхнего блока обратным хватом", MuscleGroup = "Верхняя часть спины" },
            };

            // Определение последнего элемента в списке
            for (int i = 0; i < Exercises.Count; i++)
            {
                Exercises[i].IsLastItem = (i == Exercises.Count - 1);
            }
        }
    }
}
