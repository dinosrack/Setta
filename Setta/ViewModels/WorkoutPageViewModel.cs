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

        public bool IsWorkoutListEmpty => Workouts.Count == 0;
        public bool IsWorkoutListNotEmpty => !IsWorkoutListEmpty;

        // Добавь сюда метод загрузки
        public async Task LoadWorkoutsAsync()
        {
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "workout.db");
            var db = new WorkoutDatabaseService(dbPath);
            var workouts = await db.GetWorkoutsAsync();
            Workouts.Clear();
            foreach (var w in workouts)
                Workouts.Add(w);
            OnPropertyChanged(nameof(IsWorkoutListEmpty));
            OnPropertyChanged(nameof(IsWorkoutListNotEmpty));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }


}
