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

    private async void OnEmptyWorkoutClicked(object sender, EventArgs e)
    {
        var selectedDate = WorkoutDatePicker.Date;

        var allWorkouts = await WorkoutDatabaseService.GetWorkoutsAsync();

        // Проверка на активную тренировку
        if (allWorkouts.Any(w => w.EndDateTime == null))
        {
            await Application.Current.MainPage.ShowPopupAsync(
                new ErrorsTemplatesPopup("Нельзя начать новую тренировку, пока есть активная."));
            return;
        }

        // Подсчёт тренировок на выбранную дату
        int countForDate = allWorkouts.Count(w => w.StartDateTime.Date == selectedDate.Date);
        if (countForDate >= 2)
        {
            await Application.Current.MainPage.ShowPopupAsync(
                new ErrorsTemplatesPopup("Вы можете записать не более 2 тренировок за 1 день."));
            return;
        }

        var workout = new Workout
        {
            StartDateTime = selectedDate.Date.Add(DateTime.Now.TimeOfDay),
            EndDateTime = null,
            TotalWeight = 0
        };

        int id = await WorkoutDatabaseService.AddWorkoutAsync(workout);
        workout.Id = id;

        await this.CloseAsync();
        await Shell.Current.Navigation.PushAsync(new AddWorkoutPage(workout));
    }
}