using CommunityToolkit.Maui.Views;
using Setta.PopupPages;

/// <summary>
/// Страница настроек приложения.
/// Позволяет сменить тему оформления и отправить обратную связь.
/// Открывает соответствующие всплывающие окна при выборе пунктов меню.
/// </summary>

namespace Setta.Pages;

public partial class SettingsPage : ContentPage
{

    public SettingsPage()
    {
        InitializeComponent();
        BindingContext = this;
    }

    // Команда для смены темы приложения (открывает ThemePopup)
    public Command ThemeSwitch => new Command(async () =>
    {
        var popup = new ThemePopup();
        await this.ShowPopupAsync(popup);
    });

    // Команда для обратной связи (открывает FeedbackPopup)
    public Command Feedback => new Command(async () =>
    {
        var popup = new FeedbackPopup();
        await this.ShowPopupAsync(popup);
    });

}
