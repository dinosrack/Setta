using System;
using Microsoft.Maui.Controls;
using Setta.Models;
using Setta.ViewModels;

namespace Setta.Pages
{
    public partial class WorkoutInfoPage : ContentPage
    { 
        public WorkoutInfoPage()
        {
            InitializeComponent();
        }

        private async void OnBackTapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
