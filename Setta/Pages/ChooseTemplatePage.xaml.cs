using Setta.Models;
using Setta.Services;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Views;
using Setta.PopupPages;

namespace Setta.Pages;

public partial class ChooseTemplatePage : ContentPage
{
    private ObservableCollection<WorkoutTemplate> _templates = new();

    public WorkoutTemplate SelectedTemplate { get; set; }

    public event EventHandler<WorkoutTemplate> TemplateChosen;

    public ChooseTemplatePage()
    {
        InitializeComponent();
        LoadTemplates();
    }

    private async void LoadTemplates()
    {
        var templates = await TemplateDatabaseService.GetTemplatesAsync();
        _templates = new ObservableCollection<WorkoutTemplate>(templates);
        TemplateList.ItemsSource = _templates;
    }

    private async void OnChooseClicked(object sender, EventArgs e)
    {
        if (SelectedTemplate == null)
        {
            await this.ShowPopupAsync(new ErrorsTemplatesPopup("Вам нужно выбрать шаблон."));
            return;
        }

        TemplateChosen?.Invoke(this, SelectedTemplate);
        await Navigation.PopAsync();
    }

    private async void OnBackTapped(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private void OnTemplateChecked(object sender, CheckedChangedEventArgs e)
    {
        if (!e.Value) return;

        if (sender is RadioButton rb && rb.BindingContext is WorkoutTemplate selected)
        {
            // Снять выделение со всех
            foreach (var template in _templates)
                template.IsSelected = false;

            selected.IsSelected = true;
            SelectedTemplate = selected;

            // Обновить отображение вручную (если не используешь INotifyPropertyChanged)
            TemplateList.ItemsSource = null;
            TemplateList.ItemsSource = _templates;
        }
    }
}
