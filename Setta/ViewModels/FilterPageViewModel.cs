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
        // Жёстко заданный порядок отображения групп мышц
        private static readonly List<string> _muscleGroupOrder = new()
        {
            "Трапеции",
            "Передние дельты",
            "Боковые дельты",
            "Задние дельты",
            "Грудь",
            "Верх спины",
            "Бицепсы",
            "Трицепсы",
            "Предплечья",
            "Пресс",
            "Низ спины",
            "Ягодицы",
            "Квадрицепсы",
            "Бицепсы бедра",
            "Икры",
        };

        // Жёстко заданный порядок отображения оборудования
        private static readonly List<string> _equipmentOrder = new()
        {
            "Штанга",
            "Гантели",
            "Изогнутый гриф",
            "Тренажер",
            "Тренажер Смита",
            "Кроссовер",
            "Собственный вес",
        };

        public ObservableCollection<MuscleGroupFilter> MuscleGroups { get; }
        public ObservableCollection<MuscleGroupFilter> EquipmentFilters { get; }

        public ICommand ApplyCommand { get; }
        public ICommand ResetCommand { get; }

        public FilterPageViewModel(IEnumerable<string> selectedGroups,
                                   IEnumerable<string> selectedEquipment)
        {
            // Отбираем только те группы мышц, которые есть в данных
            var presentGroups = ExerciseData.Exercises
                .Select(e => e.MuscleGroup)
                .Distinct()
                .ToHashSet();

            MuscleGroups = new ObservableCollection<MuscleGroupFilter>(
                _muscleGroupOrder
                    .Where(g => presentGroups.Contains(g))
                    .Select(g => new MuscleGroupFilter(g, selectedGroups.Contains(g)))
            );

            // Отбираем только то оборудование, которое есть в данных
            var presentEquipment = ExerciseData.Exercises
                .SelectMany(e => e.EquipmentList)
                .Distinct()
                .ToHashSet();

            EquipmentFilters = new ObservableCollection<MuscleGroupFilter>(
                _equipmentOrder
                    .Where(eq => presentEquipment.Contains(eq))
                    .Select(eq => new MuscleGroupFilter(eq, selectedEquipment.Contains(eq)))
            );

            ApplyCommand = new Command(async () => await OnApplyAsync());
            ResetCommand = new Command(OnReset);
        }

        public async Task OnApplyAsync()
        {
            var chosenGroups = MuscleGroups
                .Where(f => f.IsSelected)
                .Select(f => f.Name)
                .ToList();

            var chosenEquipment = EquipmentFilters
                .Where(f => f.IsSelected)
                .Select(f => f.Name)
                .ToList();

            MessagingCenter.Send(this,
                "FiltersApplied",
                Tuple.Create(chosenGroups, chosenEquipment));
        }

        private void OnReset()
        {
            foreach (var f in MuscleGroups)
                f.IsSelected = false;

            foreach (var f in EquipmentFilters)
                f.IsSelected = false;
        }
    }
}