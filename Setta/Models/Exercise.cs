using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Setta.Models
{
    public class Exercise : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

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
                    OnPropertyChanged(nameof(SecondaryMuscleGroups)); // Обновить список
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
                    OnPropertyChanged(nameof(EquipmentList)); // Обновить список
                }
            }
        }

        public bool IsLastItem { get; set; }

        [Ignore]
        public IEnumerable<string> SecondaryMuscleGroups =>
            string.IsNullOrWhiteSpace(SecondaryMuscleGroup)
                ? Enumerable.Empty<string>()
                : SecondaryMuscleGroup.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim());

        [Ignore]
        public IEnumerable<string> EquipmentList =>
            string.IsNullOrWhiteSpace(Equipment)
                ? Enumerable.Empty<string>()
                : Equipment.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim());

        [Ignore]
        public bool IsFromDatabase { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }


    public class InverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolean)
                return !boolean;

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolean)
                return !boolean;

            return true;
        }
    }
}
