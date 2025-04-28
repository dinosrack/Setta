using Setta.Models;
using System.Collections.ObjectModel;

namespace Setta.Pages;

public partial class ExercisesPage : ContentPage
{
	public ExercisesPage()
	{
		InitializeComponent();
        BindingContext = this;
    }

    public ObservableCollection<Models.Exercise> Exercises => ExerciseData.Exercises;
}