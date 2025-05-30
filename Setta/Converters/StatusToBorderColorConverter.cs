using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Setta.Models;

namespace Setta.Converters
{
    public class StatusToBorderColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is WorkoutStatus status)
                return status == WorkoutStatus.Active
                    ? Application.Current.Resources["Important"]
                    : Colors.Transparent; // Нет обводки у завершённых
            return Colors.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
