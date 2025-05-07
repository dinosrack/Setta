using CommunityToolkit.Maui.Views;
using Setta.Models;
using Setta.PopupPages;
using Setta.Services;
using System.Collections.ObjectModel;

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

    private async void OnExerciseTapped(object sender, EventArgs e)
    {
        if ((sender as VisualElement)?.BindingContext is ExerciseInTemplate selected)
        {
            var popup = new ProgressInExercisesPopup(selected);
            var result = await this.ShowPopupAsync(popup);

            if (popup.IsSaved == false)
            {
                // ������������ ����� ��� ������� "���������" ��� "�������" � ������ �� ������
                return;
            }

            if (result is null)
            {
                // ��������
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

    private async void OnAddExercisesClicked(object sender, EventArgs e)
    {
        // �������� ������ �� �������� �������� ������
        if (_selectedExercises.Count >= 7)
        {
            await this.ShowPopupAsync(new ErrorsTemplatesPopup("�� ������ �������� �� ����� 7 ���������� � 1 ������."));
            return;
        }

        // ������� ��������, ���� ��� ��� ����
        MessagingCenter.Unsubscribe<AddExerciseToTemplatePage, List<Exercise>>(this, "ExercisesSelected");

        // �������� �� ��������� ����������
        MessagingCenter.Subscribe<AddExerciseToTemplatePage, List<Exercise>>(this,
            "ExercisesSelected", (_, list) =>
            {
                MessagingCenter.Unsubscribe<AddExerciseToTemplatePage, List<Exercise>>(this, "ExercisesSelected");

                if (list == null || list.Count == 0)
                    return;

                foreach (var ex in list)
                {
                    if (_selectedExercises.Any(e => e.Exercise.ExerciseName == ex.ExerciseName))
                        continue;

                    if (_selectedExercises.Count >= 7)
                    {
                        this.ShowPopup(new ErrorsTemplatesPopup("�� ������ �������� �� ����� 7 ���������� � 1 ������."));
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

        // ������� ��� ��������� ����������
        var alreadySelected = _selectedExercises.Select(e => e.Exercise).ToList();
        await Navigation.PushAsync(new AddExerciseToTemplatePage(alreadySelected));
    }


    private async void OnBackTapped(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
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
                await this.ShowPopupAsync(new ErrorsTemplatesPopup("���������� ������ ��������� �� ����� 1 ������������ �������."));
                return;
            }

            if (ex.Sets.Any(set => set.Weight == null || set.Reps == null))
            {
                await this.ShowPopupAsync(new ErrorsTemplatesPopup("���������� ������ ��������� ����������� �������."));
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
