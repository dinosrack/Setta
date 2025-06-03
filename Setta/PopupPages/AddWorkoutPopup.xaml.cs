using CommunityToolkit.Maui.Views;
using Setta.ViewModels;
using Setta.Pages;
using Setta.Services;
using Setta.Models;

/// <summary>
/// Popup-окно дл€ создани€ новой тренировки.
/// ѕозвол€ет выбрать дату, провер€ет ограничени€ (не более 2 тренировок на день, нельз€ в будущем),
/// создаЄт новую тренировку и переходит к еЄ редактированию.
/// »спользует popup дл€ вывода ошибок.
/// </summary>

namespace Setta.PopupPages;
public partial class AddWorkoutPopup : Popup
{
    public AddWorkoutPopup()
    {
        InitializeComponent();
    }

    // ќбработка нажати€ на "ѕродолжить"
    private async void OnContinueClicked(object sender, EventArgs e)
    {
        var selectedDate = WorkoutDatePicker.Date.Date;
        var now = DateTime.Now;

        // Ќельз€ выбрать дату в будущем
        if (selectedDate > now.Date)
        {
            await Shell.Current.CurrentPage.ShowPopupAsync(new ErrorsPopup(
                "Ќельз€ создать тренировку на будущее. ¬ыберите сегодн€шнюю или прошедшую дату."));
            return;
        }

        var allWorkouts = await WorkoutDatabaseService.GetWorkoutsAsync();

        // ќграничение: не более 2 тренировок на выбранный день
        int countForDate = allWorkouts.Count(w => w.StartDateTime.Date == selectedDate);
        if (countForDate >= 2)
        {
            await Shell.Current.CurrentPage.ShowPopupAsync(new ErrorsPopup(
                "¬ы можете записать не более 2 тренировок за 1 день."));
            return;
        }

        // —оздание новой тренировки
        var workoutDate = selectedDate.Date;

        DateTime startDateTime;
        DateTime? endDateTime = null;

        if (workoutDate == now.Date)
        {
            // —егодн€: активна€ тренировка, начинаетс€ сейчас
            startDateTime = now;
        }
        else
        {
            // ѕрошла€ дата: фиксированное врем€ начала и окончани€
            startDateTime = workoutDate.AddHours(12);
            endDateTime = workoutDate.AddHours(13); // строго через 1 час, тоже по дате workoutDate
        }

        var workout = new Workout
        {
            StartDateTime = startDateTime,
            EndDateTime = endDateTime,
            TotalWeight = 0
        };

        int id = await WorkoutDatabaseService.AddWorkoutAsync(workout);
        workout.Id = id;

        await this.CloseAsync();
        await Shell.Current.Navigation.PushAsync(new AddWorkoutPage(workout));
    }
}
