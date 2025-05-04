using Setta.Models;
using Setta.PopupPages;
using Setta.Services;
using Setta.ViewModels;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace Setta.Pages
{
    public partial class EditExercisePage : ContentPage
    {
        private readonly Exercise _exercise;

        public EditExercisePage(Exercise exercise)
        {
            InitializeComponent();
            _exercise = exercise;

            // Заполняем поля
            ExerciseNameEntry.Text = _exercise.ExerciseName;
            PrimaryMuscleLabel.Text = _exercise.MuscleGroup;
            SecondaryMuscleLabel.Text = string.IsNullOrWhiteSpace(_exercise.SecondaryMuscleGroup)
                ? "Выберите группу"
                : _exercise.SecondaryMuscleGroup;
            EquipmentLabel.Text = _exercise.Equipment;
        }

        async void OnPrimaryMuscleTapped(object sender, EventArgs e)
        {
            var popup = new SelectionPopup(
                "Основная группа",
                FilterPageViewModel.GetAvailableMuscleGroups(),
                new[] { PrimaryMuscleLabel.Text },
                isMultiSelect: false);

            var result = await this.ShowPopupAsync(popup) as List<string>;
            if (result?.Any() == true)
                PrimaryMuscleLabel.Text = result.First();
        }

        async void OnSecondaryMusclesTapped(object sender, EventArgs e)
        {
            var current = string.IsNullOrWhiteSpace(SecondaryMuscleLabel.Text) ||
                          SecondaryMuscleLabel.Text.StartsWith("Выберите")
                ? Array.Empty<string>()
                : SecondaryMuscleLabel.Text.Split(", ", StringSplitOptions.RemoveEmptyEntries);

            var popup = new SelectionPopup(
                "Второстепенная группа",
                FilterPageViewModel.GetAvailableMuscleGroups(),
                current,
                isMultiSelect: true);

            var result = await this.ShowPopupAsync(popup) as List<string>;
            if (result != null)
                SecondaryMuscleLabel.Text = result.Any()
                    ? string.Join(", ", result)
                    : "Выберите группу";
        }

        async void OnEquipmentTapped(object sender, EventArgs e)
        {
            var popup = new SelectionPopup(
                "Оборудование",
                FilterPageViewModel.GetAvailableEquipment(),
                new[] { EquipmentLabel.Text },
                isMultiSelect: false);

            var result = await this.ShowPopupAsync(popup) as List<string>;
            if (result?.Any() == true)
                EquipmentLabel.Text = result.First();
        }

        async void OnSaveClicked(object sender, EventArgs e)
        {
            bool hasError = false;

            // Валидация названия
            if (string.IsNullOrWhiteSpace(ExerciseNameEntry.Text))
            {
                NameErrorLabel.IsVisible = true;
                hasError = true;
            }
            else
            {
                NameErrorLabel.IsVisible = false;
            }

            // Валидация основной группы мышц
            if (string.IsNullOrWhiteSpace(PrimaryMuscleLabel.Text) ||
                PrimaryMuscleLabel.Text == "Выберите группу")
            {
                PrimaryMuscleErrorLabel.IsVisible = true;
                hasError = true;
            }
            else
            {
                PrimaryMuscleErrorLabel.IsVisible = false;
            }

            // Валидация оборудования
            if (string.IsNullOrWhiteSpace(EquipmentLabel.Text) ||
                EquipmentLabel.Text == "Выберите оборудование")
            {
                EquipmentErrorLabel.IsVisible = true;
                hasError = true;
            }
            else
            {
                EquipmentErrorLabel.IsVisible = false;
            }

            if (hasError)
                return;

            // Сохранение изменений
            _exercise.ExerciseName = ExerciseNameEntry.Text.Trim();
            _exercise.MuscleGroup = PrimaryMuscleLabel.Text;
            _exercise.SecondaryMuscleGroup = SecondaryMuscleLabel.Text.StartsWith("Выберите")
                ? string.Empty
                : SecondaryMuscleLabel.Text;
            _exercise.Equipment = EquipmentLabel.Text;

            await ExerciseDatabaseService.UpdateExerciseAsync(_exercise);
            MessagingCenter.Send(this, "ExerciseUpdated", _exercise);
            await Navigation.PopAsync();
        }

        async void OnDeleteClicked(object sender, EventArgs e)
        {
            var popup = new DeleteExercisePopup();
            var result = await this.ShowPopupAsync(popup) as bool?;

            if (result == true)
            {
                await ExerciseDatabaseService.DeleteExerciseAsync(_exercise);
                MessagingCenter.Send(this, "ExerciseDeleted", _exercise);
                await Shell.Current.Navigation.PopToRootAsync();
            }
        }

        async void OnBackTapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}