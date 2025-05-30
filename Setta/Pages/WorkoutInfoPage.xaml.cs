using System;
using Microsoft.Maui.Controls;
using Setta.Models;
using Setta.ViewModels;

namespace Setta.Pages
{
    public partial class WorkoutInfoPage : ContentPage
    {
        private WorkoutInfoPageViewModel ViewModel => BindingContext as WorkoutInfoPageViewModel;

        public WorkoutInfoPage(int workoutId)
        {
            InitializeComponent();
            var vm = new WorkoutInfoPageViewModel(workoutId);
            vm.RequestClose += (s, e) => ClosePage();
            BindingContext = vm;
        }

        private async void OnBackTapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        // Обработчик тапа по упражнению: откроет попап подходов если тренировка активна
        private void OnExerciseTapped(object sender, EventArgs e)
        {
            if (!ViewModel.IsActive)
                return;

            var context = (sender as VisualElement)?.BindingContext as ExerciseInTemplate;
            if (context != null)
                ViewModel.EditExercise(context);
        }

        private async void ClosePage()
        {
            // Просто возвращаемся назад после завершения тренировки
            await Navigation.PopAsync();
        }
    }
}
