using System;
using System.Collections.ObjectModel;
using Setta.Models;
using Microsoft.Maui.Controls;

namespace Setta.Pages
{
    public partial class ExerciseInfoPage : ContentPage
    {
        // Свойства для привязки
        public string ExerciseName { get; private set; }
        public string MuscleGroup { get; private set; }
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
            MuscleGroup = exercise.MuscleGroup;

            SecondaryMuscleGroups = new ObservableCollection<string>(exercise.SecondaryMuscleGroups);
            EquipmentList = new ObservableCollection<string>(exercise.EquipmentList);

            BindingContext = this;
        }

        private async void OnBackTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
