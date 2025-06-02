using Microsoft.Maui.Controls;
using Setta.Models;

/// <summary>
/// Страница отображения подробной информации об упражнении.
/// Показывает все данные по выбранному упражнению, позволяет перейти к редактированию.
/// Обновляет данные после изменения и обеспечивает возврат назад.
/// Использует MessagingCenter для обработки обновления информации.
/// </summary>

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

            // Подписка на обновление данных упражнения после редактирования
            MessagingCenter.Subscribe<EditExercisePage, Exercise>(this, "ExerciseUpdated", (_, updatedExercise) =>
            {
                _exercise.ExerciseName = updatedExercise.ExerciseName;
                _exercise.MuscleGroup = updatedExercise.MuscleGroup;
                _exercise.SecondaryMuscleGroup = updatedExercise.SecondaryMuscleGroup;
                _exercise.Equipment = updatedExercise.Equipment;

                // Принудительное обновление BindingContext, если не реализован INotifyPropertyChanged
                BindingContext = null;
                BindingContext = _exercise;
            });
        }

        // Переход к странице редактирования упражнения
        private async void OnEditClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EditExercisePage(_exercise));
        }

        // Возврат на предыдущую страницу
        private async void OnBackTapped(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }

        // Отписка от событий при уходе со страницы
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MessagingCenter.Unsubscribe<EditExercisePage, Exercise>(this, "ExerciseUpdated");
        }
    }
}
