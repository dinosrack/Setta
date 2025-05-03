using CommunityToolkit.Maui.Views;
using Setta.ViewModels;

namespace Setta.PopupPages;

public partial class FilterPopup : Popup
{
    readonly FilterPageViewModel _vm;

    // теперь конструктор принимает два списка
    public FilterPopup(IEnumerable<string> selectedGroups,
                       IEnumerable<string> selectedEquipment)
    {
        InitializeComponent();
        _vm = new FilterPageViewModel(selectedGroups, selectedEquipment);
        BindingContext = _vm;
    }

    async void OnApplyTapped(object sender, EventArgs e)
    {
        await _vm.OnApplyAsync();
        Close();
    }
}