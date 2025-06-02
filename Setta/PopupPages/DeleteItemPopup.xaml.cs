using CommunityToolkit.Maui.Views;

/// <summary>
/// Popup-окно для подтверждения удаления элемента.
/// Позволяет отменить или подтвердить удаление, возвращает результат через Close и вызывает дополнительное действие по подтверждению.
/// Используется в интерфейсе при удалении упражнения, тренировки или шаблона.
/// </summary>

namespace Setta.PopupPages
{
    public partial class DeleteItemPopup : Popup
    {
        public Action OnConfirmAction { get; set; } // Действие, вызываемое при подтверждении

        public DeleteItemPopup()
        {
            InitializeComponent();
        }

        // Отмена удаления
        private void OnCancelClicked(object sender, EventArgs e)
        {
            Close(false);
        }

        // Подтверждение удаления
        private void OnConfirmClicked(object sender, EventArgs e)
        {
            Close(true);
            OnConfirmAction?.Invoke();
        }
    }
}
