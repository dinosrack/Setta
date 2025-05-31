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
        var selectedDate = WorkoutDatePicker.Date;

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