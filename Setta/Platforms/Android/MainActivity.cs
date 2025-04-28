using Android.App;
using Android.Content.PM;
using Android.OS;
using Microsoft.Maui.Platform; // Для .ToPlatform()
using Android.Views;
using Android.Content.Res;

namespace Setta
{
    [Activity(
        Theme = "@style/Maui.SplashTheme",
        MainLauncher = true,
        LaunchMode = LaunchMode.SingleTop,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density
    )]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

#if ANDROID
            SetSystemBarsColor();
#endif
        }

        public void SetSystemBarsColor()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                var window = Window;
                if (window != null)
                {
                    bool isDarkTheme = (Resources.Configuration.UiMode & UiMode.NightMask) == UiMode.NightYes;

                    var navigationColor = isDarkTheme
                        ? Microsoft.Maui.Graphics.Color.FromArgb("#202020") // Тёмная навигация
                        : Microsoft.Maui.Graphics.Color.FromArgb("#FFFFFF"); // Светлая навигация

                    var statusColor = isDarkTheme
                        ? Microsoft.Maui.Graphics.Color.FromArgb("#202020") // Тёмный статус
                        : Microsoft.Maui.Graphics.Color.FromArgb("#FFFFFF"); // Светлый статус

                    window.SetNavigationBarColor(navigationColor.ToPlatform());
                    window.SetStatusBarColor(statusColor.ToPlatform());

                    // Для Android 11+ можно сделать StatusBar текст светлым или тёмным
                    if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
                    {
                        var decorView = window.DecorView;
                        var flags = decorView.SystemUiVisibility;

                        if (isDarkTheme)
                        {
                            // Тёмная тема — белые иконки
                            flags &= ~(StatusBarVisibility)SystemUiFlags.LightStatusBar;
                        }
                        else
                        {
                            // Светлая тема — тёмные иконки
                            flags |= (StatusBarVisibility)SystemUiFlags.LightStatusBar;
                        }

                        decorView.SystemUiVisibility = flags;
                    }
                }
            }
        }
    }
}
