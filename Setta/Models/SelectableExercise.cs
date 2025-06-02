using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

/// <summary>
/// Обёртка над Exercise с состоянием выбора и командой для переключения.
/// Используется для реализации выбора упражнений в списках (например, при добавлении к тренировке или в шаблон).
/// Поддерживает биндинг выбранности (IsSelected), передачу команды для изменения выбранности,
/// а также хранит отметку о последнем элементе (для корректного отображения разделителей в UI).
/// </summary>

namespace Setta.Models
{
    public class SelectableExercise : INotifyPropertyChanged
    {
        public Exercise Exercise { get; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand ToggleCommand { get; set; } // Команда для смены выбранности

        public bool IsLastItem { get; set; } // Флаг для скрытия разделителя в UI

        // Конструктор без команды
        public SelectableExercise(Exercise exercise)
        {
            Exercise = exercise;
        }

        // Конструктор с командой
        public SelectableExercise(Exercise exercise, ICommand toggleCommand)
        {
            Exercise = exercise;
            ToggleCommand = toggleCommand;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
