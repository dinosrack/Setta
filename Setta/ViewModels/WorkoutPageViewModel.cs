using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using Setta.Models;
using Setta.Services;
using System.Runtime.CompilerServices;

public class WorkoutPageViewModel : INotifyPropertyChanged
{
    public ObservableCollection<WorkoutGroup> GroupedWorkouts { get; set; } = new();
    public bool IsWorkoutListEmpty => !GroupedWorkouts.Any();
    public bool IsWorkoutListNotEmpty => GroupedWorkouts.Any();

    public event PropertyChangedEventHandler PropertyChanged;

    public WorkoutPageViewModel()
    {
        LoadWorkouts();
    }

    private async void LoadWorkouts()
    {
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "workout.db");
        var db = new WorkoutDatabaseService(dbPath);
        var workouts = await db.GetWorkoutsAsync();

        // Группировка по датам
        var grouped = workouts
            .GroupBy(w => w.Date.Date)
            .OrderByDescending(g => g.Key)
            .Select(g => new WorkoutGroup(
                g.Key.ToString("d MMMM", new CultureInfo("ru-RU")), // "13 апреля"
                g.OrderByDescending(x => x.StartTime) // порядок внутри дня: новые сверху
            ));

        GroupedWorkouts.Clear();
        foreach (var group in grouped)
            GroupedWorkouts.Add(group);

        OnPropertyChanged(nameof(IsWorkoutListEmpty));
        OnPropertyChanged(nameof(IsWorkoutListNotEmpty));
    }

    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    // Для группировки (дата + коллекция тренировок)
    public class WorkoutGroup : ObservableCollection<Workout>
    {
        public string Date { get; set; }
        public WorkoutGroup(string date, IEnumerable<Workout> workouts) : base(workouts)
        {
            Date = date;
        }
    }
}
