using Setta.PopupPages;
using Setta.Models;
using Setta.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Maui.Views;
using System.Linq;
using Setta.Services;

namespace Setta.Pages;

public partial class ExercisesPage : ContentPage, INotifyPropertyChanged
{
    // Поисковый запрос
    private string _searchQuery;

    // Исходный список упражнений
    private ObservableCollection<Exercise> _allExercises;

    // Фильтруемый список упражнений
    private ObservableCollection<Exercise> _exercises;

    // Выбранные фильтры
    private List<string> _selectedGroups = new();
    private List<string> _selectedEquipment = new();

    // Публичное свойство для привязки списка
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

    // Публичное свойство для привязки поискового запроса
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

        // Подписка на выбранные фильтры
        MessagingCenter.Subscribe<FilterPageViewModel, Tuple<List<string>, List<string>>>(this, "FiltersApplied",
            (_, tuple) =>
            {
                _selectedGroups = tuple.Item1;
                _selectedEquipment = tuple.Item2;
                ApplyFilters();
            });

        // Подписка на новое упражнение
        MessagingCenter.Subscribe<AddExercisePage, Exercise>(this, "ExerciseAdded", async (_, _) =>
        {
            await LoadExercisesAsync(); // Обновляем упражнения после добавления
        });

        // Подписка на обновление упражнения
        MessagingCenter.Subscribe<EditExercisePage, Exercise>(this, "ExerciseUpdated", async (_, _) =>
        {
            await LoadExercisesAsync(); // Обновляем упражнения после изменения
        });

        // Подписка на удаление упражнения
        MessagingCenter.Subscribe<EditExercisePage, Exercise>(this, "ExerciseDeleted", async (_, _) =>
        {
            await LoadExercisesAsync(); // Обновляем упражнения после удаления
        });
    }

    private async Task LoadExercisesAsync()
    {
        var excelExercises = ExerciseData.Exercises;

        var dbExercises = await ExerciseDatabaseService.GetExercisesAsync();

        // Устанавливаем флаг для упражнений из базы данных
        foreach (var ex in dbExercises)
            ex.IsFromDatabase = true;

        // Сортировка по убыванию ID
        var sortedDbExercises = dbExercises
            .OrderByDescending(e => e.Id)
            .ToList();

        // Объединяем: сначала пользовательские, затем Excel
        var all = sortedDbExercises.Concat(excelExercises).ToList();

        _allExercises = new ObservableCollection<Exercise>(all);
        ApplyFilters();
    }


    // Общая логика фильтрации по группам, оборудованию и тексту
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

    // Отмечаем последний элемент списка (для отрисовки separator’а)
    private void UpdateLastItem()
    {
        for (int i = 0; i < Exercises.Count; i++)
            Exercises[i].IsLastItem = i == Exercises.Count - 1;
    }

    // Открытие страницы фильтров
    private void OnFilterClicked(object sender, EventArgs e)
    {
        var popup = new FilterPopup(_selectedGroups, _selectedEquipment);
        this.ShowPopup(popup);
    }

    private async void OnAddExerciseClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddExercisePage());
    }


    // Открытие страницы деталей упражнения
    private async void OnExerciseTapped(object sender, EventArgs e)
    {
        if ((sender as VisualElement)?.BindingContext is Exercise exercise)
            await Navigation.PushAsync(new ExerciseInfoPage(exercise));
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
