using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Platform; // Äëÿ Platform.CurrentActivity

namespace Setta.PopupPages
{
    public partial class FeedbackPopup : Popup
    {
        public FeedbackPopup()
        {
            InitializeComponent();
            BindingContext = this;
        }

        private async void OnTelegramTapped(object sender, EventArgs e)
        {
            var uri = new Uri("https://t.me/redin_alxr");
            await Launcher.Default.OpenAsync(uri);
        }
    }
}
