using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Setta.Models;
using Setta.Services;
using Setta.Pages;
using System;
using System.Linq;
using System.Diagnostics;

public class WorkoutInfoPageViewModel : INotifyPropertyChanged
{
    private readonly WorkoutDatabaseService _db;
    private int _workoutId;

    public Workout Workout { get; set; }
    public ObservableCollection<ExerciseInTemplate> Exercises { get; set; } = new();

    public bool IsActive => Workout?.Status == WorkoutStatus.Active;
    public bool IsNotActive => Workout?.Status == WorkoutStatus.Completed;

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public WorkoutInfoPageViewModel(int workoutId)
    {
        _db = new WorkoutDatabaseService(Path.Combine(FileSystem.AppDataDirectory, "workout.db"));
        _workoutId = workoutId;
        _ = LoadWorkout();

        // Подписка на ExercisesSelected
        MessagingCenter.Subscribe<AddExerciseToTemplatePage, List<Exercise>>(this, "ExercisesSelected", (sender, selected) =>
        {
            MessagingCenter.Unsubscribe<AddExerciseToTemplatePage, List<Exercise>>(this, "ExercisesSelected");
            if (selected != null)
            {
                foreach (var ex in selected)
                {
                    if (Exercises.Count >= 7) break;
                    if (Exercises.Any(e => e.Exercise.ExerciseName == ex.ExerciseName)) continue;
                    Exercises.Add(new ExerciseInTemplate
                    {
                        Exercise = ex,
                        Sets = new ObservableCollection<ExerciseSet>()
                    });
                }
            }
            OnPropertyChanged(nameof(Exercises));
        });
    }


    private async Task LoadWorkout()
    {
        Workout = await _db.GetWorkoutAsync(_workoutId);
        // TODO: Загрузить упражнения из базы (если реализовано)
        OnPropertyChanged(nameof(Workout));
        OnPropertyChanged(nameof(IsActive));
        OnPropertyChanged(nameof(IsNotActive));
    }

    // КОМАНДА ДОБАВЛЕНИЯ УПРАЖНЕНИЯ
    public Command AddExerciseCommand => new Command(OnAddExercise);

    private async void OnAddExercise()
    {
        var alreadySelected = Exercises.Select(e => e.Exercise).ToList();
        var page = new AddExerciseToTemplatePage(alreadySelected);
        await Application.Current.MainPage.Navigation.PushAsync(page);
    }

    public Command CompleteOrCancelCommand => new Command(OnCompleteWorkout);

    private async void OnCompleteWorkout()
    {
        if (Workout.Status != WorkoutStatus.Active)
            return;

        // 1. Время окончания
        Workout.EndTime = DateTime.Now;

        // 2. Длительность
        var duration = Workout.EndTime.Value - Workout.StartTime;
        Workout.TotalDuration = $"{(int)duration.TotalMinutes} мин {duration.Seconds} сек";

        // 3. Общий вес
        double totalWeight = 0;
        foreach (var exercise in Exercises)
        {
            foreach (var set in exercise.Sets)
            {
                if (int.TryParse(set.Weight, out int weight) && int.TryParse(set.Reps, out int reps))
                    totalWeight += weight * reps;
            }
        }
        Workout.TotalWeight = totalWeight;

        // 4. Статус
        Workout.Status = WorkoutStatus.Completed;

        // 5. Сохраняем в базе
        await _db.SaveWorkoutAsync(Workout);

        // 6. Обновить UI
        OnPropertyChanged(nameof(Workout));
        OnPropertyChanged(nameof(IsActive));
        OnPropertyChanged(nameof(IsNotActive));
        OnPropertyChanged(nameof(Exercises));
    }
}
