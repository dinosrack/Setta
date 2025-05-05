namespace Setta.Pages;

public partial class AddTemplatePage : ContentPage
{
	public AddTemplatePage()
	{
		InitializeComponent();
	}

    async void OnBackTapped(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}