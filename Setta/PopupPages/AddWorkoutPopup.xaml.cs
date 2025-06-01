using CommunityToolkit.Maui.Views;
using Setta.ViewModels;
using Setta.Pages;
using Setta.Services;
using Setta.Models;

namespace Setta.PopupPages;
public partial class AddWorkoutPopup : Popup
{
    public AddWorkoutPopup()
    {
        InitializeComponent();
    }

    private async void OnContinueClicked(object sender, EventArgs e)
    {
        var selectedDate = WorkoutDatePicker.Date.Date;
        var now = DateTime.Today;

        // Нельзя выбрать дату в будущем
        if (selectedDate > now)
        {
            await Shell.Current.CurrentPage.ShowPopupAsync(new ErrorsTemplatesPopup(
                "Нельзя создать тренировку на будущее. Выберите сегодняшнюю или прошедшую дату."));
            return;
        }

        var allWorkouts = await WorkoutDatabaseService.GetWorkoutsAsync();

        // Ограничение: не более 2 тренировок на выбранный день
        int countForDate = allWorkouts.Count(w => w.StartDateTime.Date == selectedDate);
        if (countForDate >= 2)
        {
            await Shell.Current.CurrentPage.ShowPopupAsync(new ErrorsTemplatesPopup(
                "Вы можете записать не более 2 тренировок за 1 день."));
            return;
        }

        var workout = new Workout
        {
            StartDateTime = selectedDate.Add(DateTime.Now.TimeOfDay),
            EndDateTime = null,
            TotalWeight = 0
        };

        int id = await WorkoutDatabaseService.AddWorkoutAsync(workout);
        workout.Id = id;

        await this.CloseAsync();
        await Shell.Current.Navigation.PushAsync(new AddWorkoutPage(workout));
    }
}
