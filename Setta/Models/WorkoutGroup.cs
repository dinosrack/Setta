using System.Collections.ObjectModel;

/// <summary>
/// Группа тренировок для отображения в списке, объединённых по дате.
/// Наследует ObservableCollection для поддержки биндинга и группировки в интерфейсе.
/// Свойство DateDisplay содержит строку для отображения даты группы (например, "13 апреля").
/// </summary>

namespace Setta.Models
{
    public class WorkoutGroup : ObservableCollection<WorkoutViewItem>
    {
        public string DateDisplay { get; } // Отображаемая дата группы

        public WorkoutGroup(string dateDisplay, IEnumerable<WorkoutViewItem> items) : base(items)
        {
            DateDisplay = dateDisplay;
        }
    }
}
