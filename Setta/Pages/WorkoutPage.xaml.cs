using CommunityToolkit.Maui.Views;
using Setta.PopupPages;
using Setta.Models;
using Setta.ViewModels;
using Setta.Services;

namespace Setta.Pages;

public partial class WorkoutPage : ContentPage
{
    private WorkoutDatabaseService _db;
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
        if (result == null) return;

        var date = result.Item1;
        var useTemplate = result.Item2;

        if (date > DateTime.Today)
        {
            await this.ShowPopupAsync(new ErrorsTemplatesPopup("������ �������� ���������� � ����, ������� �� ��������."));
            return;
        }

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "workout.db");
        var db = new WorkoutDatabaseService(dbPath);

        if (!useTemplate)
        {
            // ������ ����������
            var workout = new Workout
            {
                Date = date,
                IsActive = (date == DateTime.Today),
                StartTime = DateTime.Now,
                ExercisesJson = "" // ����� ��� ��������
            };
            await db.SaveWorkoutAsync(workout);

            // ����� ���������� ����������� ������ ������
            await RefreshWorkouts();

            // ��������� �������� � Id ����� ����������
            await Navigation.PushAsync(new WorkoutInfoPage(workout.Id));
        }
        else
        {
            // ����� ������� (����������� ��������)
            await Navigation.PushAsync(new ChooseTemplatePage(date));
        }
    }

    // ���������� ������ ���������� ������ ��� ��� ��������� ��������
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ViewModel.LoadWorkoutsAsync();
    }

    private async Task RefreshWorkouts()
    {
        var workouts = await _db.GetWorkoutsAsync();
        ViewModel.Workouts.Clear();
        foreach (var w in workouts)
            ViewModel.Workouts.Add(w);

        OnPropertyChanged(nameof(ViewModel.IsWorkoutListEmpty));
        OnPropertyChanged(nameof(ViewModel.IsWorkoutListNotEmpty));
    }
}
