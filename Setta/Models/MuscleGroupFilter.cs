using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Maui.Controls;

/// <summary>
/// Модель фильтра по группе мышц для интерфейса выбора упражнений.
/// Позволяет отмечать и снимать отметку с фильтра (IsSelected), а также предоставляет команду ToggleCommand для инвертирования состояния.
/// Используется для фильтрации списка упражнений по выбранным группам мышц.
/// Реализует INotifyPropertyChanged для поддержки биндинга в UI.
/// </summary>

namespace Setta.Models
{
    public class MuscleGroupFilter : INotifyPropertyChanged
    {
        // Название группы мышц, по которой фильтруется
        public string Name { get; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; OnPropertyChanged(); }
        }

        // Команда для переключения состояния фильтра (выделен/не выделен)
        public ICommand ToggleCommand { get; }

        public MuscleGroupFilter(string name, bool isSelected = false)
        {
            Name = name;
            IsSelected = isSelected;
            // При выполнении команды инвертируем выбранность фильтра
            ToggleCommand = new Command(() => IsSelected = !IsSelected);
        }

        // Реализация INotifyPropertyChanged — уведомляет интерфейс об изменениях свойств
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
