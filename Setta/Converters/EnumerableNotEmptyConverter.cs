using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace Setta.Converters
{
    public class EnumerableNotEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is System.Collections.IEnumerable en)
                return en.Cast<object>().Any();
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}