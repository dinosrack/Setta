using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Platform; // Для Platform.CurrentActivity

namespace Setta.PopupPages
{
    public partial class ThemePopup : Popup
    {
        public ThemePopup()
        {
            InitializeComponent();
        }

        private void OnCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (sender is RadioButton radioButton && e.Value && radioButton.Value is string selectedTheme)
            {
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
                    default:
                        App.Current.UserAppTheme = AppTheme.Unspecified;
                        break;
                }

                Preferences.Set("SelectedTheme", selectedTheme); // Сохраняем выбранную тему

#if ANDROID
                (Platform.CurrentActivity as MainActivity)?.SetSystemBarsColor();
#endif
            }
        }

        private void OnPopupOpened(object? sender, EventArgs e)
        {
            string selectedTheme = Preferences.Get("SelectedTheme", "Auto");

            foreach (var view in (Content as Frame)?.Content as VerticalStackLayout)
            {
                if (view is RadioButton radioButton && radioButton.Value is string value)
                {
                    radioButton.IsChecked = value == selectedTheme;
                }
            }
        }
    }
}
