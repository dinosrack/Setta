using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Setta.Models;

namespace Setta.ViewModels
{
    public class FilterPageViewModel
    {
        // Список фильтров для отображения
        public ObservableCollection<MuscleGroupFilter> MuscleGroups { get; }

        public ICommand ApplyCommand { get; }
        public ICommand ResetCommand { get; }

        public FilterPageViewModel(IEnumerable<string> selected)
        {
            // все уникальные группы из исходных данных
            var allGroups = ExerciseData.Exercises
                .Select(e => e.MuscleGroup)
                .Distinct()
                .OrderBy(g => g);

            MuscleGroups = new ObservableCollection<MuscleGroupFilter>(
                allGroups.Select(g => new MuscleGroupFilter(g, selected.Contains(g)))
            );

            ApplyCommand = new Command(OnApply);
            ResetCommand = new Command(OnReset);
        }

        void OnApply()
        {
            // собираем выбранные
            var chosen = MuscleGroups
                .Where(f => f.IsSelected)
                .Select(f => f.Name)
                .ToList();

            // отправляем обратно на ExercisesPage
            MessagingCenter.Send(this, "FiltersApplied", chosen);
            // закрываем страницу
            Application.Current.MainPage.Navigation.PopAsync();
        }

        void OnReset()
        {
            // снимаем выделение со всех
            foreach (var filter in MuscleGroups)
                filter.IsSelected = false;
        }
    }
}
