using CommunityToolkit.Maui.Views;
using Setta.ViewModels;
using Setta.Pages;
using Setta.Services;
using Setta.Models;

namespace Setta.PopupPages;
public partial class AddWorkoutPopup : Popup
{
    public AddWorkoutPopup()
    {
        InitializeComponent();
    }

    private async void OnContinueClicked(object sender, EventArgs e)
    {
        var selectedDate = WorkoutDatePicker.Date.Date;
        var now = DateTime.Today;

        // ������ ������� ���� � �������
        if (selectedDate > now)
        {
            await Shell.Current.CurrentPage.ShowPopupAsync(new ErrorsTemplatesPopup(
                "������ ������� ���������� �� �������. �������� ����������� ��� ��������� ����."));
            return;
        }

        var allWorkouts = await WorkoutDatabaseService.GetWorkoutsAsync();

        // �����������: �� ����� 2 ���������� �� ��������� ����
        int countForDate = allWorkouts.Count(w => w.StartDateTime.Date == selectedDate);
        if (countForDate >= 2)
        {
            await Shell.Current.CurrentPage.ShowPopupAsync(new ErrorsTemplatesPopup(
                "�� ������ �������� �� ����� 2 ���������� �� 1 ����."));
            return;
        }

        var workout = new Workout
        {
            StartDateTime = selectedDate.Add(DateTime.Now.TimeOfDay),
            EndDateTime = null,
            TotalWeight = 0
        };

        int id = await WorkoutDatabaseService.AddWorkoutAsync(workout);
        workout.Id = id;

        await this.CloseAsync();
        await Shell.Current.Navigation.PushAsync(new AddWorkoutPage(workout));
    }
}
