namespace Setta.Pages;

public partial class ChooseTemplatePage : ContentPage
{
    public ChooseTemplatePage()
    {
        InitializeComponent();
    }

    public ChooseTemplatePage(DateTime date)
    {
        InitializeComponent();
    }

    private async void OnBackTapped(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}