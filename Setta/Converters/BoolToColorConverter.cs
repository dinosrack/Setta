using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace Setta.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isSelected = value is bool b && b;
            if (isSelected)
                return Application.Current.Resources["Important"];
            // динамический выбор цвета по теме
            var light = (Color)Application.Current.Resources["LightElement"];
            var dark = (Color)Application.Current.Resources["DarkElement"];
            return Application.Current.RequestedTheme == AppTheme.Dark ? dark : light;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}