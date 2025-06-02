using Setta.Models;
using Setta.PopupPages;
using Setta.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using Setta.Services;

/// <summary>
/// Страница добавления нового упражнения в приложение.
/// Реализует выбор основной и второстепенной группы мышц, оборудования, а также валидацию введённых данных.
/// После успешного добавления вызывает сервис сохранения в базу данных и возвращает пользователя назад.
/// Использует всплывающие окна для выбора групп мышц и оборудования.
/// </summary>

namespace Setta.Pages
{
    public partial class AddExercisePage : ContentPage
    {
        private const string PlaceholderUnselectedMuscle = "Не выбрана";
        private const string PlaceholderUnselectedEquipment = "Не выбрано";

        public AddExercisePage()
        {
            InitializeComponent();
        }

        // Обработка нажатия на основную группу мышц
        async void OnPrimaryMuscleTapped(object sender, EventArgs e)
        {
            var isPlaceholder = string.IsNullOrWhiteSpace(PrimaryMuscleLabel.Text)
                                || PrimaryMuscleLabel.Text == PlaceholderUnselectedMuscle;
            var initial = isPlaceholder
                ? Array.Empty<string>()
                : new[] { PrimaryMuscleLabel.Text };

            var popup = new SelectionPopup(
                "Основная группа",
                FilterPageViewModel.GetAvailableMuscleGroups(),
                initial,
                isMultiSelect: false);

            var result = await this.ShowPopupAsync(popup) as List<string>;
            if (result?.Any() == true)
            {
                PrimaryMuscleLabel.Text = result.First();
            }
        }

        // Обработка нажатия на второстепенные группы мышц
        async void OnSecondaryMusclesTapped(object sender, EventArgs e)
        {
            var isPlaceholder = string.IsNullOrWhiteSpace(SecondaryMuscleLabel.Text)
                                || SecondaryMuscleLabel.Text == PlaceholderUnselectedMuscle;

            var selected = isPlaceholder
                ? Array.Empty<string>()
                : SecondaryMuscleLabel.Text.Split(", ", StringSplitOptions.RemoveEmptyEntries);

            var popup = new SelectionPopup(
                "Второстепенная группа",
                FilterPageViewModel.GetAvailableMuscleGroups(),
                selected,
                isMultiSelect: true);

            var result = await this.ShowPopupAsync(popup) as List<string>;
            if (result != null)
            {
                SecondaryMuscleLabel.Text = result.Any()
                    ? string.Join(", ", result)
                    : PlaceholderUnselectedMuscle;
            }
        }

        // Обработка нажатия на выбор оборудования
        async void OnEquipmentTapped(object sender, EventArgs e)
        {
            var isPlaceholder = string.IsNullOrWhiteSpace(EquipmentLabel.Text)
                                || EquipmentLabel.Text == PlaceholderUnselectedEquipment;
            var initial = isPlaceholder
                ? Array.Empty<string>()
                : new[] { EquipmentLabel.Text };

            var popup = new SelectionPopup(
                "Оборудование",
                FilterPageViewModel.GetAvailableEquipment(),
                initial,
                isMultiSelect: false);

            var result = await this.ShowPopupAsync(popup) as List<string>;
            if (result?.Any() == true)
            {
                EquipmentLabel.Text = result.First();
            }
        }

        // Добавление упражнения по кнопке
        async void OnAddExerciseClicked(object sender, EventArgs e)
        {
            bool hasError = false;

            // Валидация названия упражнения
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
            if (string.IsNullOrWhiteSpace(PrimaryMuscleLabel.Text)
                || PrimaryMuscleLabel.Text == PlaceholderUnselectedMuscle)
            {
                PrimaryMuscleErrorLabel.IsVisible = true;
                hasError = true;
            }
            else
            {
                PrimaryMuscleErrorLabel.IsVisible = false;
            }

            // Валидация оборудования
            if (string.IsNullOrWhiteSpace(EquipmentLabel.Text)
                || EquipmentLabel.Text == PlaceholderUnselectedEquipment)
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

            var secondaryText = SecondaryMuscleLabel.Text;
            var isSecondaryValid = !string.IsNullOrWhiteSpace(secondaryText)
                                   && secondaryText != PlaceholderUnselectedMuscle;

            // Формируем новый объект упражнения
            var newExercise = new Exercise
            {
                ExerciseName = ExerciseNameEntry.Text.Trim(),
                MuscleGroup = PrimaryMuscleLabel.Text,
                SecondaryMuscleGroup = isSecondaryValid ? secondaryText : string.Empty,
                Equipment = EquipmentLabel.Text
            };

            await ExerciseDatabaseService.AddExerciseAsync(newExercise);

            // Уведомляем остальные части приложения о добавлении нового упражнения
            MessagingCenter.Send(this, "ExerciseAdded", newExercise);
            await Navigation.PopAsync();
        }

        // Возврат на предыдущую страницу
        async void OnBackTapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
