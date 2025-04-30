using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Setta.Models;

namespace Setta.ViewModels
{
    public class FilterPageViewModel
    {
        // Список фильтров для отображения
        public ObservableCollection<MuscleGroupFilter> MuscleGroups { get; }

        // Команды для кнопок
        public ICommand ApplyCommand { get; }
        public ICommand ResetCommand { get; }

        public FilterPageViewModel(IEnumerable<string> selected)
        {
            // Получаем все уникальные группы из ExerciseData
            var allGroups = ExerciseData.Exercises
                .Select(e => e.MuscleGroup)
                .Distinct()
                .OrderBy(g => g);

            MuscleGroups = new ObservableCollection<MuscleGroupFilter>(
                allGroups.Select(g => new MuscleGroupFilter(g, selected.Contains(g)))
            );

            // Лямбда-подписка на асинхронный метод
            ApplyCommand = new Command(async () => await OnApplyAsync());
            ResetCommand = new Command(OnReset);
        }

        // Асинхронный метод применения фильтров и закрытия страницы
        private async Task OnApplyAsync()
        {
            // Собирать выбранные группы
            var chosen = MuscleGroups
                .Where(f => f.IsSelected)
                .Select(f => f.Name)
                .ToList();

            // Отправить результат на ExercisesPage
            MessagingCenter.Send(this, "FiltersApplied", chosen);

            // Закрыть страницу через тот же NavigationStack
            await Shell.Current.Navigation.PopAsync(animated: false);
        }

        // Сброс всех фильтров
        private void OnReset()
        {
            foreach (var filter in MuscleGroups)
                filter.IsSelected = false;
        }
    }
}
