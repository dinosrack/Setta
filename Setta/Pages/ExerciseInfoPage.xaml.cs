using System;
using System.Collections.ObjectModel;
using Setta.Models;

namespace Setta.Pages
{
    public partial class ExerciseInfoPage : ContentPage
    {
        // Свойства для привязки
        public string ExerciseName { get; private set; }
        public ObservableCollection<MuscleGroupFilter> MuscleGroups { get; private set; }

        // Новые коллекции для вторичных мышц и оборудования
        public ObservableCollection<string> SecondaryMuscleGroups { get; private set; }
        public ObservableCollection<string> EquipmentList { get; private set; }

        public ExerciseInfoPage()
        {
            InitializeComponent();
        }

        // Конструктор, принимающий выбранное упражнение
        public ExerciseInfoPage(Exercise exercise) : this()
        {
            ExerciseName = exercise.ExerciseName;
            MuscleGroups = new ObservableCollection<MuscleGroupFilter>
        { new(exercise.MuscleGroup, true) };

            SecondaryMuscleGroups =
              new ObservableCollection<string>(exercise.SecondaryMuscleGroups);
            EquipmentList =
              new ObservableCollection<string>(exercise.EquipmentList);

            BindingContext = this;
        }

        private async void OnBackTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
