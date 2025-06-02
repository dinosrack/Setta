using System;
using System.Globalization;
using Microsoft.Maui.Controls;

/// <summary>
/// Конвертер для проверки, что значение не равно null.
/// Если значение не null — возвращает true, иначе false.
/// Используется для управления состояниями или видимостью UI-элементов, когда важно наличие значения.
/// </summary>

namespace Setta.Converters
{
    public class NotNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            // Возвращает true, если значение не null
            => value != null;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            // Обратное преобразование не реализовано
            => throw new NotImplementedException();
    }
}
