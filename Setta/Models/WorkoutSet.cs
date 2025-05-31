using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Setta.Models
{
    public class WorkoutSet
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int ExerciseId { get; set; }   
        public int Reps { get; set; }
        public int Weight { get; set; }
    }

}
