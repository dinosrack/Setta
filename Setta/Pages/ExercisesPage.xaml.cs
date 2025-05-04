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
    // ��������� ������
    private string _searchQuery;

    // �������� ������ ����������
    private ObservableCollection<Exercise> _allExercises;

    // ����������� ������ ����������
    private ObservableCollection<Exercise> _exercises;

    // ��������� �������
    private List<string> _selectedGroups = new();
    private List<string> _selectedEquipment = new();

    // ��������� �������� ��� �������� ������
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

    // ��������� �������� ��� �������� ���������� �������
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

        // �������� �� ��������� �������
        MessagingCenter.Subscribe<FilterPageViewModel, Tuple<List<string>, List<string>>>(this, "FiltersApplied",
            (_, tuple) =>
            {
                _selectedGroups = tuple.Item1;
                _selectedEquipment = tuple.Item2;
                ApplyFilters();
            });

        // �������� �� ����� ����������
        MessagingCenter.Subscribe<AddExercisePage, Exercise>(this, "ExerciseAdded", async (_, _) =>
        {
            await LoadExercisesAsync(); // ��������� ���������� ����� ����������
        });

        // �������� �� ���������� ����������
        MessagingCenter.Subscribe<EditExercisePage, Exercise>(this, "ExerciseUpdated", async (_, _) =>
        {
            await LoadExercisesAsync(); // ��������� ���������� ����� ���������
        });

        // �������� �� �������� ����������
        MessagingCenter.Subscribe<EditExercisePage, Exercise>(this, "ExerciseDeleted", async (_, _) =>
        {
            await LoadExercisesAsync(); // ��������� ���������� ����� ��������
        });
    }

    private async Task LoadExercisesAsync()
    {
        var excelExercises = ExerciseData.Exercises;

        var dbExercises = await ExerciseDatabaseService.GetExercisesAsync();

        // ������������� ���� ��� ���������� �� ���� ������
        foreach (var ex in dbExercises)
            ex.IsFromDatabase = true;

        // ���������� �� �������� ID
        var sortedDbExercises = dbExercises
            .OrderByDescending(e => e.Id)
            .ToList();

        // ����������: ������� ����������������, ����� Excel
        var all = sortedDbExercises.Concat(excelExercises).ToList();

        _allExercises = new ObservableCollection<Exercise>(all);
        ApplyFilters();
    }


    // ����� ������ ���������� �� �������, ������������ � ������
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

    // �������� ��������� ������� ������ (��� ��������� separator��)
    private void UpdateLastItem()
    {
        for (int i = 0; i < Exercises.Count; i++)
            Exercises[i].IsLastItem = i == Exercises.Count - 1;
    }

    // �������� �������� ��������
    private void OnFilterClicked(object sender, EventArgs e)
    {
        var popup = new FilterPopup(_selectedGroups, _selectedEquipment);
        this.ShowPopup(popup);
    }

    private async void OnAddExerciseClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddExercisePage());
    }


    // �������� �������� ������� ����������
    private async void OnExerciseTapped(object sender, EventArgs e)
    {
        if ((sender as VisualElement)?.BindingContext is Exercise exercise)
            await Navigation.PushAsync(new ExerciseInfoPage(exercise));
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
