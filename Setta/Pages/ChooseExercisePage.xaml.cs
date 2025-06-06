using CommunityToolkit.Maui.Views;
using Setta.Models;
using Setta.PopupPages;
using Setta.ViewModels;

/// <summary>
/// �������� ���������� ���������� � ������ ����������.
/// ��������� ������� ���������� �� ������ ������, ��������� ������� �� ������ ���� � ������������,
/// � ��������� ��������� �������� ������� � ���������� ���������.
/// ���������� ����������� ���� ��� ���������� � ����������� ������.
/// </summary>

namespace Setta.Pages;

public partial class ChooseExercisePage : ContentPage
{
    private ChooseExerciseViewModel ViewModel => BindingContext as ChooseExerciseViewModel;

    public ChooseExercisePage(List<Exercise> alreadySelected)
    {
        InitializeComponent();

        // ��������� ���������� ����� ����������� (������� ��� ��������� ����������)
        ViewModel?.SetExcludedExercises(alreadySelected);

        // �������� �� ���������� �������� ����� popup
        MessagingCenter.Subscribe<FilterPageViewModel, Tuple<List<string>, List<string>>>(this,
            "FiltersApplied", (_, tuple) =>
            {
                ViewModel?.ApplyFiltersFromPopup(tuple.Item1, tuple.Item2);
            });
    }

    // ������� ����� �� ����
    private async void OnBackTapped(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    // ���������� ��������� ���������� � ������
    private async void OnAddClicked(object sender, EventArgs e)
    {
        var selected = ViewModel?.GetSelectedExercises();

        if (selected == null || selected.Count == 0)
        {
            await this.ShowPopupAsync(new ErrorsPopup("�������� �� ����� 1 ����������."));
            return;
        }

        // ������� ��������� ���������� � ��������� ��������
        MessagingCenter.Send(this, "ExercisesSelected", selected);
        await Navigation.PopAsync();
    }

    // �������� ���� ����������
    private void OnFilterClicked(object sender, EventArgs e)
    {
        if (ViewModel == null) return;

        var popup = new FilterPopup(ViewModel.SelectedMuscleGroups, ViewModel.SelectedEquipment);
        this.ShowPopup(popup);
    }
}
