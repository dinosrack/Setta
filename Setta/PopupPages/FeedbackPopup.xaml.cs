using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Platform;

/// <summary>
/// Popup-���� ��� �������� �������� �����.
/// ��������� ������������ ������� �� ������ � Telegram ��� ����� � �������������.
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

        // �������� Telegram �� ������� �� ������/������
        private async void OnTelegramTapped(object sender, EventArgs e)
        {
            var uri = new Uri("https://t.me/redin_alxr");
            await Launcher.Default.OpenAsync(uri);
        }
    }
}
