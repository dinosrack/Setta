using CommunityToolkit.Maui.Views;
using Setta.Models;
using System.Collections.ObjectModel;

namespace Setta.PopupPages;
public partial class ProgressInExercisesPopup : Popup
{
    public ExerciseInTemplate Exercise { get; private set; }

    public ProgressInExercisesPopup(ExerciseInTemplate exercise)
    {
        InitializeComponent();

        Exercise = exercise;

        // ���� �������� ��� � ������ ���� ������
        if (Exercise.Sets.Count == 0)
            Exercise.Sets.Add(new ExerciseSet
            {
                Weight = null,
                Reps = null
            });

        BindingContext = Exercise;
    }

    private void OnAddSet(object sender, EventArgs e)
    {
        if (Exercise.Sets.Count >= 10)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    var popup = new ErrorsPopup("�� ������ �������� �� ����� 10 ��������.");
                    App.Current.MainPage.ShowPopup(popup);
                });
            });
            return;
        }

        Exercise.Sets.Add(new ExerciseSet());
    }


    private void OnDeleteSet(object sender, EventArgs e)
    {
        if ((sender as VisualElement)?.BindingContext is ExerciseSet set)
        {
            // ������ ������� ��������� ������
            if (Exercise.Sets.Count == 1)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    var popup = new ErrorsPopup("�� �� ������ ������� ������������ ������.");
                    App.Current.MainPage.ShowPopup(popup);
                });
                return;
            }

            Exercise.Sets.Remove(set);

            // ������� ItemsSource ������� (�������� ����������� ���� ��� �������� �������)
            SetsView.ItemsSource = null;
            SetsView.ItemsSource = Exercise.Sets;
        }
    }

    public bool IsSaved { get; private set; } = false;

    private void OnSaveExercise(object sender, EventArgs e)
    {
        IsSaved = true;
        Close(Exercise); // ������� �������
    }

    private void OnDeleteExercise(object sender, EventArgs e)
    {
        IsSaved = true;
        Close(null); // ����� null ? ��������
    }
}
