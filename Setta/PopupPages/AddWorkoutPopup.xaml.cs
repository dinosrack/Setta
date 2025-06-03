using CommunityToolkit.Maui.Views;
using Setta.ViewModels;
using Setta.Pages;
using Setta.Services;
using Setta.Models;

/// <summary>
/// Popup-���� ��� �������� ����� ����������.
/// ��������� ������� ����, ��������� ����������� (�� ����� 2 ���������� �� ����, ������ � �������),
/// ������ ����� ���������� � ��������� � � ��������������.
/// ���������� popup ��� ������ ������.
/// </summary>

namespace Setta.PopupPages;
public partial class AddWorkoutPopup : Popup
{
    public AddWorkoutPopup()
    {
        InitializeComponent();
    }

    // ��������� ������� �� "����������"
    private async void OnContinueClicked(object sender, EventArgs e)
    {
        var selectedDate = WorkoutDatePicker.Date.Date;
        var now = DateTime.Now;

        // ������ ������� ���� � �������
        if (selectedDate > now.Date)
        {
            await Shell.Current.CurrentPage.ShowPopupAsync(new ErrorsPopup(
                "������ ������� ���������� �� �������. �������� ����������� ��� ��������� ����."));
            return;
        }

        var allWorkouts = await WorkoutDatabaseService.GetWorkoutsAsync();

        // �����������: �� ����� 2 ���������� �� ��������� ����
        int countForDate = allWorkouts.Count(w => w.StartDateTime.Date == selectedDate);
        if (countForDate >= 2)
        {
            await Shell.Current.CurrentPage.ShowPopupAsync(new ErrorsPopup(
                "�� ������ �������� �� ����� 2 ���������� �� 1 ����."));
            return;
        }

        // �������� ����� ����������
        var workoutDate = selectedDate.Date;

        DateTime startDateTime;
        DateTime? endDateTime = null;

        if (workoutDate == now.Date)
        {
            // ���������� �� ������� � ������ ������� �����
            startDateTime = now;
        }
        else
        {
            // ���������� �� ������ ���� � 12:00 � ����� ����������� ����� 1 ���
            startDateTime = workoutDate.AddHours(12);
            endDateTime = startDateTime.AddHours(1);
        }

        var workout = new Workout
        {
            StartDateTime = startDateTime,
            EndDateTime = endDateTime,
            TotalWeight = 0
        };

        int id = await WorkoutDatabaseService.AddWorkoutAsync(workout);
        workout.Id = id;

        await this.CloseAsync();
        await Shell.Current.Navigation.PushAsync(new AddWorkoutPage(workout));
    }
}
