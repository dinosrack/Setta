using CommunityToolkit.Maui.Views;

namespace Setta.PopupPages;

public partial class ErrorsTemplatesPopup : Popup
{
    public ErrorsTemplatesPopup(string message)
    {
        InitializeComponent();
        MessageLabel.Text = message;
    }

    private void OnOkClicked(object sender, EventArgs e)
    {
        Close();
    }
}
