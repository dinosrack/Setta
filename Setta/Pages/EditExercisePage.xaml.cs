namespace Setta.Pages;

public partial class EditExercisePage : ContentPage
{
	public EditExercisePage()
	{
		InitializeComponent();
	}

    private async void OnBackTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}