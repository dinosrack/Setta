using CommunityToolkit.Maui.Views;
using Setta.ViewModels;
using Setta.Pages;
using Setta.Services;
using Setta.Models;

/// <summary>
/// Popup-окно для создания новой тренировки.
/// Позволяет выбрать дату, проверяет ограничения (не более 2 тренировок на день, нельзя в будущем),
/// создаёт новую тренировку и переходит к её редактированию.
/// Использует popup для вывода ошибок.
/// </summary>

namespace Setta.PopupPages;
public partial class AddWorkoutPopup : Popup
{
    public AddWorkoutPopup()
    {
        InitializeComponent();
    }

    // Обработка нажатия на "Продолжить"
    private async void OnContinueClicked(object sender, EventArgs e)
    {
        var selectedDate = WorkoutDatePicker.Date.Date;
        var now = DateTime.Now;

        // Нельзя выбрать дату в будущем
        if (selectedDate > now.Date)
        {
            await Shell.Current.CurrentPage.ShowPopupAsync(new ErrorsPopup(
                "Нельзя создать тренировку на будущее. Выберите сегодняшнюю или прошедшую дату."));
            return;
        }

        var allWorkouts = await WorkoutDatabaseService.GetWorkoutsAsync();

        // Ограничение: не более 2 тренировок на выбранный день
        int countForDate = allWorkouts.Count(w => w.StartDateTime.Date == selectedDate);
        if (countForDate >= 2)
        {
            await Shell.Current.CurrentPage.ShowPopupAsync(new ErrorsPopup(
                "Вы можете записать не более 2 тренировок за 1 день."));
            return;
        }

        // Создание новой тренировки
        var workoutDate = selectedDate.Date;

        DateTime startDateTime;
        DateTime? endDateTime = null;

        if (workoutDate == now.Date)
        {
            // Тренировка на сегодня — точное текущее время
            startDateTime = now;
        }
        else
        {
            // Тренировка на другой день — 12:00 и сразу завершается через 1 час
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
