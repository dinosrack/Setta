using CommunityToolkit.Maui.Views;
using Setta.Models;
using Setta.PopupPages;
using Setta.ViewModels;

/// <summary>
/// Страница добавления упражнений в шаблон тренировки.
/// Позволяет выбрать упражнения из общего списка, применить фильтры по группе мышц и оборудованию,
/// и отправить выбранные элементы обратно в вызывающий компонент.
/// Использует всплывающие окна для фильтрации и отображения ошибок.
/// </summary>

namespace Setta.Pages;

public partial class ChooseExercisePage : ContentPage
{
    private ChooseExerciseViewModel ViewModel => BindingContext as ChooseExerciseViewModel;

    public ChooseExercisePage(List<Exercise> alreadySelected)
    {
        InitializeComponent();

        // Установим исключения перед фильтрацией (убираем уже выбранные упражнения)
        ViewModel?.SetExcludedExercises(alreadySelected);

        // Подписка на применение фильтров через popup
        MessagingCenter.Subscribe<FilterPageViewModel, Tuple<List<string>, List<string>>>(this,
            "FiltersApplied", (_, tuple) =>
            {
                ViewModel?.ApplyFiltersFromPopup(tuple.Item1, tuple.Item2);
            });
    }

    // Возврат назад по тапу
    private async void OnBackTapped(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    // Добавление выбранных упражнений в шаблон
    private async void OnAddClicked(object sender, EventArgs e)
    {
        var selected = ViewModel?.GetSelectedExercises();

        if (selected == null || selected.Count == 0)
        {
            await this.ShowPopupAsync(new ErrorsPopup("Выберите не менее 1 упражнения."));
            return;
        }

        // Передаём выбранные упражнения и закрываем страницу
        MessagingCenter.Send(this, "ExercisesSelected", selected);
        await Navigation.PopAsync();
    }

    // Открытие окна фильтрации
    private void OnFilterClicked(object sender, EventArgs e)
    {
        if (ViewModel == null) return;

        var popup = new FilterPopup(ViewModel.SelectedMuscleGroups, ViewModel.SelectedEquipment);
        this.ShowPopup(popup);
    }
}
