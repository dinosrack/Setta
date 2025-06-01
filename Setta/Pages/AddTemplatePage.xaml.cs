using CommunityToolkit.Maui.Views;
using Setta.Models;
using Setta.PopupPages;
using Setta.Services;
using System.Collections.ObjectModel;

namespace Setta.Pages;

public partial class AddTemplatePage : ContentPage
{
    private ObservableCollection<ExerciseInTemplate> _selectedExercises = new();

    public AddTemplatePage()
    {
        InitializeComponent();
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
        // Ограничение на количество упражнений
        if (_selectedExercises.Count >= 7)
        {
            await this.ShowPopupAsync(new ErrorsPopup("Вы можете добавить не более 7 упражнений в 1 шаблон."));
            return;
        }

        // Удалим предыдущую подписку на всякий случай
        MessagingCenter.Unsubscribe<AddExerciseToTemplatePage, List<Exercise>>(this, "ExercisesSelected");

        // Подписка на выбранные упражнения
        MessagingCenter.Subscribe<AddExerciseToTemplatePage, List<Exercise>>(this,
            "ExercisesSelected", (_, list) =>
            {
                MessagingCenter.Unsubscribe<AddExerciseToTemplatePage, List<Exercise>>(this, "ExercisesSelected");

                if (list == null || list.Count == 0)
                    return;

                foreach (var ex in list)
                {
                    if (ex == null || ex.ExerciseName == null)
                        continue;

                    if (_selectedExercises.Any(e => e.Exercise.ExerciseName == ex.ExerciseName))
                        continue;

                    if (_selectedExercises.Count >= 7)
                    {
                        this.ShowPopup(new ErrorsPopup("Вы можете добавить не более 7 упражнений в 1 шаблон."));
                        break;
                    }

                    _selectedExercises.Add(new ExerciseInTemplate
                    {
                        Exercise = ex,
                        Sets = new ObservableCollection<ExerciseSet>()
                    });
                }

                UpdateExerciseListUI();
                ExercisesErrorLabel.IsVisible = false;
            });

        // Передаём уже выбранные упражнения
        var alreadySelected = _selectedExercises.Select(e => e.Exercise).ToList();
        await Navigation.PushAsync(new AddExerciseToTemplatePage(alreadySelected));
    }

    private async void OnExerciseTapped(object sender, EventArgs e)
    {
        if ((sender as VisualElement)?.BindingContext is ExerciseInTemplate selected)
        {
            var popup = new ProgressInExercisesPopup(selected);
            var result = await this.ShowPopupAsync(popup);

            if (popup.IsSaved == false)
            {
                // Пользователь вышел без нажатия "Сохранить" или "Удалить" — ничего не делаем
                return;
            }

            if (result is null)
            {
                _selectedExercises.Remove(selected); // Явное удаление
            }
            else if (result is ExerciseInTemplate updated)
            {
                var index = _selectedExercises.IndexOf(selected);
                _selectedExercises[index] = updated;
            }

            UpdateExerciseListUI();
        }
    }

    private async void OnCreateTemplateClicked(object sender, EventArgs e)
    {
        var name = TemplateNameEntry.Text?.Trim();
        bool hasError = false;

        if (string.IsNullOrWhiteSpace(name))
        {
            NameErrorLabel.IsVisible = true;
            hasError = true;
        }
        else
        {
            NameErrorLabel.IsVisible = false;
        }

        if (!_selectedExercises.Any())
        {
            ExercisesErrorLabel.IsVisible = true;
            hasError = true;
        }
        else
        {
            ExercisesErrorLabel.IsVisible = false;
        }

        if (hasError) return;

        foreach (var ex in _selectedExercises)
        {
            if (ex.Sets == null || ex.Sets.Count == 0)
            {
                await this.ShowPopupAsync(new ErrorsPopup("Упражнения должны содержать не менее 1 заполненного подхода."));
                return;
            }

            if (ex.Sets.Any(set => set.Weight == null || set.Reps == null))
            {
                await this.ShowPopupAsync(new ErrorsPopup("Упражнения должны содержать заполненные подходы."));
                return;
            }
        }

        var list = _selectedExercises.Select(e => new TemplateExercise
        {
            Name = e.Exercise.ExerciseName,
            MuscleGroup = e.Exercise.MuscleGroup,
            Sets = e.Sets.ToList()
        }).ToList();

        var json = System.Text.Json.JsonSerializer.Serialize(list);

        var template = new WorkoutTemplate
        {
            Name = name,
            ExercisesJson = json
        };

        await TemplateDatabaseService.AddTemplateAsync(template);
        MessagingCenter.Send(this, "TemplateCreated", template);
        await Navigation.PopAsync();
    }
}
