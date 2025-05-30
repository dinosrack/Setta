using System.Collections.ObjectModel;
using Setta.Models;
using Setta.Pages;
using System.Linq;
using System.Windows.Input;

namespace Setta.ViewModels
{
    public class WorkoutPageViewModel : BindableObject
    {
        public ObservableCollection<Workout> Workouts { get; set; } = new();

        public bool IsWorkoutListEmpty => !Workouts.Any();
        public bool IsWorkoutListNotEmpty => Workouts.Any();

        public ICommand OpenWorkoutCommand => new Command<Workout>(async (workout) =>
        {
            if (workout != null)
                await Shell.Current.Navigation.PushAsync(new WorkoutInfoPage(workout.Id));
        });
    }

}
