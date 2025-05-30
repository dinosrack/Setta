using CommunityToolkit.Maui.Views;
using Setta.Models;
using Setta.PopupPages;
using Setta.Services;
using System;
using System.IO;

namespace Setta.Pages;

public partial class WorkoutPage : ContentPage
{
    private readonly WorkoutDatabaseService _db;
    public WorkoutPageViewModel ViewModel { get; set; }

    public WorkoutPage()
    {
        InitializeComponent();
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "workout.db");
        _db = new WorkoutDatabaseService(dbPath);

        ViewModel = new WorkoutPageViewModel();
        BindingContext = ViewModel;
    }

    private async void OnAddWorkoutClicked(object sender, EventArgs e)
    {
        var popup = new AddWorkoutPopup(DateTime.Today);
        var result = await this.ShowPopupAsync(popup) as Tuple<DateTime, bool>;
        if (result == null)
            return;

        var date = result.Item1;

        // 1. ¬алидаци€: будуща€ дата
        if (date.Date > DateTime.Today)
        {
            await this.ShowPopupAsync(new ErrorsTemplatesPopup("Ќельз€ записать тренировку в день, который не наступил."));
            return;
        }

        // 2. —оздаЄм новую тренировку
        var workout = new Workout
        {
            Date = date.Date,
            StartTime = date.Date == DateTime.Today ? DateTime.Now : date.Date, // если сегодн€ Ч текущее врем€
            Status = date.Date == DateTime.Today ? WorkoutStatus.Active : WorkoutStatus.Completed,
            // остальные пол€ Ч пустые, будут добавл€тьс€ по ходу (EndTime, TotalWeight, TotalDuration)
        };

        await _db.SaveWorkoutAsync(workout);

        // 3. ќбновить ViewModel (чтобы тренировка по€вилась на главном экране)
        ViewModel?.GroupedWorkouts.Clear();
        ViewModel?.GetType().GetMethod("LoadWorkouts", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.Invoke(ViewModel, null);

        // 4. ќткрываем WorkoutInfoPage (можно по id тренировки)
        await Navigation.PushAsync(new WorkoutInfoPage(workout.Id));
    }
}
