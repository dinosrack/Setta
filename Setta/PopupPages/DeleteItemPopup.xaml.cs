using CommunityToolkit.Maui.Views;

namespace Setta.PopupPages
{
    public partial class DeleteItemPopup : Popup
    {
        public Action OnConfirmAction { get; set; }

        public DeleteItemPopup()
        {
            InitializeComponent();
        }

        private void OnCancelClicked(object sender, EventArgs e)
        {
            Close(false);
        }

        private void OnConfirmClicked(object sender, EventArgs e)
        {
            Close(true);
            OnConfirmAction?.Invoke();
        }
    }
}
