using System;
using System.Globalization;
using Microsoft.Maui.Controls;

/// <summary>
/// Конвертер для проверки эквивалентности двух объектов по ссылке или значению.
/// В Convert возвращает true, если value и parameter равны (используется метод Equals).
/// Может использоваться, например, для выделения выбранного элемента в списке по привязке.
/// В ConvertBack возвращает parameter, если значение true (то есть элемент выбран), иначе null.
/// </summary>

namespace Setta.Converters
{
    public class ReferenceEqualityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            // Проверяет эквивалентность value и parameter
            => Equals(value, parameter);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            // Если значение true (элемент выбран), возвращает parameter, иначе null
            => (bool)value ? parameter : null;
    }
}
