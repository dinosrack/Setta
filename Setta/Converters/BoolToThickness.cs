using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Конвертер для привязки толщины рамки (или линии) к булевому значению.
/// Если значение true — возвращает толщину 2 (например, элемент выбран и должен быть выделен рамкой).
/// Если значение false — возвращает толщину 0 (рамка не отображается).
/// Используется для визуального выделения выбранных элементов в интерфейсе.
/// </summary>

namespace Setta.Converters
{
    public class BoolToThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Если значение true — толщина 2, иначе 0
            return value is bool b && b ? 2 : 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Обратное преобразование не реализовано
            throw new NotImplementedException();
        }
    }
}
