using System.ComponentModel;
using System.Runtime.CompilerServices;

/// <summary>
/// Модель одного подхода (ExerciseSet) в упражнении.
/// Хранит значения веса (Weight) и количества повторений (Reps) как строки для поддержки ввода и биндинга в UI.
/// Реализует INotifyPropertyChanged для корректной работы привязки данных (автоматическое обновление интерфейса при изменении свойств).
/// </summary>

namespace Setta.Models
{
    public class ExerciseSet : INotifyPropertyChanged
    {
        private string _weight;
        public string Weight
        {
            get => _weight;
            set
            {
                if (_weight != value)
                {
                    _weight = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _reps;
        public string Reps
        {
            get => _reps;
            set
            {
                if (_reps != value)
                {
                    _reps = value;
                    OnPropertyChanged();
                }
            }
        }

        // Событие для оповещения интерфейса об изменении свойств (реализация INotifyPropertyChanged)
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
