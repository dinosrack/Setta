using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Setta.Models
{
    // Модель данных для упражнения
    public class Exercise
    {
        public string ExerciseName { get; set; }
        public string MuscleGroup { get; set; }
        public string SecondaryMuscleGroup { get; set; }
        //public string ExerciseIcon { get; set; } 
        public bool IsLastItem { get; set; }
    }

    // Конвертер для инверсии логического значения (true - false)
    // Используется для управления видимостью элементов
    public class InverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolean)
                return !boolean;

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolean)
                return !boolean;

            return true;
        }
    }
}
