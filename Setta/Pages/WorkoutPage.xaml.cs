using CommunityToolkit.Maui.Views;
using Setta.Models;
using Setta.PopupPages;
using Setta.Services;
using System;
using System.IO;

namespace Setta.Pages;

public partial class WorkoutPage : ContentPage
{
    private readonly WorkoutDatabaseService _db;
    public WorkoutPageViewModel ViewModel { get; set; }

    public WorkoutPage()
    {
        InitializeComponent();
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "workout.db");
        _db = new WorkoutDatabaseService(dbPath);

        ViewModel = new WorkoutPageViewModel();
        BindingContext = ViewModel;
    }

    private async void OnAddWorkoutClicked(object sender, EventArgs e)
    {
        var popup = new AddWorkoutPopup(DateTime.Today);
        var result = await this.ShowPopupAsync(popup) as Tuple<DateTime, bool>;
        if (result == null)
            return;

        var date = result.Item1;

        // 1. ���������: ������� ����
        if (date.Date > DateTime.Today)
        {
            await this.ShowPopupAsync(new ErrorsTemplatesPopup("������ �������� ���������� � ����, ������� �� ��������."));
            return;
        }

        // 2. ������ ����� ����������
        var workout = new Workout
        {
            Date = date.Date,
            StartTime = date.Date == DateTime.Today ? DateTime.Now : date.Date, // ���� ������� � ������� �����
            Status = date.Date == DateTime.Today ? WorkoutStatus.Active : WorkoutStatus.Completed,
            // ��������� ���� � ������, ����� ����������� �� ���� (EndTime, TotalWeight, TotalDuration)
        };

        await _db.SaveWorkoutAsync(workout);

        // 3. �������� ViewModel (����� ���������� ��������� �� ������� ������)
        ViewModel?.GroupedWorkouts.Clear();
        ViewModel?.GetType().GetMethod("LoadWorkouts", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.Invoke(ViewModel, null);

        // 4. ��������� WorkoutInfoPage (����� �� id ����������)
        await Navigation.PushAsync(new WorkoutInfoPage(workout.Id));
    }
}
