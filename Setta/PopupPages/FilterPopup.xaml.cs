using CommunityToolkit.Maui.Views;
using Setta.ViewModels;

/// <summary>
/// Popup-окно для выбора фильтров (группа мышц и оборудование).
/// Позволяет пользователю выбрать фильтры и применить их к списку упражнений.
/// Использует FilterPageViewModel для логики фильтрации.
/// </summary>

namespace Setta.PopupPages;

public partial class FilterPopup : Popup
{
    readonly FilterPageViewModel _vm;

    public FilterPopup(IEnumerable<string> selectedGroups,
                       IEnumerable<string> selectedEquipment)
    {
        InitializeComponent();
        _vm = new FilterPageViewModel(selectedGroups, selectedEquipment);
        BindingContext = _vm;
    }

    // Применение выбранных фильтров
    async void OnApplyTapped(object sender, EventArgs e)
    {
        await _vm.OnApplyAsync();
        Close();
    }
}
