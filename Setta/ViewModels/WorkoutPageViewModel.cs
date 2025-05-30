using System.Collections.ObjectModel;
using Setta.Models;
using System.Linq;
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

        public ObservableCollection<WorkoutGroup> WorkoutGroups { get; set; } = new();

        private bool _isWorkoutListEmpty;
        public bool IsWorkoutListEmpty
        {
            get => _isWorkoutListEmpty;
            set
            {
                if (_isWorkoutListEmpty != value)
                {
                    _isWorkoutListEmpty = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsWorkoutListNotEmpty));
                }
            }
        }
        public bool IsWorkoutListNotEmpty => !IsWorkoutListEmpty;

        public WorkoutPageViewModel()
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "workout.db");
            _db = new WorkoutDatabaseService(dbPath);

            Task.Run(async () => await LoadWorkoutsAsync());
        }

        public async Task LoadWorkoutsAsync()
        {
            var allWorkouts = await _db.GetAllWorkoutsAsync();

            var grouped = allWorkouts
                .GroupBy(w => w.Date.ToString("d MMMM yyyy", new CultureInfo("ru-RU")))
                .OrderByDescending(g => g.Key)
                .Select(g => new WorkoutGroup(g.Key, g))
                .ToList();

            WorkoutGroups.Clear();
            foreach (var group in grouped)
                WorkoutGroups.Add(group);

            IsWorkoutListEmpty = WorkoutGroups.Count == 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
