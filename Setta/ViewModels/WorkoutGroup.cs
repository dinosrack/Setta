using System;
using System.Collections.ObjectModel;
using Setta.Models;

namespace Setta.ViewModels
{
    public class WorkoutGroup : ObservableCollection<WorkoutCardViewModel>
    {
        public string DateHeader { get; }

        public WorkoutGroup(string dateHeader, IEnumerable<WorkoutCardViewModel> items) : base(items)
        {
            DateHeader = dateHeader;
        }
    }
}
