using CommunityToolkit.Maui.Views;
using Setta.Models;
using Setta.PopupPages;
using Setta.Services;
using System.Collections.ObjectModel;

namespace Setta.Pages;

public partial class AddWorkoutPage : ContentPage
{
    private ObservableCollection<ExerciseInTemplate> _selectedExercises = new();
    private Workout _workout;

    public AddWorkoutPage(Workout workout)
    {
        InitializeComponent();
        _workout = workout;

        SelectedDateLabel.Text = _workout.StartDateTime.ToString("dd MMMM yyyy");
    }

    private async void OnBackTapped(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private void UpdateExerciseListUI()
    {
        SelectedExercisesView.ItemsSource = _selectedExercises;
        SelectedExercisesView.IsVisible = _selectedExercises.Any();
    }

    private async void OnAddExercisesClicked(object sender, EventArgs e)
    {
        if (_selectedExercises.Count >= 20)
        {
            await this.ShowPopupAsync(new ErrorsTemplatesPopup("Можно добавить не более 20 упражнений в тренировку."));
            return;
        }

        MessagingCenter.Unsubscribe<AddExerciseToTemplatePage, List<Exercise>>(this, "ExercisesSelected");

        MessagingCenter.Subscribe<AddExerciseToTemplatePage, List<Exercise>>(this, "ExercisesSelected", (_, list) =>
        {
            MessagingCenter.Unsubscribe<AddExerciseToTemplatePage, List<Exercise>>(this, "ExercisesSelected");

            if (list == null || list.Count == 0) return;

            foreach (var ex in list)
            {
                if (ex == null || ex.ExerciseName == null) continue;
                if (_selectedExercises.Any(e => e.Exercise.ExerciseName == ex.ExerciseName)) continue;

                _selectedExercises.Add(new ExerciseInTemplate
                {
                    Exercise = ex,
                    Sets = new ObservableCollection<ExerciseSet>()
                });
            }

            UpdateExerciseListUI();
        });

        var alreadySelected = _selectedExercises.Select(e => e.Exercise).ToList();
        await Navigation.PushAsync(new AddExerciseToTemplatePage(alreadySelected));
    }

    private async void OnExerciseTapped(object sender, EventArgs e)
    {
        if ((sender as VisualElement)?.BindingContext is ExerciseInTemplate selected)
        {
            var popup = new ProgressInExercisesPopup(selected);
            var result = await this.ShowPopupAsync(popup);

            if (popup.IsSaved == false) return;

            if (result is null)
            {
                _selectedExercises.Remove(selected);
            }
            else if (result is ExerciseInTemplate updated)
            {
                var index = _selectedExercises.IndexOf(selected);
                _selectedExercises[index] = updated;
            }

            UpdateExerciseListUI();
        }
    }

    private async void OnSaveWorkoutClicked(object sender, EventArgs e)
    {
        if (!_selectedExercises.Any())
        {
            await this.ShowPopupAsync(new ErrorsTemplatesPopup("Добавьте хотя бы одно упражнение."));
            return;
        }

        if (_selectedExercises.Any(e => e.Sets.Count == 0 || e.Sets.Any(s => s.Reps == null || s.Weight == null)))
        {
            await this.ShowPopupAsync(new ErrorsTemplatesPopup("Каждое упражнение должно содержать хотя бы один заполненный подход."));
            return;
        }

        // Пример подсчета суммарного веса
        int totalWeight = _selectedExercises.Sum(ex =>
            ex.Sets.Where(s => int.TryParse(s.Reps, out _) && int.TryParse(s.Weight, out _))
                   .Sum(s => int.Parse(s.Reps) * int.Parse(s.Weight)));

        _workout.TotalWeight = totalWeight;
        _workout.EndDateTime = DateTime.Now;

        await WorkoutDatabaseService.UpdateWorkoutAsync(_workout);

        await this.ShowPopupAsync(new ErrorsTemplatesPopup("Тренировка успешно сохранена!"));
        await Navigation.PopAsync();
    }
}
