using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using Setta.Models;
using Setta.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Setta.PopupPages;

/// <summary>
/// Страница управления шаблонами тренировок.
/// Позволяет создавать, редактировать и удалять шаблоны (до 3-х штук).
/// Обновляет список шаблонов в реальном времени при добавлении, изменении или удалении.
/// Использует всплывающие окна для отображения ошибок и подтверждений.
/// </summary>

namespace Setta.Pages;

public partial class TemplatePage : ContentPage
{
    // Коллекция шаблонов для отображения в списке
    private ObservableCollection<WorkoutTemplate> _templates = new();

    public TemplatePage()
    {
        InitializeComponent();
        LoadTemplates();

        // Подписка на добавление нового шаблона
        MessagingCenter.Subscribe<AddTemplatePage, WorkoutTemplate>(this, "TemplateCreated", (_, _) =>
        {
            LoadTemplates();
        });

        // Подписка на обновление после редактирования или удаления
        MessagingCenter.Subscribe<EditTemplatePage, bool>(this, "TemplateUpdatedOrDeleted", (_, _) =>
        {
            LoadTemplates();
        });
    }

    // Загрузка шаблонов из базы данных
    private async void LoadTemplates()
    {
        var templates = await TemplateDatabaseService.GetTemplatesAsync();

        _templates = new ObservableCollection<WorkoutTemplate>(templates);
        TemplatesView.ItemsSource = _templates;

        NoTemplateLabel.IsVisible = !_templates.Any();
        TemplatesView.IsVisible = _templates.Any();
    }

    // Обработка нажатия на кнопку добавления шаблона
    private async void OnAddTemplateClicked(object sender, EventArgs e)
    {
        if (_templates.Count >= 3)
        {
            var popup = new ErrorsPopup("Вы можете создать не более 3 шаблонов.");
            await this.ShowPopupAsync(popup);
            return;
        }

        await Navigation.PushAsync(new AddTemplatePage());
    }

    // Команда для открытия страницы редактирования шаблона
    public ICommand TemplateTapCommand => new Command<WorkoutTemplate>(async (template) =>
    {
        await Navigation.PushAsync(new EditTemplatePage(template));
    });
}
