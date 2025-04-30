using System.Collections.ObjectModel;
using Setta.Models;

namespace Setta.Pages;

public partial class ExerciseInfoPage : ContentPage
{
    // �������� ��� ��������
    public string ExerciseName { get; private set; }
    public ObservableCollection<MuscleGroupFilter> MuscleGroups { get; private set; }

    // ��������less-����������� ����� ��� XAML
    public ExerciseInfoPage()
    {
        InitializeComponent();
    }

    // ��� �����������, ����������� ��������� ����������
    public ExerciseInfoPage(Exercise exercise) : this()
    {
        // ��������� ��������
        ExerciseName = exercise.ExerciseName;                      
        MuscleGroups = new ObservableCollection<MuscleGroupFilter>
        {
            new MuscleGroupFilter(exercise.MuscleGroup, true)     
        };

        // ������������� BindingContext ��� ����� �������������
        BindingContext = this;
    }

    private async void OnBackTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
