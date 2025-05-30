using CommunityToolkit.Maui.Views;

namespace Setta.PopupPages
{
    public partial class AddWorkoutPopup : Popup
    {
        public AddWorkoutPopup(DateTime selectedDate)
        {
            InitializeComponent();
            WorkoutDatePicker.Date = selectedDate;
        }

        private void OnEmptyClicked(object sender, EventArgs e)
        {
            Close(new Tuple<DateTime, bool>(WorkoutDatePicker.Date, false));
        }

        private void OnTemplateClicked(object sender, EventArgs e)
        {
            Close(new Tuple<DateTime, bool>(WorkoutDatePicker.Date, true));
        }
    }
}
