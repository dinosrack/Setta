using System.Collections.ObjectModel;
using Setta.Models;
using Setta.Pages;
using System.Linq;
using System.Windows.Input;
using System.ComponentModel;
using Setta.Services;
using System.Runtime.CompilerServices;

namespace Setta.ViewModels
{
    public class WorkoutPageViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Workout> Workouts { get; set; } = new();
        public ObservableCollection<WorkoutGroup> WorkoutGroups { get; set; } = new();

        public bool IsWorkoutListEmpty => Workouts.Count == 0;
        public bool IsWorkoutListNotEmpty => !IsWorkoutListEmpty;

        // Добавь сюда метод загрузки
        public async Task LoadWorkoutsAsync()
        {
            WorkoutGroups.Clear();

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "workout.db");
            var db = new WorkoutDatabaseService(dbPath);
            var workouts = await db.GetWorkoutsAsync();

            // Маппинг Workout → WorkoutCardViewModel
            var cards = workouts
                .OrderByDescending(x => x.StartTime)
                .Select(w => new WorkoutCardViewModel(w))
                .ToList();

            // Группировка по дате (без времени)
            var grouped = cards
                .GroupBy(w => w.StartTime.Date)
                .OrderByDescending(g => g.Key);

            foreach (var group in grouped)
            {
                string dateHeader = group.Key.ToString("dd MMMM yyyy");
                WorkoutGroups.Add(new WorkoutGroup(dateHeader, group));
            }

            OnPropertyChanged(nameof(WorkoutGroups));
            // Текст "нет тренировок"
            OnPropertyChanged(nameof(IsWorkoutListEmpty));
            OnPropertyChanged(nameof(IsWorkoutListNotEmpty));
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }


}
