using System.Collections.ObjectModel;
using Setta.Models;

namespace Setta.Pages;

public partial class ExerciseInfoPage : ContentPage
{
    // Свойства для привязки
    public string ExerciseName { get; private set; }
    public ObservableCollection<MuscleGroupFilter> MuscleGroups { get; private set; }

    // Параметрless-конструктор нужен для XAML
    public ExerciseInfoPage()
    {
        InitializeComponent();
    }

    // Ваш конструктор, принимающий выбранное упражнение
    public ExerciseInfoPage(Exercise exercise) : this()
    {
        // Заполняем свойства
        ExerciseName = exercise.ExerciseName;                      
        MuscleGroups = new ObservableCollection<MuscleGroupFilter>
        {
            new MuscleGroupFilter(exercise.MuscleGroup, true)     
        };

        // Устанавливаем BindingContext уже после инициализации
        BindingContext = this;
    }

    private async void OnBackTapped(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
