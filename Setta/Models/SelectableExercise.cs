using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Setta.Models
{
    /// <summary>
    /// Обёртка над Exercise с состоянием выбора и командой для переключения
    /// </summary>
    public class SelectableExercise : INotifyPropertyChanged
    {
        /// <summary>
        /// Связанное упражнение
        /// </summary>
        public Exercise Exercise { get; }

        /// <summary>
        /// Состояние выбора
        /// </summary>
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

        /// <summary>
        /// Команда переключения выбора (используется в XAML)
        /// </summary>
        public ICommand ToggleCommand { get; set; }

        /// <summary>
        /// Отметка, последний ли это элемент (для скрытия разделителя)
        /// </summary>
        public bool IsLastItem { get; set; }

        /// <summary>
        /// Конструктор без команды (не рекомендуется)
        /// </summary>
        public SelectableExercise(Exercise exercise)
        {
            Exercise = exercise;
        }

        /// <summary>
        /// Конструктор с командой
        /// </summary>
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
