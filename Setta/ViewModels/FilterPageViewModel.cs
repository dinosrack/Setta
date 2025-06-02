using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Setta.Models;

/// <summary>
/// ViewModel для работы с фильтрами по группе мышц и оборудованию.
/// Формирует коллекции фильтров на основе данных, поддерживает выбор, сброс, применение фильтров и выдаёт только реально используемые группы.
/// Используется в popup-окнах и страницах с фильтрами.
/// </summary>

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
            // Определяем только реально присутствующие группы мышц из данных
            var presentGroups = ExerciseData.Exercises
                .Select(e => e.MuscleGroup)
                .Distinct()
                .ToHashSet();

            MuscleGroups = new ObservableCollection<MuscleGroupFilter>(
                _muscleGroupOrder
                    .Where(g => presentGroups.Contains(g))
                    .Select(g => new MuscleGroupFilter(g, selectedGroups.Contains(g)))
            );

            // Определяем только реально присутствующее оборудование из данных
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

        // Получить только реально доступные группы мышц
        public static List<string> GetAvailableMuscleGroups()
        {
            var presentGroups = ExerciseData.Exercises
                .Select(e => e.MuscleGroup)
                .Distinct()
                .ToHashSet();

            return _muscleGroupOrder
                .Where(g => presentGroups.Contains(g))
                .ToList();
        }

        // Получить только реально доступное оборудование
        public static List<string> GetAvailableEquipment()
        {
            var presentEquipment = ExerciseData.Exercises
                .SelectMany(e => e.EquipmentList)
                .Distinct()
                .ToHashSet();

            return _equipmentOrder
                .Where(eq => presentEquipment.Contains(eq))
                .ToList();
        }

        // Отправить выбранные фильтры через MessagingCenter
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

        // Сброс всех фильтров
        private void OnReset()
        {
            foreach (var f in MuscleGroups)
                f.IsSelected = false;

            foreach (var f in EquipmentFilters)
                f.IsSelected = false;
        }
    }
}
