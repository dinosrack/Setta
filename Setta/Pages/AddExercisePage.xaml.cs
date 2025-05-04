using Setta.Models;
using Setta.PopupPages;
using Setta.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using Setta.Services;

namespace Setta.Pages
{
    public partial class AddExercisePage : ContentPage
    {
        public AddExercisePage()
        {
            InitializeComponent();
        }

        async void OnPrimaryMuscleTapped(object sender, EventArgs e)
        {
            var popup = new SelectionPopup(
                "�������� ������",
                FilterPageViewModel.GetAvailableMuscleGroups(),  // ������� + �� Excel
                string.IsNullOrWhiteSpace(PrimaryMuscleLabel.Text) || PrimaryMuscleLabel.Text == "��������..."
                    ? Array.Empty<string>()
                    : new[] { PrimaryMuscleLabel.Text },
                isMultiSelect: false);

            var result = await this.ShowPopupAsync(popup) as List<string>;
            if (result?.Any() == true)
            {
                PrimaryMuscleLabel.Text = result.First();
            }
        }

        async void OnSecondaryMusclesTapped(object sender, EventArgs e)
        {
            var isPlaceholder = string.IsNullOrWhiteSpace(SecondaryMuscleLabel.Text)
                                || SecondaryMuscleLabel.Text.StartsWith("��������");

            var selected = isPlaceholder
                ? Array.Empty<string>()
                : SecondaryMuscleLabel.Text.Split(", ", StringSplitOptions.RemoveEmptyEntries);

            var popup = new SelectionPopup(
                "�������������� ������",
                FilterPageViewModel.GetAvailableMuscleGroups(),
                selected,
                isMultiSelect: true);

            var result = await this.ShowPopupAsync(popup) as List<string>;

            // ��������� ����� ������ ���� ������������ ���� ����� "���������"
            if (result != null)
            {
                SecondaryMuscleLabel.Text = result.Any()
                    ? string.Join(", ", result)
                    : "�������� ������";
            }
        }

        async void OnAddExerciseClicked(object sender, EventArgs e)
        {
            bool hasError = false;

            if (string.IsNullOrWhiteSpace(ExerciseNameEntry.Text))
            {
                NameErrorLabel.IsVisible = true;
                hasError = true;
            }
            else
            {
                NameErrorLabel.IsVisible = false;
            }

            if (string.IsNullOrWhiteSpace(PrimaryMuscleLabel.Text) || PrimaryMuscleLabel.Text == "�������� ������")
            {
                PrimaryMuscleErrorLabel.IsVisible = true;
                hasError = true;
            }
            else
            {
                PrimaryMuscleErrorLabel.IsVisible = false;
            }

            if (string.IsNullOrWhiteSpace(EquipmentLabel.Text) || EquipmentLabel.Text == "�������� ������������")
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

            // ��������������� ��������� �����, ���� ������������ �� ������ �������������� ������
            var secondaryText = SecondaryMuscleLabel.Text;
            var isSecondaryValid = !string.IsNullOrWhiteSpace(secondaryText) && !secondaryText.StartsWith("��������");

            var newExercise = new Exercise
            {
                ExerciseName = ExerciseNameEntry.Text.Trim(),
                MuscleGroup = PrimaryMuscleLabel.Text,
                SecondaryMuscleGroup = isSecondaryValid ? secondaryText : "",
                Equipment = EquipmentLabel.Text
            };

            await ExerciseDatabaseService.AddExerciseAsync(newExercise);

            MessagingCenter.Send(this, "ExerciseAdded", newExercise);

            await Navigation.PopAsync();
        }

        async void OnEquipmentTapped(object sender, EventArgs e)
        {
            var popup = new SelectionPopup(
                "������������",
                FilterPageViewModel.GetAvailableEquipment(),
                string.IsNullOrWhiteSpace(EquipmentLabel.Text) || EquipmentLabel.Text == "��������..."
                    ? Array.Empty<string>()
                    : new[] { EquipmentLabel.Text },
                isMultiSelect: false);

            var result = await this.ShowPopupAsync(popup) as List<string>;
            if (result?.Any() == true)
            {
                EquipmentLabel.Text = result.First();
            }
        }

        async void OnBackTapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
