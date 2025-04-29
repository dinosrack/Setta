using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace Setta.Models
{
    public class MuscleGroupFilter : INotifyPropertyChanged
    {
        public string Name { get; }

        bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; OnPropertyChanged(); }
        }

        public ICommand ToggleCommand { get; }

        public MuscleGroupFilter(string name, bool isSelected = false)
        {
            Name = name;
            IsSelected = isSelected;
            ToggleCommand = new Command(() => IsSelected = !IsSelected);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string prop = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}