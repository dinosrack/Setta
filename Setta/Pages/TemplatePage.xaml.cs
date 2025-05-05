namespace Setta.Pages;

public partial class TemplatePage : ContentPage
{
	public TemplatePage()
	{
		InitializeComponent();
	}

    private async void OnAddTemplateClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddTemplatePage());
    }
}