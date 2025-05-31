using System.Collections.ObjectModel;

namespace Setta.Models
{
    public class WorkoutGroup : ObservableCollection<WorkoutViewItem>
    {
        public string DateDisplay { get; }

        public WorkoutGroup(string dateDisplay, IEnumerable<WorkoutViewItem> items) : base(items)
        {
            DateDisplay = dateDisplay;
        }
    }
}