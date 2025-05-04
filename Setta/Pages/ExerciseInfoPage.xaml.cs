using Microsoft.Maui.Controls;
using Setta.Models;

namespace Setta.Pages
{
    public partial class ExerciseInfoPage : ContentPage
    {
        private Exercise _exercise;

        public ExerciseInfoPage(Exercise exercise)
        {
            InitializeComponent();
            _exercise = exercise;
            BindingContext = _exercise;

            MessagingCenter.Subscribe<EditExercisePage, Exercise>(this, "ExerciseUpdated", (_, updatedExercise) =>
            {
                _exercise.ExerciseName = updatedExercise.ExerciseName;
                _exercise.MuscleGroup = updatedExercise.MuscleGroup;
                _exercise.SecondaryMuscleGroup = updatedExercise.SecondaryMuscleGroup;
                _exercise.Equipment = updatedExercise.Equipment;

                // Принудительно обновляем BindingContext, если нет INotifyPropertyChanged
                BindingContext = null;
                BindingContext = _exercise;
            });
        }

        private async void OnEditClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EditExercisePage(_exercise));
        }

        private async void OnBackTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<EditExercisePage, Exercise>(this, "ExerciseUpdated");
        }
    }
}
