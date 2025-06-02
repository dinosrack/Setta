using Setta.Models;
using Setta.Services;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Views;
using Setta.PopupPages;

/// <summary>
/// Страница выбора шаблона тренировки из существующих.
/// Позволяет выбрать только один шаблон, отправить выбранный шаблон в вызывающий компонент или вернуться назад.
/// Предусмотрены проверки выбора и всплывающее окно для ошибок.
/// </summary>

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

    // Загрузка шаблонов из базы
    private async void LoadTemplates()
    {
        var templates = await TemplateDatabaseService.GetTemplatesAsync();
        _templates = new ObservableCollection<WorkoutTemplate>(templates);
        TemplateList.ItemsSource = _templates;
    }

    // Подтвердить выбор шаблона
    private async void OnChooseClicked(object sender, EventArgs e)
    {
        if (SelectedTemplate == null)
        {
            await this.ShowPopupAsync(new ErrorsPopup("Вам нужно выбрать шаблон."));
            return;
        }

        TemplateChosen?.Invoke(this, SelectedTemplate);
        await Navigation.PopAsync();
    }

    // Вернуться назад
    private async void OnBackTapped(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    // Обработка выбора шаблона (радиокнопка)
    private void OnTemplateChecked(object sender, CheckedChangedEventArgs e)
    {
        if (!e.Value) return;

        if (sender is RadioButton rb && rb.BindingContext is WorkoutTemplate selected)
        {
            // Снять выделение со всех шаблонов
            foreach (var template in _templates)
                template.IsSelected = false;

            selected.IsSelected = true;
            SelectedTemplate = selected;

            // Принудительно обновить отображение
            TemplateList.ItemsSource = null;
            TemplateList.ItemsSource = _templates;
        }
    }
}
