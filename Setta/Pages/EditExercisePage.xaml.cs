using Setta.Models;
using Setta.PopupPages;
using Setta.Services;
using Setta.ViewModels;
using CommunityToolkit.Maui.Views;

namespace Setta.Pages;

public partial class EditExercisePage : ContentPage
{
    private Exercise _exercise;

    public EditExercisePage(Exercise exercise)
    {
        InitializeComponent();
        _exercise = exercise;

        // ��������� ����
        ExerciseNameEntry.Text = _exercise.ExerciseName;
        PrimaryMuscleLabel.Text = _exercise.MuscleGroup;
        SecondaryMuscleLabel.Text = string.IsNullOrWhiteSpace(_exercise.SecondaryMuscleGroup) ? "�������� ������" : _exercise.SecondaryMuscleGroup;
        EquipmentLabel.Text = _exercise.Equipment;
    }

    async void OnPrimaryMuscleTapped(object sender, EventArgs e)
    {
        var popup = new SelectionPopup(
            "�������� ������",
            FilterPageViewModel.GetAvailableMuscleGroups(),
            new[] { PrimaryMuscleLabel.Text },
            isMultiSelect: false);

        var result = await this.ShowPopupAsync(popup) as List<string>;
        if (result?.Any() == true)
            PrimaryMuscleLabel.Text = result.First();
    }

    async void OnSecondaryMusclesTapped(object sender, EventArgs e)
    {
        var current = string.IsNullOrWhiteSpace(SecondaryMuscleLabel.Text) || SecondaryMuscleLabel.Text.StartsWith("��������")
            ? Array.Empty<string>()
            : SecondaryMuscleLabel.Text.Split(", ", StringSplitOptions.RemoveEmptyEntries);

        var popup = new SelectionPopup(
            "�������������� ������",
            FilterPageViewModel.GetAvailableMuscleGroups(),
            current,
            isMultiSelect: true);

        var result = await this.ShowPopupAsync(popup) as List<string>;
        if (result != null)
        {
            SecondaryMuscleLabel.Text = result.Any()
                ? string.Join(", ", result)
                : "�������� ������";
        }
    }

    async void OnEquipmentTapped(object sender, EventArgs e)
    {
        var popup = new SelectionPopup(
            "������������",
            FilterPageViewModel.GetAvailableEquipment(),
            new[] { EquipmentLabel.Text },
            isMultiSelect: false);

        var result = await this.ShowPopupAsync(popup) as List<string>;
        if (result?.Any() == true)
            EquipmentLabel.Text = result.First();
    }

    async void OnSaveClicked(object sender, EventArgs e)
    {
        _exercise.ExerciseName = ExerciseNameEntry.Text?.Trim();
        _exercise.MuscleGroup = PrimaryMuscleLabel.Text;
        _exercise.Equipment = EquipmentLabel.Text;
        _exercise.SecondaryMuscleGroup = SecondaryMuscleLabel.Text.StartsWith("��������") ? "" : SecondaryMuscleLabel.Text;

        // ��������� ��������� � ���� ������
        await ExerciseDatabaseService.UpdateExerciseAsync(_exercise);

        // ���������� ��������� � ���, ��� ���������� ���� ���������
        MessagingCenter.Send(this, "ExerciseUpdated", _exercise);

        // ������������ �� �������� ExerciseInfoPage
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

            // ������� ��� ��������
            await Shell.Current.Navigation.PopToRootAsync();
        }
    }

    async void OnBackTapped(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
