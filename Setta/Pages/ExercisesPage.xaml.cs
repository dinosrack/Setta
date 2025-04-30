using Setta.PopupPages;
using Setta.Models;
using Setta.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using CommunityToolkit.Maui.Views;

namespace Setta.Pages;

public partial class ExercisesPage : ContentPage, INotifyPropertyChanged
{
    private string _searchQuery;

    // Исходный список упражнений
    private ObservableCollection<Exercise> _allExercises;

    // Фильтруемый список упражнений
    private ObservableCollection<Exercise> _exercises;

    // храним выбранные группы между открытиями
    private List<string> _selectedGroups = new();

    public ObservableCollection<Exercise> Exercises
    {
        get => _exercises;
        set
        {
            _exercises = value;
            OnPropertyChanged();
        }
    }

    public string SearchQuery
    {
        get => _searchQuery;
        set
        {
            if (_searchQuery != value)
            {
                _searchQuery = value;
                OnPropertyChanged();
                FilterExercises();
            }
        }
    }

    public ExercisesPage()
    {
        InitializeComponent();
        BindingContext = this;

        _allExercises = new ObservableCollection<Exercise>(ExerciseData.Exercises);
        Exercises = new ObservableCollection<Exercise>(_allExercises);

        // подписка на событие применения фильтра
        MessagingCenter.Subscribe<FilterPageViewModel, List<string>>(this, "FiltersApplied", (_, selected) =>
        {
            _selectedGroups = selected;
            ApplyGroupFilter();
        });
    }

    private async void OnFilterClicked(object sender, EventArgs e)
    {
        // Открываем Popup с текущими выбранными группами
        var popup = new FilterPopup(_selectedGroups);
        // показываем его поверх страницы
        this.ShowPopup(popup);
    }

    // Применить групповой фильтр + поиск по строке (если есть)
    private void ApplyGroupFilter()
    {
        var source = ExerciseData.Exercises.AsEnumerable();
        if (_selectedGroups.Any())
            source = source.Where(e => _selectedGroups.Contains(e.MuscleGroup));

        // дополнительно фильтр по строке поиска
        if (!string.IsNullOrWhiteSpace(SearchQuery))
            source = source.Where(e => e.ExerciseName
                .Contains(SearchQuery, StringComparison.OrdinalIgnoreCase));

        Exercises = new ObservableCollection<Exercise>(source);
        UpdateLastItem();
    }

    // Фильтрация запроса поиска
    private void FilterExercises()
    {
        if (string.IsNullOrWhiteSpace(SearchQuery))
        {
            Exercises = new ObservableCollection<Exercise>(_allExercises);
        }
        else
        {
            var filteredExercises = _allExercises
                .Where(e => e.ExerciseName.ToLower().Contains(SearchQuery.ToLower()));

            Exercises = new ObservableCollection<Exercise>(filteredExercises);
        }

        UpdateLastItem();
    }

    private void UpdateLastItem()
    {
        for (int i = 0; i < Exercises.Count; i++)
        {
            Exercises[i].IsLastItem = i == Exercises.Count - 1;
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
