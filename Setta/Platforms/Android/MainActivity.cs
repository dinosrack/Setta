using Android.App;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using Microsoft.Maui.Platform; // Нужно для ToPlatform()

namespace Setta
{
    [Activity(Theme = "@style/Maui.SplashTheme",
              MainLauncher = true,
              LaunchMode = LaunchMode.SingleTop,
              ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

#if ANDROID
            SetNavigationBarColor();
#endif
        }

        private void SetNavigationBarColor()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                var window = Window;
                if (window != null)
                {
                    bool isDarkTheme = (Resources.Configuration.UiMode & UiMode.NightMask) == UiMode.NightYes;

                    var color = isDarkTheme
                        ? Microsoft.Maui.Graphics.Color.FromArgb("#202020") // Темная тема
                        : Microsoft.Maui.Graphics.Color.FromArgb("#FFFFFF"); // Светлая тема

                    window.SetNavigationBarColor(color.ToPlatform());
                }
            }
        }
    }
}
