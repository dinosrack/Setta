using Setta.Models;
using Setta.ViewModels;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;

namespace Setta.AnotherPages
{
    public partial class FilterPage : ContentPage
    {
        public FilterPage(IEnumerable<string> selectedGroups)
        {
            InitializeComponent();
            BindingContext = new FilterPageViewModel(selectedGroups);
        }

        private async void OnCloseTapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync(animated: false);
        }
    }
}