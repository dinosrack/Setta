using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Setta.Models;

namespace Setta.Converters
{
    public class SetIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is CollectionView collectionView &&
                value is ExerciseSet set)
            {
                var index = collectionView.ItemsSource.Cast<object>().ToList().IndexOf(set);
                return $"{index + 1}.";
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
