using CommunityToolkit.Maui.Views;
using Setta.PopupPages;
using Setta.Services;
using Setta.Models;
using System.Windows.Input;

namespace Setta.Pages;

public partial class WorkoutPage : ContentPage
{
	public WorkoutPage()
	{
		InitializeComponent();
	}

    private async void OnAddWorkoutClicked(object sender, EventArgs e)
    {
        var popup = new AddWorkoutPopup();
        await this.ShowPopupAsync(popup);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadWorkouts();
    }

    private async void LoadWorkouts()
    {
        var workouts = await WorkoutDatabaseService.GetWorkoutsAsync();

        var viewItems = workouts.Select(w => new WorkoutViewItem
        {
            Id = w.Id,
            StartDateTime = w.StartDateTime,
            EndDateTime = w.EndDateTime,
            TotalWeight = w.TotalWeight
        });

        var grouped = viewItems
            .GroupBy(w => w.StartDateTime.Date)
            .OrderByDescending(g => g.Key)
            .Select(g => new WorkoutGroup(
                g.Key.ToString("d MMMM", new System.Globalization.CultureInfo("ru-RU")),
                g.OrderByDescending(w => w.StartDateTime))) 
            .ToList();

        WorkoutView.ItemsSource = grouped;
        WorkoutView.IsVisible = grouped.Any();
        NoWorkoutLabel.IsVisible = !grouped.Any();
    }

    public ICommand WorkoutTapCommand => new Command<WorkoutViewItem>(async (item) =>
    {
        if (item == null) return;

        var workout = await WorkoutDatabaseService.GetWorkoutByIdAsync(item.Id);
        if (workout != null)
            await Navigation.PushAsync(new AddWorkoutPage(workout));
    });
}