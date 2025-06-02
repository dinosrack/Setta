using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Setta.Models;

/// <summary>
/// Конвертер для получения порядкового номера (индекса) подхода (ExerciseSet) в CollectionView.
/// Возвращает строку вида "1.", "2.", ... для отображения номеров подходов в UI.
/// Если значение не найдено или параметры не соответствуют — возвращает пустую строку.
/// Используется для визуальной нумерации подходов внутри упражнения.
/// </summary>

namespace Setta.Converters
{
    public class SetIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Проверяем, что parameter — это CollectionView, а value — это ExerciseSet
            if (parameter is CollectionView collectionView &&
                value is ExerciseSet set)
            {
                // Получаем индекс текущего подхода (set) в источнике данных CollectionView
                var index = collectionView.ItemsSource.Cast<object>().ToList().IndexOf(set);
                // Возвращаем номер подхода с точкой, начиная с 1
                return $"{index + 1}.";
            }

            // Если условия не выполнены — возвращаем пустую строку
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            // Обратное преобразование не реализовано
            => throw new NotImplementedException();
    }
}
