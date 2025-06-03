using CommunityToolkit.Maui.Views;
using Setta.Models;
using Setta.PopupPages;
using Setta.Services;
using System.Collections.ObjectModel;

/// <summary>
/// Страница добавления и редактирования тренировки.
/// Позволяет добавлять до 7 упражнений, редактировать подходы, использовать шаблоны, сохранять и удалять тренировку.
/// Интегрируется с базой данных: загружает, добавляет и удаляет упражнения и подходы в SQLite.
/// Предусмотрены проверки корректности заполнения и всплывающие окна для ошибок и подтверждений.
/// </summary>

namespace Setta.Pages;

public partial class AddWorkoutPage : ContentPage
{
    // Коллекция выбранных упражнений для тренировки
    private ObservableCollection<ExerciseInTemplate> _selectedExercises = new();
    private Workout _workout;

    public AddWorkoutPage(Workout workout, WorkoutTemplate? template = null)
    {
        InitializeComponent();
        _workout = workout;

        SelectedDateLabel.Text = _workout.StartDateTime.ToString("dd MMMM yyyy");

        // Загружаем упражнения только из базы данных
        _ = LoadExistingExercisesAsync();
    }

    // Вернуться на предыдущую страницу
    private async void OnBackTapped(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    // Обновить отображение списка упражнений
    private void UpdateExerciseListUI()
    {
        SelectedExercisesView.ItemsSource = _selectedExercises;
        SelectedExercisesView.IsVisible = _selectedExercises.Any();
    }

    // Добавление упражнений
    private async void OnAddExercisesClicked(object sender, EventArgs e)
    {
        if (_selectedExercises.Count >= 7)
        {
            await this.ShowPopupAsync(new ErrorsPopup("Вы можете добавить не более 7 упражнений в 1 тренировку."));
            return;
        }

        // Сброс предыдущей подписки
        MessagingCenter.Unsubscribe<ChooseExercisePage, List<Exercise>>(this, "ExercisesSelected");

        // Подписка на получение выбранных упражнений
        MessagingCenter.Subscribe<ChooseExercisePage, List<Exercise>>(this, "ExercisesSelected", async (_, list) =>
        {
            MessagingCenter.Unsubscribe<ChooseExercisePage, List<Exercise>>(this, "ExercisesSelected");

            if (list == null || list.Count == 0) return;

            int availableSlots = 7 - _selectedExercises.Count;

            var newExercises = list
                .Where(ex => ex != null && ex.ExerciseName != null &&
                             !_selectedExercises.Any(e => e.Exercise.ExerciseName == ex.ExerciseName))
                .Take(availableSlots)
                .ToList();

            if (newExercises.Count < list.Count)
            {
                await this.ShowPopupAsync(new ErrorsPopup("Вы можете добавить не более 7 упражнений в 1 тренировку."));
            }

            foreach (var ex in newExercises)
            {
                var workoutExercise = new WorkoutExercise
                {
                    WorkoutId = _workout.Id,
                    Name = ex.ExerciseName,
                    MuscleGroup = ex.MuscleGroup
                };

                // Сохраняем упражнение в базе и получаем Id
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

        // Передаём уже выбранные упражнения
        var alreadySelected = _selectedExercises.Select(e => e.Exercise).ToList();
        await Navigation.PushAsync(new ChooseExercisePage(alreadySelected));
    }

    // Редактирование или удаление упражнения в тренировке
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

                // Удаляем из базы все подходы и упражнение
                if (selected.WorkoutExerciseId.HasValue)
                {
                    await WorkoutDatabaseService.DeleteWorkoutExerciseWithSetsAsync(selected.WorkoutExerciseId.Value);
                }
            }
            else if (result is ExerciseInTemplate updated)
            {
                var index = _selectedExercises.IndexOf(selected);
                _selectedExercises[index] = updated;

                // Сохраняем подходы в базе данных
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

                // Обновить общий вес тренировки
                int totalWeight = _selectedExercises.Sum(ex =>
                    ex.Sets.Where(s => int.TryParse(s.Reps, out _) && int.TryParse(s.Weight, out _))
                           .Sum(s => int.Parse(s.Reps) * int.Parse(s.Weight)));

                _workout.TotalWeight = totalWeight;
                await WorkoutDatabaseService.UpdateWorkoutAsync(_workout);
            }

            UpdateExerciseListUI();
        }
    }

    // Сохранение тренировки
    private async void OnSaveWorkoutClicked(object sender, EventArgs e)
    {
        if (!_selectedExercises.Any())
        {
            await this.ShowPopupAsync(new ErrorsPopup("Добавьте не менее 1 упражнения."));
            return;
        }

        if (_selectedExercises.Any(e => e.Sets.Count == 0 || e.Sets.Any(s => s.Reps == null || s.Weight == null)))
        {
            await this.ShowPopupAsync(new ErrorsPopup("Упражнения должны содержать не менее 1 заполненного подхода."));
            return;
        }

        // Подсчёт суммарного веса
        int totalWeight = _selectedExercises.Sum(ex =>
            ex.Sets.Where(s => int.TryParse(s.Reps, out _) && int.TryParse(s.Weight, out _))
                   .Sum(s => int.Parse(s.Reps) * int.Parse(s.Weight)));

        _workout.TotalWeight = totalWeight;

        // Завершаем тренировку, только если она активна
        if (_workout.EndDateTime == null)
        {
            _workout.EndDateTime = DateTime.Now;
        }

        await WorkoutDatabaseService.UpdateWorkoutAsync(_workout);
        await Navigation.PopAsync();
    }

    // Загрузка упражнений из базы данных для текущей тренировки
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

    // Использовать шаблон для создания тренировки
    private async void OnUseTemplateClicked(object sender, EventArgs e)
    {
        if (_selectedExercises.Any())
        {
            await this.ShowPopupAsync(new ErrorsPopup("Вы уже добавили упражнения. Шаблон можно использовать только в пустой тренировке."));
            return;
        }

        var templates = await TemplateDatabaseService.GetTemplatesAsync();
        if (templates == null || templates.Count == 0)
        {
            await this.ShowPopupAsync(new ErrorsPopup("У вас еще нет шаблонов."));
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

    // Удаление тренировки
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
