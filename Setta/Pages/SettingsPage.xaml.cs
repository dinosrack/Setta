using CommunityToolkit.Maui.Views;
using Setta.PopupPages;

namespace Setta.Pages;

public partial class SettingsPage : ContentPage
{

    public SettingsPage()
	{
		InitializeComponent();
        BindingContext = this;
    }

    public Command ThemeSwitch => new Command(async () =>
    {
        var popup = new ThemePopup();
        await this.ShowPopupAsync(popup);
    });

}