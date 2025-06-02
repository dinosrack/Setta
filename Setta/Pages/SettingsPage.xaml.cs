using CommunityToolkit.Maui.Views;
using Setta.PopupPages;

/// <summary>
/// �������� �������� ����������.
/// ��������� ������� ���� ���������� � ��������� �������� �����.
/// ��������� ��������������� ����������� ���� ��� ������ ������� ����.
/// </summary>

namespace Setta.Pages;

public partial class SettingsPage : ContentPage
{

    public SettingsPage()
    {
        InitializeComponent();
        BindingContext = this;
    }

    // ������� ��� ����� ���� ���������� (��������� ThemePopup)
    public Command ThemeSwitch => new Command(async () =>
    {
        var popup = new ThemePopup();
        await this.ShowPopupAsync(popup);
    });

    // ������� ��� �������� ����� (��������� FeedbackPopup)
    public Command Feedback => new Command(async () =>
    {
        var popup = new FeedbackPopup();
        await this.ShowPopupAsync(popup);
    });

}
