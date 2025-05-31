using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Setta.Models
{
    public class Workout
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public DateTime StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }

        public bool IsActive => EndDateTime == null;

        public int TotalWeight { get; set; }
        public TimeSpan Duration => (EndDateTime ?? DateTime.Now) - StartDateTime;
    }

}
