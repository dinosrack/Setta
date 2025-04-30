using CommunityToolkit.Maui.Views;
using Setta.ViewModels;

namespace Setta.PopupPages;

public partial class FilterPopup : Popup
{
    readonly FilterPageViewModel _vm;

    public FilterPopup(IEnumerable<string> selectedGroups)
    {
        InitializeComponent();
        _vm = new FilterPageViewModel(selectedGroups);
        BindingContext = _vm;
    }

    async void OnApplyTapped(object sender, EventArgs e)
    {
        // вызываем Apply из VM Ч он шлЄт MessagingCenter
        await _vm.OnApplyAsync();

        // и закрываем попап
        Close();
    }
}