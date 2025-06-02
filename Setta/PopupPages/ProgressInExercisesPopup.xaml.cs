using CommunityToolkit.Maui.Views;
using Setta.Models;
using System.Collections.ObjectModel;

/// <summary>
/// Popup-окно для редактирования подходов в упражнении внутри тренировки или шаблона.
/// Позволяет добавлять и удалять подходы (максимум 10), редактировать вес и повторения.
/// Поддерживает валидацию, не даёт удалить последний подход и возвращает результат при сохранении или удалении.
/// </summary>

namespace Setta.PopupPages;
public partial class ProgressInExercisesPopup : Popup
{
    public ExerciseInTemplate Exercise { get; private set; }

    public ProgressInExercisesPopup(ExerciseInTemplate exercise)
    {
        InitializeComponent();

        Exercise = exercise;

        // Если подходов нет — создаём один пустой
        if (Exercise.Sets.Count == 0)
            Exercise.Sets.Add(new ExerciseSet
            {
                Weight = null,
                Reps = null
            });

        BindingContext = Exercise;
    }

    // Добавить подход (максимум 10)
    private void OnAddSet(object sender, EventArgs e)
    {
        if (Exercise.Sets.Count >= 10)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    var popup = new ErrorsPopup("Вы можете добавить не более 10 подходов.");
                    App.Current.MainPage.ShowPopup(popup);
                });
            });
            return;
        }

        Exercise.Sets.Add(new ExerciseSet());
    }

    // Удалить подход (нельзя удалить единственный)
    private void OnDeleteSet(object sender, EventArgs e)
    {
        if ((sender as VisualElement)?.BindingContext is ExerciseSet set)
        {
            // Нельзя удалить последний подход
            if (Exercise.Sets.Count == 1)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    var popup = new ErrorsPopup("Вы не можете удалить единственный подход.");
                    App.Current.MainPage.ShowPopup(popup);
                });
                return;
            }

            Exercise.Sets.Remove(set);

            // Обновим ItemsSource вручную (фикс визуального бага при удалении первого)
            SetsView.ItemsSource = null;
            SetsView.ItemsSource = Exercise.Sets;
        }
    }

    public bool IsSaved { get; private set; } = false;

    // Сохранить изменения в подходах
    private void OnSaveExercise(object sender, EventArgs e)
    {
        IsSaved = true;
        Close(Exercise); // Возврат объекта упражнения
    }

    // Удалить всё упражнение (возврат null)
    private void OnDeleteExercise(object sender, EventArgs e)
    {
        IsSaved = true;
        Close(null);
    }
}
