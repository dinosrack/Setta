using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

/// <summary>
/// Модель упражнения для хранения в базе данных и работы с UI.
/// Содержит основное и второстепенные группы мышц, оборудование, а также вспомогательные свойства для работы с коллекциями и состояния объекта.
/// Реализует INotifyPropertyChanged для поддержки привязки данных.
/// Также внутри определён конвертер InverseBoolConverter для преобразования булевых значений в обратное.
/// </summary>

namespace Setta.Models
{
    public class Exercise : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; } // Уникальный идентификатор упражнения (автоинкремент в SQLite)

        private string _exerciseName;
        public string ExerciseName
        {
            get => _exerciseName;
            set
            {
                if (_exerciseName != value)
                {
                    _exerciseName = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _muscleGroup;
        public string MuscleGroup
        {
            get => _muscleGroup;
            set
            {
                if (_muscleGroup != value)
                {
                    _muscleGroup = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _secondaryMuscleGroup;
        public string SecondaryMuscleGroup
        {
            get => _secondaryMuscleGroup;
            set
            {
                if (_secondaryMuscleGroup != value)
                {
                    _secondaryMuscleGroup = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(SecondaryMuscleGroups)); // Обновить список при изменении строки
                }
            }
        }

        private string _equipment;
        public string Equipment
        {
            get => _equipment;
            set
            {
                if (_equipment != value)
                {
                    _equipment = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(EquipmentList)); // Обновить список при изменении строки
                }
            }
        }

        public bool IsLastItem { get; set; } // Вспомогательное свойство для UI (например, стилизация последнего элемента)

        [Ignore]
        public IEnumerable<string> SecondaryMuscleGroups =>
            string.IsNullOrWhiteSpace(SecondaryMuscleGroup)
                ? Enumerable.Empty<string>()
                : SecondaryMuscleGroup.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim());
        // Возвращает перечисление второстепенных групп мышц (разделённые запятыми в строке)

        [Ignore]
        public IEnumerable<string> EquipmentList =>
            string.IsNullOrWhiteSpace(Equipment)
                ? Enumerable.Empty<string>()
                : Equipment.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim());
        // Возвращает перечисление оборудования (разделённого запятыми в строке)

        [Ignore]
        public bool IsFromDatabase { get; set; } // Флаг: получено из БД или нет (для логики приложения, не сохраняется в БД)

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        // Уведомление UI об изменении свойства
    }

    /// <summary>
    /// Конвертер для преобразования булевого значения в противоположное.
    /// Если на входе true — возвращает false, если false — true.
    /// Используется, например, для управления состоянием видимости/доступности элементов в UI.
    /// </summary>
    public class InverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Инвертирует булево значение, если оно задано
            if (value is bool boolean)
                return !boolean;

            // Если значение не булево — по умолчанию возвращает true
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Инвертирует булево значение обратно
            if (value is bool boolean)
                return !boolean;

            return true;
        }
    }
}
