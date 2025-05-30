using System.Collections.ObjectModel;
using Setta.Models;
using System.Linq;
using System.Windows.Input;
using System.ComponentModel;
using Setta.Services;
using System.Runtime.CompilerServices;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace Setta.ViewModels
{
    // Класс для группировки тренировок по датам
    public class WorkoutGroup : ObservableCollection<Workout>
    {
        public string Date { get; set; }
        public WorkoutGroup(string date, IEnumerable<Workout> workouts) : base(workouts)
        {
            Date = date;
        }
    }

    public class WorkoutPageViewModel : INotifyPropertyChanged
    {
        private WorkoutDatabaseService _db;

        public ObservableCollection<WorkoutGroup> GroupedWorkouts { get; set; } = new();

        private bool _isEmpty;
        public bool IsEmpty
        {
            get => _isEmpty;
            set
            {
                if (_isEmpty != value)
                {
                    _isEmpty = value;
                    OnPropertyChanged();
                }
            }
        }

        public WorkoutPageViewModel()
        {
            // Инициализация сервиса БД (убери, если используешь DI)
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "workout.db");
            _db = new WorkoutDatabaseService(dbPath);

            // Загрузка тренировок при создании ViewModel
            Task.Run(async () => await LoadWorkoutsAsync());
        }

        public async Task LoadWorkoutsAsync()
        {
            var allWorkouts = await _db.GetAllWorkoutsAsync();

            var grouped = allWorkouts
                .GroupBy(w => w.StartDate.ToString("d MMMM yyyy", new CultureInfo("ru-RU")))
                .OrderByDescending(g => g.Key)
                .Select(g => new WorkoutGroup(g.Key, g))
                .ToList();

            GroupedWorkouts.Clear();
            foreach (var group in grouped)
                GroupedWorkouts.Add(group);

            IsEmpty = GroupedWorkouts.Count == 0;
        }

        // Для обновления после добавления/удаления тренировок вызови LoadWorkoutsAsync()

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
