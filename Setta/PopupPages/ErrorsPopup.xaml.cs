using CommunityToolkit.Maui.Views;

/// <summary>
/// Popup-���� ��� ����������� ��������� �� ������.
/// ���������� ����� ������ � ����������� �� ������� ������ "��".
/// ������������ ��� ����������� ������������ � ��������� ����������� ��� ������������ ���������.
/// </summary>

namespace Setta.PopupPages;

public partial class ErrorsPopup : Popup
{
    public ErrorsPopup(string message)
    {
        InitializeComponent();
        MessageLabel.Text = message; // ������������� ����� ������
    }

    // ������� popup �� ������� "��"
    private void OnOkClicked(object sender, EventArgs e)
    {
        Close();
    }
}
