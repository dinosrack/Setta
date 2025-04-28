namespace Setta
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            string selectedTheme = Preferences.Get("SelectedTheme", "Auto");

            switch (selectedTheme)
            {
                case "Light":
                    App.Current.UserAppTheme = AppTheme.Light;
                    break;
                case "Dark":
                    App.Current.UserAppTheme = AppTheme.Dark;
                    break;
                default:
                    App.Current.UserAppTheme = AppTheme.Unspecified;
                    break;
            }

            MainPage = new AppShell();
        }
    }
}
