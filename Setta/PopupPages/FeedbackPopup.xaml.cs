using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Platform;

/// <summary>
/// Popup-окно для отправки обратной связи.
/// Позволяет пользователю перейти по ссылке в Telegram для связи с разработчиком.
/// </summary>

namespace Setta.PopupPages
{
    public partial class FeedbackPopup : Popup
    {
        public FeedbackPopup()
        {
            InitializeComponent();
            BindingContext = this;
        }

        // Открытие Telegram по нажатию на кнопку/ссылку
        private async void OnTelegramTapped(object sender, EventArgs e)
        {
            var uri = new Uri("https://t.me/redin_alxr");
            await Launcher.Default.OpenAsync(uri);
        }
    }
}
