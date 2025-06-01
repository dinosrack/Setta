using CommunityToolkit.Maui.Views;

namespace Setta.PopupPages;

public partial class ErrorsPopup : Popup
{
    public ErrorsPopup(string message)
    {
        InitializeComponent();
        MessageLabel.Text = message;
    }

    private void OnOkClicked(object sender, EventArgs e)
    {
        Close();
    }
}
