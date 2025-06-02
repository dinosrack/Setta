using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Platform;

/// <summary>
/// Popup-окно для смены темы приложения (Светлая, Тёмная, Авто).
/// Сохраняет выбранную тему в Preferences и применяет её немедленно.
/// На Android меняет цвет системных панелей через MainActivity.
/// </summary>

namespace Setta.PopupPages
{
    public partial class ThemePopup : Popup
    {
        public ThemePopup()
        {
            InitializeComponent();
        }

        // Обработка изменения выбора темы
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

        // Автоматически отмечаем RadioButton при открытии popup в соответствии с текущей темой
        private void OnPopupOpened(object? sender, EventArgs e)
        {
            string selectedTheme = Preferences.Get("SelectedTheme", "Auto");

            if (Content is Grid grid)
            {
                if (grid.Children.FirstOrDefault() is Border border)
                {
                    if (border.Content is VerticalStackLayout layout)
                    {
                        foreach (var view in layout.Children)
                        {
                            if (view is RadioButton radioButton && radioButton.Value is string value)
                            {
                                radioButton.IsChecked = value == selectedTheme;
                            }
                        }
                    }
                }
            }
        }
    }
}
