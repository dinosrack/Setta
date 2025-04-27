using CommunityToolkit.Maui.Views;

namespace Setta.PopupPages;

public partial class ThemePopup : Popup
{
	public ThemePopup()
	{
		InitializeComponent();
    }

    private void OnCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (sender is RadioButton radioButton && e.Value)
        {
            string selectedTheme = (string)radioButton.Value;

            switch (selectedTheme)
            {
                case "Light":
                    App.Current.UserAppTheme = AppTheme.Light;
                    break;
                case "Dark":
                    App.Current.UserAppTheme = AppTheme.Dark;
                    break;
                case "Auto":
                    App.Current.UserAppTheme = AppTheme.Unspecified;
                    break;
            }

        }
    }
}