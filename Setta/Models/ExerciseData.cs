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
                // Трапеции
                new Models.Exercise { ExerciseName = "Шраги со штангой", MuscleGroup = "Трапеции" },
                new Models.Exercise { ExerciseName = "Шраги с гантелями", MuscleGroup = "Трапеции" },

                // Передние дельты
                new Models.Exercise { ExerciseName = "Армейский жим со штангой", MuscleGroup = "Передние дельты" },
                new Models.Exercise { ExerciseName = "Армейский жим с гантелями", MuscleGroup = "Передние дельты" },
                new Models.Exercise { ExerciseName = "Армейский жим в тренажере", MuscleGroup = "Передние дельты" },
                new Models.Exercise { ExerciseName = "Армейский жим в тренажере Смита", MuscleGroup = "Передние дельты" },
                new Models.Exercise { ExerciseName = "Жим Арнольда с гантелями", MuscleGroup = "Передние дельты" },
                new Models.Exercise { ExerciseName = "Подъем гантелей перед собой", MuscleGroup = "Передние дельты" },

                // Боковые дельты
                new Models.Exercise { ExerciseName = "Махи гантелей в стороны", MuscleGroup = "Боковые дельты" },
                new Models.Exercise { ExerciseName = "Махи в стороны в кроссовере", MuscleGroup = "Боковые дельты" },
                new Models.Exercise { ExerciseName = "Тяга к подбородку", MuscleGroup = "Боковые дельты" },
                new Models.Exercise { ExerciseName = "Тяга к подбородку в кроссовере", MuscleGroup = "Боковые дельты" },

                // Задние дельты
                new Models.Exercise { ExerciseName = "Разводка гантелей назад", MuscleGroup = "Боковые дельты" },
                new Models.Exercise { ExerciseName = "Разводка в кроссовере", MuscleGroup = "Боковые дельты" },
                new Models.Exercise { ExerciseName = "Тяга гантелей к лицу", MuscleGroup = "Задние дельты" },
                new Models.Exercise { ExerciseName = "Тяга к лицу в кроссовере", MuscleGroup = "Боковые дельты" },
                new Models.Exercise { ExerciseName = "Махи назад на тренажере", MuscleGroup = "Боковые дельты" },

                // Грудь
                new Models.Exercise { ExerciseName = "Жим лежа", MuscleGroup = "Грудь" },
                new Models.Exercise { ExerciseName = "Жим лежа узким хватом", MuscleGroup = "Грудь" },
                new Models.Exercise { ExerciseName = "Жим лежа широким хватом", MuscleGroup = "Грудь" },
                new Models.Exercise { ExerciseName = "Жим лежа Спото", MuscleGroup = "Грудь" },
                new Models.Exercise { ExerciseName = "Жим лежа в тренажере Смита", MuscleGroup = "Грудь" },
                new Models.Exercise { ExerciseName = "Жим лежа в тренажере Смита узким хватом", MuscleGroup = "Грудь" },
                new Models.Exercise { ExerciseName = "Жим лежа в тренажере Смита широким хватом", MuscleGroup = "Грудь" },
                new Models.Exercise { ExerciseName = "Жим лежа в тренажере Смита на наклонной скамье", MuscleGroup = "Грудь" },
                new Models.Exercise { ExerciseName = "Жим лежа на наклонной скамье", MuscleGroup = "Грудь" },
                new Models.Exercise { ExerciseName = "Жим лежа с бруском", MuscleGroup = "Грудь" },
                new Models.Exercise { ExerciseName = "Жим лежа с поднятыми ногами", MuscleGroup = "Грудь" },
                new Models.Exercise { ExerciseName = "Жим лежа с долгой паузой", MuscleGroup = "Грудь" },
                new Models.Exercise { ExerciseName = "Жим гантелей", MuscleGroup = "Грудь" },
                new Models.Exercise { ExerciseName = "Жим гантелей на наклонной скамье", MuscleGroup = "Грудь" },
                new Models.Exercise { ExerciseName = "Разводка гантелей в стороны", MuscleGroup = "Грудь" },
                new Models.Exercise { ExerciseName = "Разводка гантелей в стороны на наклонной скамье", MuscleGroup = "Грудь" },
                new Models.Exercise { ExerciseName = "Разводка в тренажере", MuscleGroup = "Грудь" },
                new Models.Exercise { ExerciseName = "Жим от груди сидя", MuscleGroup = "Грудь" },
                new Models.Exercise { ExerciseName = "Жим от груди сидя на наклонной скамье", MuscleGroup = "Грудь" },
                new Models.Exercise { ExerciseName = "Сведение рук в кроссовере", MuscleGroup = "Грудь" },
                new Models.Exercise { ExerciseName = "Сведение рук вниз в кроссовере", MuscleGroup = "Грудь" },
                new Models.Exercise { ExerciseName = "Сведение рук вверх в кроссовере", MuscleGroup = "Грудь" },
                new Models.Exercise { ExerciseName = "Отжимания", MuscleGroup = "Грудь" },

                // Верх спины
                new Models.Exercise { ExerciseName = "Тяга верхнего блока обратным хватом", MuscleGroup = "Верх спины" },
                new Models.Exercise { ExerciseName = "Тяга нижнего блока", MuscleGroup = "Верх спины" },
                new Models.Exercise { ExerciseName = "Тяга штанги в наклоне", MuscleGroup = "Верх спины" },
                new Models.Exercise { ExerciseName = "Тяга гантелей в наклоне", MuscleGroup = "Верх спины" },
                new Models.Exercise { ExerciseName = "Тяга гантели одной рукой", MuscleGroup = "Верх спины" },
                new Models.Exercise { ExerciseName = "Пуловер с гантелью", MuscleGroup = "Верх спины" },

                // Бицепсы
                new Models.Exercise { ExerciseName = "Подъем штанги", MuscleGroup = "Бицепсы" },
                new Models.Exercise { ExerciseName = "Подъем изогнутой штанги", MuscleGroup = "Бицепсы" },
                new Models.Exercise { ExerciseName = "Подъем гантелей", MuscleGroup = "Бицепсы" },
                new Models.Exercise { ExerciseName = "Поочередный подъем гантелей", MuscleGroup = "Бицепсы" },
                new Models.Exercise { ExerciseName = "Подъем гантелей в наклонной скамье", MuscleGroup = "Бицепсы" },
                new Models.Exercise { ExerciseName = "Подъем Скотта", MuscleGroup = "Бицепсы" },
                new Models.Exercise { ExerciseName = "Молотки", MuscleGroup = "Бицепсы" },
                new Models.Exercise { ExerciseName = "Молотки на наклонной скамье", MuscleGroup = "Бицепсы" },
                new Models.Exercise { ExerciseName = "Подъем нижнего блока с канатом", MuscleGroup = "Бицепсы" },
                new Models.Exercise { ExerciseName = "Подъем нижнего блока с рукоятью", MuscleGroup = "Бицепсы" },
                new Models.Exercise { ExerciseName = "Подъем нижнего блока с изогнутой рукоятью", MuscleGroup = "Бицепсы" },

                // Трицепсы
                new Models.Exercise { ExerciseName = "Французский жим", MuscleGroup = "Трицепсы" },
                new Models.Exercise { ExerciseName = "Французский жим с гантелями", MuscleGroup = "Трицепсы" },
                new Models.Exercise { ExerciseName = "Французский жим с гантелей сидя", MuscleGroup = "Трицепсы" },
                new Models.Exercise { ExerciseName = "Французский жим с гантелями на наклонной скамье", MuscleGroup = "Трицепсы" },
                new Models.Exercise { ExerciseName = "Разгибание с канатом в кроссовере", MuscleGroup = "Трицепсы" },
                new Models.Exercise { ExerciseName = "Разгибание с канатом в кроссовере из-за головы", MuscleGroup = "Трицепсы" },
                new Models.Exercise { ExerciseName = "Разгибание с рукоятью в кроссовере", MuscleGroup = "Трицепсы" },
                new Models.Exercise { ExerciseName = "Отжимания на брусьях", MuscleGroup = "Трицепсы" },

                // Предплечья
                new Models.Exercise { ExerciseName = "Сгибания запястий с гантелями сидя", MuscleGroup = "Предплечья" },
                new Models.Exercise { ExerciseName = "Сгибания запястий со штангой сидя", MuscleGroup = "Предплечья" },
                new Models.Exercise { ExerciseName = "Пронация кисти с гантелью", MuscleGroup = "Предплечья" },
                new Models.Exercise { ExerciseName = "Вис на турнике", MuscleGroup = "Предплечья" },

                // Пресс
                new Models.Exercise { ExerciseName = "Скручивания", MuscleGroup = "Пресс" },
                new Models.Exercise { ExerciseName = "Обратные скручивания", MuscleGroup = "Пресс" },
                new Models.Exercise { ExerciseName = "Русский твист", MuscleGroup = "Пресс" },
                new Models.Exercise { ExerciseName = "Альпинист", MuscleGroup = "Пресс" },
                new Models.Exercise { ExerciseName = "С колесом", MuscleGroup = "Пресс" },
                new Models.Exercise { ExerciseName = "Подъем ног в висе", MuscleGroup = "Пресс" },
                new Models.Exercise { ExerciseName = "Подъем ног лежа", MuscleGroup = "Пресс" },
                new Models.Exercise { ExerciseName = "Планка", MuscleGroup = "Пресс" },

                // Низ спины
                new Models.Exercise { ExerciseName = "Гиперэкстензия", MuscleGroup = "Низ спины" },

                // Ягодицы
                new Models.Exercise { ExerciseName = "Ягодичный мост", MuscleGroup = "Ягодицы" },
                new Models.Exercise { ExerciseName = "Разведение ног в тренажере", MuscleGroup = "Ягодицы" },
                new Models.Exercise { ExerciseName = "Махи ногой с утяжелителем", MuscleGroup = "Ягодицы" },
                new Models.Exercise { ExerciseName = "Махи ногой в тренажере", MuscleGroup = "Ягодицы" },
                new Models.Exercise { ExerciseName = "Махи ногой в тренажере", MuscleGroup = "Ягодицы" },
                new Models.Exercise { ExerciseName = "Приседания", MuscleGroup = "Ягодицы" },
                new Models.Exercise { ExerciseName = "Приседания сумо", MuscleGroup = "Ягодицы" },
                new Models.Exercise { ExerciseName = "Выпады", MuscleGroup = "Ягодицы" },

                // Квадрицепсы

                // Бицепсы бедер

                // Икры

            };

            // Определение последнего элемента в списке
            for (int i = 0; i < Exercises.Count; i++)
            {
                Exercises[i].IsLastItem = (i == Exercises.Count - 1);
            }
        }
    }
}
