using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Setta.Models;
using Setta.Services;
using System.Linq;

/// <summary>
/// ViewModel для выбора упражнений при создании или редактировании шаблона.
/// Поддерживает фильтрацию по названию, группе мышц, оборудованию, а также исключение уже выбранных упражнений.
/// Реализует биндинг, команды для UI, а также возвращает выбранные элементы для добавления в шаблон.
/// </summary>

namespace Setta.ViewModels
{
    public class ChooseExerciseViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<SelectableExercise> Exercises { get; private set; } = new();

        private string _searchQuery;
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                if (_searchQuery != value)
                {
                    _searchQuery = value;
                    OnPropertyChanged();
                    ApplyFilters();
                }
            }
        }

        private List<Exercise> _allExercises = new();
        private List<string> _excludedExerciseNames = new(); // Список исключённых названий

        public List<string> SelectedMuscleGroups { get; set; } = new();
        public List<string> SelectedEquipment { get; set; } = new();

        public ChooseExerciseViewModel()
        {
            _ = LoadExercisesAsync();
        }

        // Установка списка уже выбранных упражнений (для исключения из выбора)
        public void SetExcludedExercises(List<Exercise> alreadySelected)
        {
            _excludedExerciseNames = alreadySelected
                .Where(e => !string.IsNullOrWhiteSpace(e.ExerciseName))
                .Select(e => e.ExerciseName)
                .Distinct()
                .ToList();

            ApplyFilters();
        }

        // Загрузка всех упражнений из БД и Excel
        private async Task LoadExercisesAsync()
        {
            var dbExercises = await ExerciseDatabaseService.GetExercisesAsync();
            foreach (var e in dbExercises)
                e.IsFromDatabase = true;

            _allExercises = dbExercises
                .OrderByDescending(e => e.Id)
                .Concat(ExerciseData.Exercises)
                .ToList();

            ApplyFilters();
        }

        // Применить фильтры из popup
        public void ApplyFiltersFromPopup(List<string> groups, List<string> equipment)
        {
            SelectedMuscleGroups = groups;
            SelectedEquipment = equipment;
            ApplyFilters();
        }

        // Фильтрация и формирование списка упражнений
        private void ApplyFilters()
        {
            var filtered = _allExercises
                .Where(e =>
                    !_excludedExerciseNames.Contains(e.ExerciseName) && // Исключаем уже выбранные
                    (string.IsNullOrWhiteSpace(SearchQuery) || e.ExerciseName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)) &&
                    (SelectedMuscleGroups.Count == 0 || SelectedMuscleGroups.Contains(e.MuscleGroup)) &&
                    (SelectedEquipment.Count == 0 || e.EquipmentList.Any(eq => SelectedEquipment.Contains(eq))))
                .Select(e => new SelectableExercise(e, ToggleSelectCommand))
                .ToList();

            for (int i = 0; i < filtered.Count; i++)
                filtered[i].IsLastItem = (i == filtered.Count - 1);

            Exercises = new ObservableCollection<SelectableExercise>(filtered);
            OnPropertyChanged(nameof(Exercises));
        }

        // Команда для переключения состояния выбранности упражнения
        public ICommand ToggleSelectCommand => new Command<SelectableExercise>(ex =>
        {
            if (ex != null)
                ex.IsSelected = !ex.IsSelected;

            OnPropertyChanged(nameof(Exercises));
        });

        // Получить выбранные упражнения для добавления в шаблон
        public List<Exercise> GetSelectedExercises() =>
            Exercises.Where(e => e.IsSelected).Select(e => e.Exercise).ToList();

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
