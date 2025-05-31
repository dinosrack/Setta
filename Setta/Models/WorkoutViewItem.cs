using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Setta.Models
{
    public class WorkoutViewItem
    {
        public int Id { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public int TotalWeight { get; set; }

        public bool IsActive => EndDateTime == null;

        public string TimeDisplay => IsActive ? "Сейчас" : $"{StartDateTime:HH:mm} - {EndDateTime?.ToString("HH:mm")}";

        public string SummaryText =>
            !IsActive && EndDateTime.HasValue
                ? $"{(int)(EndDateTime.Value - StartDateTime).TotalMinutes} мин | {TotalWeight} кг"
                : "";
    }
}
