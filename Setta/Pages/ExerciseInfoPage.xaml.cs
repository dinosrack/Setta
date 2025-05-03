using System;
using System.Collections.ObjectModel;
using Setta.Models;

namespace Setta.Pages
{
    public partial class ExerciseInfoPage : ContentPage
    {
        // �������� ��� ��������
        public string ExerciseName { get; private set; }
        public ObservableCollection<MuscleGroupFilter> MuscleGroups { get; private set; }

        // ����� ��������� ��� ��������� ���� � ������������
        public ObservableCollection<string> SecondaryMuscleGroups { get; private set; }
        public ObservableCollection<string> EquipmentList { get; private set; }

        public ExerciseInfoPage()
        {
            InitializeComponent();
        }

        // �����������, ����������� ��������� ����������
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
