using Setta.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Setta.Pages;

public partial class ExercisesPage : ContentPage, INotifyPropertyChanged
{
    private string _searchQuery;

    // Исходный список упражнений
    private ObservableCollection<Exercise> _allExercises;

    // Фильтруемый список упражнений
    private ObservableCollection<Exercise> _exercises;

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

        // Загрузка начальных данных
        _allExercises = new ObservableCollection<Exercise>(ExerciseData.Exercises);
        Exercises = new ObservableCollection<Exercise>(_allExercises);
    }

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
