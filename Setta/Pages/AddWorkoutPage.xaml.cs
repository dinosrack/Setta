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

    public AddWorkoutPage(Workout workout, WorkoutTemplate? template = null)
    {
        InitializeComponent();
        _workout = workout;

        SelectedDateLabel.Text = _workout.StartDateTime.ToString("dd MMMM yyyy");

        // Загружаем упражнения ТОЛЬКО из базы
        _ = LoadExistingExercisesAsync();
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
        if (_selectedExercises.Count >= 7)
        {
            await this.ShowPopupAsync(new ErrorsTemplatesPopup("Вы можете добавить не более 7 упражнений в 1 тренировку."));
            return;
        }

        MessagingCenter.Unsubscribe<AddExerciseToTemplatePage, List<Exercise>>(this, "ExercisesSelected");

        MessagingCenter.Subscribe<AddExerciseToTemplatePage, List<Exercise>>(this, "ExercisesSelected", async (_, list) =>
        {
            MessagingCenter.Unsubscribe<AddExerciseToTemplatePage, List<Exercise>>(this, "ExercisesSelected");

            if (list == null || list.Count == 0) return;

            int availableSlots = 7 - _selectedExercises.Count;

            var newExercises = list
                .Where(ex => ex != null && ex.ExerciseName != null &&
                             !_selectedExercises.Any(e => e.Exercise.ExerciseName == ex.ExerciseName))
                .Take(availableSlots)
                .ToList();

            if (newExercises.Count < list.Count)
            {
                await this.ShowPopupAsync(new ErrorsTemplatesPopup("Вы можете добавить не более 7 упражнений в 1 тренировку."));
            }

            foreach (var ex in newExercises)
            {
                var workoutExercise = new WorkoutExercise
                {
                    WorkoutId = _workout.Id,
                    Name = ex.ExerciseName,
                    MuscleGroup = ex.MuscleGroup
                };

                int id = await WorkoutDatabaseService.AddWorkoutExerciseAsync(workoutExercise);

                var newItem = new ExerciseInTemplate
                {
                    Exercise = ex,
                    WorkoutExerciseId = id,
                    Sets = new ObservableCollection<ExerciseSet>()
                };

                _selectedExercises.Add(newItem);
            }

            await WorkoutDatabaseService.UpdateWorkoutAsync(_workout);
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

                // Удаляем из SQLite
                if (selected.WorkoutExerciseId.HasValue)
                {
                    await WorkoutDatabaseService.DeleteWorkoutExerciseWithSetsAsync(selected.WorkoutExerciseId.Value);
                }
            }
            else if (result is ExerciseInTemplate updated)
            {
                var index = _selectedExercises.IndexOf(selected);
                _selectedExercises[index] = updated;

                // Сохраняем подходы в SQLite
                if (updated.WorkoutExerciseId.HasValue)
                {
                    foreach (var set in updated.Sets)
                    {
                        if (int.TryParse(set.Reps, out int reps) && int.TryParse(set.Weight, out int weight))
                        {
                            await WorkoutDatabaseService.AddWorkoutSetAsync(new WorkoutSet
                            {
                                ExerciseId = updated.WorkoutExerciseId.Value,
                                Reps = reps,
                                Weight = weight
                            });
                        }
                    }
                }

                // Обновим общий вес тренировки
                int totalWeight = _selectedExercises.Sum(ex =>
                    ex.Sets.Where(s => int.TryParse(s.Reps, out _) && int.TryParse(s.Weight, out _))
                           .Sum(s => int.Parse(s.Reps) * int.Parse(s.Weight)));

                _workout.TotalWeight = totalWeight;
                await WorkoutDatabaseService.UpdateWorkoutAsync(_workout);
            }

            UpdateExerciseListUI();
        }
    }

    private async void OnSaveWorkoutClicked(object sender, EventArgs e)
    {
        if (!_selectedExercises.Any())
        {
            await this.ShowPopupAsync(new ErrorsTemplatesPopup("Добавьте не менее 1 упражнения."));
            return;
        }

        if (_selectedExercises.Any(e => e.Sets.Count == 0 || e.Sets.Any(s => s.Reps == null || s.Weight == null)))
        {
            await this.ShowPopupAsync(new ErrorsTemplatesPopup("Упражнения должны содержать не менее 1 заполненного подхода."));
            return;
        }

        // Подсчёт суммарного веса
        int totalWeight = _selectedExercises.Sum(ex =>
            ex.Sets.Where(s => int.TryParse(s.Reps, out _) && int.TryParse(s.Weight, out _))
                   .Sum(s => int.Parse(s.Reps) * int.Parse(s.Weight)));

        _workout.TotalWeight = totalWeight;
        _workout.EndDateTime = DateTime.Now;

        await WorkoutDatabaseService.UpdateWorkoutAsync(_workout);
        await Navigation.PopAsync();
    }

    private async Task LoadExistingExercisesAsync()
    {
        _selectedExercises.Clear();

        var exercises = await WorkoutDatabaseService.GetWorkoutExercisesAsync(_workout.Id);

        foreach (var ex in exercises)
        {
            var sets = await WorkoutDatabaseService.GetWorkoutSetsAsync(ex.Id);

            var exerciseInTemplate = new ExerciseInTemplate
            {
                Exercise = new Exercise
                {
                    ExerciseName = ex.Name,
                    MuscleGroup = ex.MuscleGroup
                },
                WorkoutExerciseId = ex.Id,
                Sets = new ObservableCollection<ExerciseSet>(
                    sets.Select(s => new ExerciseSet
                    {
                        Reps = s.Reps.ToString(),
                        Weight = s.Weight.ToString()
                    }))
            };

            _selectedExercises.Add(exerciseInTemplate);
        }

        UpdateExerciseListUI();
    }


    private async void OnUseTemplateClicked(object sender, EventArgs e)
    {
        if (_selectedExercises.Any())
        {
            await this.ShowPopupAsync(new ErrorsTemplatesPopup("Вы уже добавили упражнения. Шаблон можно использовать только в пустой тренировке."));
            return;
        }

        var templates = await TemplateDatabaseService.GetTemplatesAsync();
        if (templates == null || templates.Count == 0)
        {
            await this.ShowPopupAsync(new ErrorsTemplatesPopup("У вас еще нет шаблонов."));
            return;
        }

        var page = new ChooseTemplatePage();
        page.TemplateChosen += async (s, selectedTemplate) =>
        {
            if (selectedTemplate == null) return;

            await WorkoutDatabaseService.ApplyTemplateToWorkoutAsync(_workout.Id, selectedTemplate, _selectedExercises.Count);
            _selectedExercises.Clear();
            await LoadExistingExercisesAsync();
        };

        await Navigation.PushAsync(page);
    }

    private async void OnDeleteWorkoutClicked(object sender, EventArgs e)
    {
        var popup = new DeleteItemPopup();
        var result = await this.ShowPopupAsync(popup);

        if (result is bool confirmed && confirmed)
        {
            await WorkoutDatabaseService.DeleteWorkoutAsync(_workout.Id);
            await Navigation.PopAsync();
            _selectedExercises.Clear();
        }
    }
}
