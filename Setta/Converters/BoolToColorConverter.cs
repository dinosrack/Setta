using System;
using System.Globalization;
using Microsoft.Maui.Controls;

/// <summary>
/// Конвертер для привязки цвета к булевому значению.
/// Возвращает цвет "Important" из ресурсов, если значение true (например, элемент выбран).
/// В остальных случаях возвращает светлый или тёмный цвет в зависимости от текущей темы приложения (светлая или тёмная).
/// Используется для динамического изменения цвета UI-элементов на основе состояния.
/// </summary>

namespace Setta.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Проверяем, является ли значение true (например, элемент выбран)
            bool isSelected = value is bool b && b;
            if (isSelected)
                // Если выбрано — возвращаем "важный" цвет из ресурсов приложения
                return Application.Current.Resources["Important"];

            // В зависимости от темы приложения выбираем светлый или тёмный цвет
            var light = (Color)Application.Current.Resources["LightElement"];
            var dark = (Color)Application.Current.Resources["DarkElement"];
            // Если тема тёмная — возвращаем тёмный цвет, иначе светлый
            return Application.Current.RequestedTheme == AppTheme.Dark ? dark : light;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            // Обратное преобразование не реализовано, т.к. оно не требуется
            => throw new NotImplementedException();
    }
}
