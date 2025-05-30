using CommunityToolkit.Maui.Views;
using Setta.PopupPages;

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
}