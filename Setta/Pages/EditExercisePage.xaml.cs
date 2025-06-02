using Setta.Models;
using Setta.PopupPages;
using Setta.Services;
using Setta.ViewModels;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;

/// <summary>
/// Страница редактирования упражнения.
/// Позволяет изменять основную и второстепенную группу мышц, оборудование, название упражнения.
/// Поддерживает валидацию введённых данных, сохранение изменений и удаление упражнения из базы.
/// Использует всплывающие окна для выбора значений и подтверждения удаления.
/// </summary>

namespace Setta.Pages
{
    public partial class EditExercisePage : ContentPage
    {
        private const string PlaceholderUnselectedMuscle = "Не выбрана";
        private const string PlaceholderUnselectedEquipment = "Не выбрано";

        private readonly Exercise _exercise;

        public EditExercisePage(Exercise exercise)
        {
            InitializeComponent();
            _exercise = exercise;

            // Инициализация значений на странице из переданного упражнения
            ExerciseNameEntry.Text = _exercise.ExerciseName;

            PrimaryMuscleLabel.Text =
                string.IsNullOrWhiteSpace(_exercise.MuscleGroup)
                ? PlaceholderUnselectedMuscle
                : _exercise.MuscleGroup;

            SecondaryMuscleLabel.Text =
                string.IsNullOrWhiteSpace(_exercise.SecondaryMuscleGroup)
                ? PlaceholderUnselectedMuscle
                : _exercise.SecondaryMuscleGroup;

            EquipmentLabel.Text =
                string.IsNullOrWhiteSpace(_exercise.Equipment)
                ? PlaceholderUnselectedEquipment
                : _exercise.Equipment;
        }

        // Выбор основной группы мышц
        async void OnPrimaryMuscleTapped(object sender, EventArgs e)
        {
            var isPlaceholder = PrimaryMuscleLabel.Text == PlaceholderUnselectedMuscle;
            var initial = isPlaceholder
                ? Array.Empty<string>()
                : new[] { PrimaryMuscleLabel.Text };

            var popup = new SelectionPopup(
                "Основная группа",
                FilterPageViewModel.GetAvailableMuscleGroups(),
                initial,
                isMultiSelect: false);

            var result = await this.ShowPopupAsync(popup) as List<string>;
            if (result?.Any() == true)
                PrimaryMuscleLabel.Text = result.First();
        }

        // Выбор второстепенных групп мышц
        async void OnSecondaryMusclesTapped(object sender, EventArgs e)
        {
            var isPlaceholder = SecondaryMuscleLabel.Text == PlaceholderUnselectedMuscle;
            var current = isPlaceholder
                ? Array.Empty<string>()
                : SecondaryMuscleLabel.Text.Split(", ", StringSplitOptions.RemoveEmptyEntries);

            var popup = new SelectionPopup(
                "Второстепенная группа",
                FilterPageViewModel.GetAvailableMuscleGroups(),
                current,
                isMultiSelect: true);

            var result = await this.ShowPopupAsync(popup) as List<string>;
            if (result != null)
            {
                SecondaryMuscleLabel.Text = result.Any()
                    ? string.Join(", ", result)
                    : PlaceholderUnselectedMuscle;
            }
        }

        // Выбор оборудования
        async void OnEquipmentTapped(object sender, EventArgs e)
        {
            var isPlaceholder = EquipmentLabel.Text == PlaceholderUnselectedEquipment;
            var initial = isPlaceholder
                ? Array.Empty<string>()
                : new[] { EquipmentLabel.Text };

            var popup = new SelectionPopup(
                "Оборудование",
                FilterPageViewModel.GetAvailableEquipment(),
                initial,
                isMultiSelect: false);

            var result = await this.ShowPopupAsync(popup) as List<string>;
            if (result?.Any() == true)
                EquipmentLabel.Text = result.First();
        }

        // Сохранение изменений
        async void OnSaveClicked(object sender, EventArgs e)
        {
            bool hasError = false;

            // Валидация названия
            if (string.IsNullOrWhiteSpace(ExerciseNameEntry.Text))
            {
                NameErrorLabel.IsVisible = true;
                hasError = true;
            }
            else
            {
                NameErrorLabel.IsVisible = false;
            }

            // Валидация основной группы мышц
            if (PrimaryMuscleLabel.Text == PlaceholderUnselectedMuscle)
            {
                PrimaryMuscleErrorLabel.IsVisible = true;
                hasError = true;
            }
            else
            {
                PrimaryMuscleErrorLabel.IsVisible = false;
            }

            // Валидация оборудования
            if (EquipmentLabel.Text == PlaceholderUnselectedEquipment)
            {
                EquipmentErrorLabel.IsVisible = true;
                hasError = true;
            }
            else
            {
                EquipmentErrorLabel.IsVisible = false;
            }

            if (hasError)
                return;

            // Сохраняем изменения в модели
            _exercise.ExerciseName = ExerciseNameEntry.Text.Trim();
            _exercise.MuscleGroup = PrimaryMuscleLabel.Text;
            _exercise.SecondaryMuscleGroup = SecondaryMuscleLabel.Text == PlaceholderUnselectedMuscle
                ? string.Empty
                : SecondaryMuscleLabel.Text;
            _exercise.Equipment = EquipmentLabel.Text;

            await ExerciseDatabaseService.UpdateExerciseAsync(_exercise);
            MessagingCenter.Send(this, "ExerciseUpdated", _exercise);
            await Navigation.PopAsync();
        }

        // Удаление упражнения
        async void OnDeleteClicked(object sender, EventArgs e)
        {
            var popup = new DeleteItemPopup();
            var result = await this.ShowPopupAsync(popup) as bool?;
            if (result == true)
            {
                await ExerciseDatabaseService.DeleteExerciseAsync(_exercise);
                MessagingCenter.Send(this, "ExerciseDeleted", _exercise);
                await Shell.Current.Navigation.PopToRootAsync();
            }
        }

        // Вернуться на предыдущую страницу
        async void OnBackTapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
