using System;
using System.Globalization;
using Microsoft.Maui.Controls;

/// <summary>
/// Конвертер для проверки, содержит ли коллекция хотя бы один элемент.
/// Если переданная коллекция не пуста — возвращает true, иначе false.
/// Используется для управления видимостью или состоянием UI-элементов на основе наличия данных в коллекции.
/// </summary>

namespace Setta.Converters
{
    public class EnumerableNotEmptyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Проверяем, что значение — это коллекция, и есть ли в ней элементы
            if (value is System.Collections.IEnumerable en)
                return en.Cast<object>().Any();
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Обратное преобразование не поддерживается
            throw new NotSupportedException();
        }
    }
}
