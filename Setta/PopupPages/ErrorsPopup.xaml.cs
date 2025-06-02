using CommunityToolkit.Maui.Views;

/// <summary>
/// Popup-окно для отображения сообщений об ошибке.
/// Показывает текст ошибки и закрывается по нажатию кнопки "Ок".
/// Используется для уведомления пользователя о нарушении ограничений или некорректных действиях.
/// </summary>

namespace Setta.PopupPages;

public partial class ErrorsPopup : Popup
{
    public ErrorsPopup(string message)
    {
        InitializeComponent();
        MessageLabel.Text = message; // Устанавливаем текст ошибки
    }

    // Закрыть popup по нажатию "Ок"
    private void OnOkClicked(object sender, EventArgs e)
    {
        Close();
    }
}
