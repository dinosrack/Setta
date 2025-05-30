using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using Setta.Models;
using Setta.Pages;
using Setta.PopupPages;
using Setta.Services;

namespace Setta.ViewModels
{
    public class WorkoutInfoPageViewModel : BindableObject
    {
        private readonly WorkoutDatabaseService _db;
        private Workout _workout;

        public ObservableCollection<ExerciseInTemplate> Exercises { get; set; } = new();

        public bool IsActive => _workout.IsActive;
        public bool IsNotActive => !_workout.IsActive;

        public string DateTimePeriod
        {
            get
            {
                if (_workout.EndTime == null) return string.Empty;
                return $"{_workout.StartTime:HH:mm} - {_workout.EndTime:HH:mm}";
            }
        }

        public int TotalWeight => Exercises.Sum(e => e.TotalVolume);

        public string TotalDuration
        {
            get
            {
                if (_workout.EndTime == null) return "—";
                var min = _workout.TotalDuration;
                return $"{min} мин";
            }
        }

        // Для биндинга на XAML
        public ICommand AddExerciseCommand { get; }
        public ICommand CompleteOrCancelCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }

        public event EventHandler RequestClose; // Для завершения навигации из VM

        public WorkoutInfoPageViewModel(int workoutId)
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "workout.db");
            _db = new WorkoutDatabaseService(dbPath);
            Task.Run(async () => await LoadWorkoutAsync(workoutId)).Wait();

            AddExerciseCommand = new Command(OnAddExercise);
            CompleteOrCancelCommand = new Command(OnCompleteWorkout);
            // DeleteCommand и SaveCommand — по желанию, если потребуется.
        }

        private async Task LoadWorkoutAsync(int workoutId)
        {
            _workout = await _db.GetWorkoutByIdAsync(workoutId);

            // Если есть упражнения — загрузим
            if (!string.IsNullOrEmpty(_workout.ExercisesJson))
            {
                try
                {
                    var list = JsonSerializer.Deserialize<ObservableCollection<ExerciseInTemplate>>(_workout.ExercisesJson);
                    Exercises = list ?? new ObservableCollection<ExerciseInTemplate>();
                }
                catch
                {
                    Exercises = new ObservableCollection<ExerciseInTemplate>();
                }
            }
            else
            {
                Exercises = new ObservableCollection<ExerciseInTemplate>();
            }

            OnPropertyChanged(nameof(Exercises));
            OnPropertyChanged(nameof(IsActive));
            OnPropertyChanged(nameof(IsNotActive));
            OnPropertyChanged(nameof(DateTimePeriod));
            OnPropertyChanged(nameof(TotalWeight));
            OnPropertyChanged(nameof(TotalDuration));
        }

        private async void OnAddExercise()
        {
            if (Exercises.Count >= 7)
            {
                await App.Current.MainPage.ShowPopupAsync(new ErrorsTemplatesPopup("Можно добавить не более 7 упражнений."));
                return;
            }

            var alreadySelected = Exercises.Select(x => x.Exercise).ToList();
            var page = new AddExerciseToTemplatePage(alreadySelected);

            MessagingCenter.Subscribe<AddExerciseToTemplatePage, List<Exercise>>(this, "ExercisesSelected", (sender, selected) =>
            {
                MessagingCenter.Unsubscribe<AddExerciseToTemplatePage, List<Exercise>>(this, "ExercisesSelected");
                if (selected == null || selected.Count == 0) return;

                foreach (var ex in selected)
                {
                    if (Exercises.Any(e => e.Exercise.ExerciseName == ex.ExerciseName)) continue;
                    Exercises.Add(new ExerciseInTemplate
                    {
                        Exercise = ex,
                        Sets = new ObservableCollection<ExerciseSet>()
                    });
                }
                OnPropertyChanged(nameof(Exercises));
                SaveExercises();
            });

            await App.Current.MainPage.Navigation.PushAsync(page);
        }

        // Вызови этот метод после изменения Exercises
        public async void SaveExercises()
        {
            _workout.ExercisesJson = JsonSerializer.Serialize(Exercises);
            _workout.TotalWeight = TotalWeight; // Можно обновлять сразу, если нужно
            await _db.SaveWorkoutAsync(_workout);

            OnPropertyChanged(nameof(TotalWeight));
        }

        // Вызывать по тапу на упражнение для открытия попапа подходов
        public async void EditExercise(ExerciseInTemplate selected)
        {
            var popup = new ProgressInExercisesPopup(selected);
            var result = await App.Current.MainPage.ShowPopupAsync(popup);

            if (popup.IsSaved == false) return;

            if (result == null)
            {
                Exercises.Remove(selected);
            }
            else if (result is ExerciseInTemplate updated)
            {
                var idx = Exercises.IndexOf(selected);
                if (idx >= 0)
                    Exercises[idx] = updated;
            }
            OnPropertyChanged(nameof(Exercises));
            SaveExercises();
        }

        private async void OnCompleteWorkout()
        {
            if (!Exercises.Any())
            {
                await App.Current.MainPage.ShowPopupAsync(new ErrorsTemplatesPopup("Добавьте хотя бы одно упражнение!"));
                return;
            }

            _workout.IsActive = false;
            _workout.EndTime = DateTime.Now;
            _workout.TotalWeight = TotalWeight;
            _workout.TotalDuration = $"{(int)(_workout.EndTime.Value - _workout.StartTime).TotalMinutes} мин";
            SaveExercises();

            OnPropertyChanged(nameof(IsActive));
            OnPropertyChanged(nameof(IsNotActive));
            OnPropertyChanged(nameof(DateTimePeriod));
            OnPropertyChanged(nameof(TotalWeight));
            OnPropertyChanged(nameof(TotalDuration));

            await App.Current.MainPage.ShowPopupAsync(new ErrorsTemplatesPopup("Тренировка завершена!"));
            // Можно закрыть страницу после завершения
            RequestClose?.Invoke(this, EventArgs.Empty);
        }
    }
}
