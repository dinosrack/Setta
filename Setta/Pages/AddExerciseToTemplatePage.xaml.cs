using CommunityToolkit.Maui.Views;
using Setta.Models;
using Setta.PopupPages;
using Setta.ViewModels;

namespace Setta.Pages;

public partial class AddExerciseToTemplatePage : ContentPage
{
    private AddExerciseToTemplateViewModel ViewModel => BindingContext as AddExerciseToTemplateViewModel;

    public AddExerciseToTemplatePage(List<Exercise> alreadySelected)
    {
        InitializeComponent();

        // Установим исключения перед фильтрацией
        ViewModel?.SetExcludedExercises(alreadySelected);

        // Подписка на выбор фильтров из Popup
        MessagingCenter.Subscribe<FilterPageViewModel, Tuple<List<string>, List<string>>>(this,
            "FiltersApplied", (_, tuple) =>
            {
                ViewModel?.ApplyFiltersFromPopup(tuple.Item1, tuple.Item2);
            });
    }

    private async void OnBackTapped(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        var selected = ViewModel?.GetSelectedExercises();

        if (selected == null || selected.Count == 0)
        {
            await this.ShowPopupAsync(new ErrorsPopup("Выберите не менее 1 упражнения."));
            return;
        }

        MessagingCenter.Send(this, "ExercisesSelected", selected);
        await Navigation.PopAsync();
    }

    private void OnFilterClicked(object sender, EventArgs e)
    {
        if (ViewModel == null) return;

        var popup = new FilterPopup(ViewModel.SelectedMuscleGroups, ViewModel.SelectedEquipment);
        this.ShowPopup(popup);
    }
}
