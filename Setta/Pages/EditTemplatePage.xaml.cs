using CommunityToolkit.Maui.Views;
using Setta.Models;
using Setta.PopupPages;
using Setta.Services;
using System.Collections.ObjectModel;

/// <summary>
/// Страница редактирования шаблона тренировки.
/// Позволяет изменять название шаблона, добавлять и удалять упражнения (до 7), а также редактировать подходы.
/// Предусмотрена валидация данных, сохранение и удаление шаблона.
/// Использует всплывающие окна для ошибок и подтверждений, интеграцию с базой данных и работу с сериализацией упражнений.
/// </summary>

namespace Setta.Pages;

public partial class EditTemplatePage : ContentPage
{
    private WorkoutTemplate _template;
    private ObservableCollection<ExerciseInTemplate> _selectedExercises = new();

    public EditTemplatePage(WorkoutTemplate template)
    {
        InitializeComponent();
        _template = template;

        LoadData();
    }

    // Загрузка данных шаблона и упражнений из JSON
    private void LoadData()
    {
        TemplateNameEntry.Text = _template.Name;

        var parsed = System.Text.Json.JsonSerializer.Deserialize<List<TemplateExercise>>(_template.ExercisesJson);

        _selectedExercises.Clear();
        foreach (var item in parsed)
        {
            _selectedExercises.Add(new ExerciseInTemplate
            {
                Exercise = new Exercise
                {
                    ExerciseName = item.Name,
                    MuscleGroup = item.MuscleGroup
                },
                Sets = new ObservableCollection<ExerciseSet>(item.Sets)
            });
        }

        SelectedExercisesView.ItemsSource = _selectedExercises;
        SelectedExercisesView.IsVisible = _selectedExercises.Any();
    }

    // Редактирование или удаление упражнения
    private async void OnExerciseTapped(object sender, EventArgs e)
    {
        if ((sender as VisualElement)?.BindingContext is ExerciseInTemplate selected)
        {
            var popup = new ProgressInExercisesPopup(selected);
            var result = await this.ShowPopupAsync(popup);

            if (popup.IsSaved == false)
            {
                // Пользователь не сохранил изменения — ничего не делаем
                return;
            }

            if (result is null)
            {
                // Удаление упражнения из шаблона
                _selectedExercises.Remove(selected);
            }
            else if (result is ExerciseInTemplate updated)
            {
                var index = _selectedExercises.IndexOf(selected);
                _selectedExercises[index] = updated;
            }

            SelectedExercisesView.IsVisible = _selectedExercises.Any();
        }
    }

    // Добавление новых упражнений в шаблон
    private async void OnAddExercisesClicked(object sender, EventArgs e)
    {
        // Проверка лимита до открытия страницы выбора
        if (_selectedExercises.Count >= 7)
        {
            await this.ShowPopupAsync(new ErrorsPopup("Вы можете добавить не более 7 упражнений в 1 шаблон."));
            return;
        }

        // Отменяем старую подписку, если она была
        MessagingCenter.Unsubscribe<ChooseExercisePage, List<Exercise>>(this, "ExercisesSelected");

        // Подписка на выбранные упражнения
        MessagingCenter.Subscribe<ChooseExercisePage, List<Exercise>>(this,
            "ExercisesSelected", (_, list) =>
            {
                MessagingCenter.Unsubscribe<ChooseExercisePage, List<Exercise>>(this, "ExercisesSelected");

                if (list == null || list.Count == 0)
                    return;

                foreach (var ex in list)
                {
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

                SelectedExercisesView.IsVisible = _selectedExercises.Any();
            });

        // Передаём уже выбранные упражнения
        var alreadySelected = _selectedExercises.Select(e => e.Exercise).ToList();
        await Navigation.PushAsync(new ChooseExercisePage(alreadySelected));
    }

    // Вернуться назад
    private async void OnBackTapped(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    // Сохранить изменения шаблона
    private async void OnSaveClicked(object sender, EventArgs e)
    {
        var name = TemplateNameEntry.Text?.Trim();
        bool hasError = false;

        // Валидация названия
        if (string.IsNullOrWhiteSpace(name))
        {
            NameErrorLabel.IsVisible = true;
            hasError = true;
        }
        else
        {
            NameErrorLabel.IsVisible = false;
        }

        // Валидация наличия упражнений
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

        // Проверка, что у каждого упражнения есть хотя бы один заполненный подход
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

        _template.Name = name;
        var list = _selectedExercises.Select(e => new TemplateExercise
        {
            Name = e.Exercise.ExerciseName,
            MuscleGroup = e.Exercise.MuscleGroup,
            Sets = e.Sets.ToList()
        }).ToList();

        _template.ExercisesJson = System.Text.Json.JsonSerializer.Serialize(list);
        await TemplateDatabaseService.UpdateTemplateAsync(_template);
        MessagingCenter.Send(this, "TemplateUpdatedOrDeleted", true);
        await Navigation.PopAsync();
    }

    // Удалить шаблон
    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        var popup = new DeleteItemPopup();
        var confirmed = await this.ShowPopupAsync(popup);

        if (confirmed is bool result && result)
        {
            await TemplateDatabaseService.DeleteTemplateAsync(_template);
            MessagingCenter.Send(this, "TemplateUpdatedOrDeleted", true);
            await Navigation.PopAsync();
        }
    }
}
