using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace Setta.Converters
{
    public class ReferenceEqualityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => Equals(value, parameter);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => (bool)value ? parameter : null;
    }
}