using CommunityToolkit.Maui.Views;
using Setta.PopupPages;
using Setta.Services;
using Setta.Models;
using System.Windows.Input;

/// <summary>
/// —траница отображени€ и управлени€ тренировками пользовател€.
/// ѕозвол€ет начать новую тренировку (при отсутствии активных), отображает список прошлых и активных тренировок,
/// поддерживает группировку по датам, переход к детал€м тренировки и обработку ошибок через popup.
/// </summary>

namespace Setta.Pages;

public partial class WorkoutPage : ContentPage
{
    public WorkoutPage()
    {
        InitializeComponent();
    }

    // ќбработка добавлени€ новой тренировки
    private async void OnAddWorkoutClicked(object sender, EventArgs e)
    {
        var allWorkouts = await WorkoutDatabaseService.GetWorkoutsAsync();

        // ѕроверка на наличие активной тренировки
        if (allWorkouts.Any(w => w.EndDateTime == null))
        {
            await this.ShowPopupAsync(new ErrorsPopup("Ќельз€ начать новую тренировку, пока есть активна€."));
            return;
        }

        var popup = new AddWorkoutPopup();
        await this.ShowPopupAsync(popup);
    }

    // «агружаем тренировки при по€влении страницы
    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadWorkouts();
    }

    // «агрузка и группировка тренировок по датам
    private async void LoadWorkouts()
    {
        var workouts = await WorkoutDatabaseService.GetWorkoutsAsync();

        // «авершаем все старые активные тренировки, кроме самой новой
        var activeWorkouts = workouts
            .Where(w => w.EndDateTime == null)
            .OrderByDescending(w => w.StartDateTime)
            .ToList();

        if (activeWorkouts.Count > 1)
        {
            foreach (var w in activeWorkouts.Skip(1))
            {
                w.EndDateTime = w.StartDateTime; // можно также использовать DateTime.Now
                await WorkoutDatabaseService.UpdateWorkoutAsync(w);
            }
        }

        // ѕреобразуем тренировки в view-модели дл€ отображени€
        var viewItems = workouts.Select(w => new WorkoutViewItem
        {
            Id = w.Id,
            StartDateTime = w.StartDateTime,
            EndDateTime = w.EndDateTime,
            TotalWeight = w.TotalWeight
        });

        // √руппируем по дате (пример: "13 апрел€")
        var grouped = viewItems
            .GroupBy(w => w.StartDateTime.Date)
            .OrderByDescending(g => g.Key)
            .Select(g => new WorkoutGroup(
                g.Key.ToString("d MMMM", new System.Globalization.CultureInfo("ru-RU")),
                g.OrderByDescending(w => w.StartDateTime)))
            .ToList();

        WorkoutView.ItemsSource = grouped;
        WorkoutView.IsVisible = grouped.Any();
        NoWorkoutLabel.IsVisible = !grouped.Any();
    }

    //  оманда дл€ перехода к детал€м тренировки
    public ICommand WorkoutTapCommand => new Command<WorkoutViewItem>(async (item) =>
    {
        if (item == null) return;

        var workout = await WorkoutDatabaseService.GetWorkoutByIdAsync(item.Id);
        if (workout != null)
            await Navigation.PushAsync(new AddWorkoutPage(workout));
    });
}
