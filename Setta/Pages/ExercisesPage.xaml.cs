using Setta.PopupPages;
using Setta.Models;
using Setta.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Maui.Views;
using System.Linq;
using Setta.Services;

/// <summary>
/// Страница списка упражнений с возможностью поиска, фильтрации и добавления новых.
/// Загружает пользовательские и предустановленные упражнения, поддерживает фильтрацию по группе мышц, оборудованию и названию.
/// Позволяет добавлять, просматривать, редактировать и удалять упражнения.
/// Все действия отображаются в реальном времени благодаря подпискам через MessagingCenter.
/// </summary>

namespace Setta.Pages;

public partial class ExercisesPage : ContentPage, INotifyPropertyChanged
{
    // Поисковый запрос
    private string _searchQuery;

    // Исходный список упражнений (до фильтрации)
    private ObservableCollection<Exercise> _allExercises;

    // Текущий отображаемый (фильтрованный) список
    private ObservableCollection<Exercise> _exercises;

    // Выбранные фильтры
    private List<string> _selectedGroups = new();
    private List<string> _selectedEquipment = new();

    // Привязка списка для UI
    public ObservableCollection<Exercise> Exercises
    {
        get => _exercises;
        set
        {
            if (_exercises == value) return;
            _exercises = value;
            OnPropertyChanged();
        }
    }

    // Привязка поискового запроса
    public string SearchQuery
    {
        get => _searchQuery;
        set
        {
            if (_searchQuery == value) return;
            _searchQuery = value;
            OnPropertyChanged();
            ApplyFilters();
        }
    }

    public ExercisesPage()
    {
        InitializeComponent();
        BindingContext = this;

        _ = LoadExercisesAsync();

        // Подписка на изменения фильтров
        MessagingCenter.Subscribe<FilterPageViewModel, Tuple<List<string>, List<string>>>(this, "FiltersApplied",
            (_, tuple) =>
            {
                _selectedGroups = tuple.Item1;
                _selectedEquipment = tuple.Item2;
                ApplyFilters();
            });

        // Подписка на добавление упражнения
        MessagingCenter.Subscribe<AddExercisePage, Exercise>(this, "ExerciseAdded", async (_, _) =>
        {
            await LoadExercisesAsync(); // Перезагружаем упражнения
        });

        // Подписка на изменение упражнения
        MessagingCenter.Subscribe<EditExercisePage, Exercise>(this, "ExerciseUpdated", async (_, _) =>
        {
            await LoadExercisesAsync();
        });

        // Подписка на удаление упражнения
        MessagingCenter.Subscribe<EditExercisePage, Exercise>(this, "ExerciseDeleted", async (_, _) =>
        {
            await LoadExercisesAsync();
        });
    }

    // Загрузка всех упражнений: из Excel и базы данных
    private async Task LoadExercisesAsync()
    {
        var excelExercises = ExerciseData.Exercises;

        var dbExercises = await ExerciseDatabaseService.GetExercisesAsync();

        // Отмечаем, что упражнения из базы
        foreach (var ex in dbExercises)
            ex.IsFromDatabase = true;

        // Сортировка по убыванию ID (новые сверху)
        var sortedDbExercises = dbExercises
            .OrderByDescending(e => e.Id)
            .ToList();

        // Сначала пользовательские, затем Excel-упражнения
        var all = sortedDbExercises.Concat(excelExercises).ToList();

        _allExercises = new ObservableCollection<Exercise>(all);
        ApplyFilters();
    }

    // Применить фильтры по группе мышц, оборудованию и поиску
    private void ApplyFilters()
    {
        var source = _allExercises.AsEnumerable();

        if (_selectedGroups.Any())
            source = source.Where(e => _selectedGroups.Contains(e.MuscleGroup));

        if (_selectedEquipment.Any())
            source = source.Where(e => e.EquipmentList.Any(eq => _selectedEquipment.Contains(eq)));

        if (!string.IsNullOrWhiteSpace(SearchQuery))
            source = source.Where(e =>
                e.ExerciseName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase));

        Exercises = new ObservableCollection<Exercise>(source);
        UpdateLastItem();
    }

    // Отметить последний элемент для правильного отображения разделителей
    private void UpdateLastItem()
    {
        for (int i = 0; i < Exercises.Count; i++)
            Exercises[i].IsLastItem = i == Exercises.Count - 1;
    }

    // Открыть окно фильтрации
    private void OnFilterClicked(object sender, EventArgs e)
    {
        var popup = new FilterPopup(_selectedGroups, _selectedEquipment);
        this.ShowPopup(popup);
    }

    // Открыть страницу добавления нового упражнения
    private async void OnAddExerciseClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddExercisePage());
    }

    // Открыть страницу подробной информации об упражнении
    private async void OnExerciseTapped(object sender, EventArgs e)
    {
        if ((sender as VisualElement)?.BindingContext is Exercise exercise)
            await Navigation.PushAsync(new ExerciseInfoPage(exercise));
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
