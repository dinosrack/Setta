using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Views;
using Setta.Models;
using Setta.PopupPages;

namespace Setta.Pages
{
    public partial class WorkoutInfoPage : ContentPage
    {
        private ObservableCollection<ExerciseInTemplate> _selectedExercises = new();

        public WorkoutInfoPage(int workoutId)
        {
            InitializeComponent();
            // Другой init тут...
            SelectedExercisesView.ItemsSource = _selectedExercises;
            SelectedExercisesView.IsVisible = _selectedExercises.Any();
        }

        private async void OnAddExerciseClicked(object sender, EventArgs e)
        {
            if (_selectedExercises.Count >= 7)
            {
                await this.ShowPopupAsync(new ErrorsTemplatesPopup("Можно добавить не более 7 упражнений."));
                return;
            }

            MessagingCenter.Unsubscribe<AddExerciseToTemplatePage, List<Exercise>>(this, "ExercisesSelected");
            MessagingCenter.Subscribe<AddExerciseToTemplatePage, List<Exercise>>(this, "ExercisesSelected", (sender, list) =>
            {
                MessagingCenter.Unsubscribe<AddExerciseToTemplatePage, List<Exercise>>(this, "ExercisesSelected");
                if (list == null || list.Count == 0) return;
                foreach (var ex in list)
                {
                    if (_selectedExercises.Any(e => e.Exercise.ExerciseName == ex.ExerciseName))
                        continue;
                    if (_selectedExercises.Count >= 7)
                    {
                        this.ShowPopup(new ErrorsTemplatesPopup("Можно добавить не более 7 упражнений."));
                        break;
                    }
                    _selectedExercises.Add(new ExerciseInTemplate { Exercise = ex, Sets = new ObservableCollection<ExerciseSet>() });
                }
                UpdateExerciseListUI();
            });

            var alreadySelected = _selectedExercises.Select(e => e.Exercise).ToList();
            await Navigation.PushAsync(new AddExerciseToTemplatePage(alreadySelected));
        }

        private void UpdateExerciseListUI()
        {
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
                    // Пользователь вышел без нажатия "Сохранить" или "Удалить"
                    return;
                }

                if (result is null)
                {
                    _selectedExercises.Remove(selected);
                }
                else if (result is ExerciseInTemplate updated)
                {
                    var idx = _selectedExercises.IndexOf(selected);
                    _selectedExercises[idx] = updated;
                }
                UpdateExerciseListUI();
            }
        }

        private async void OnBackTapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

    }
}
