using SQLite;
using Setta.Models;

namespace Setta.Services
{
    public class WorkoutDatabaseService
    {
        private static SQLiteAsyncConnection db;

        private static async Task Init()
        {
            if (db != null)
                return;

            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "setta.db");
            db = new SQLiteAsyncConnection(databasePath);

            await db.CreateTableAsync<Workout>();
            await db.CreateTableAsync<WorkoutExercise>();
            await db.CreateTableAsync<WorkoutSet>();
        }

        // ==== Workout ====

        public static async Task<int> AddWorkoutAsync(Workout workout)
        {
            await Init();
            return await db.InsertAsync(workout);
        }

        public static async Task<List<Workout>> GetWorkoutsAsync()
        {
            await Init();
            return await db.Table<Workout>().OrderByDescending(w => w.StartDateTime).ToListAsync();
        }

        public static async Task<Workout> GetWorkoutByIdAsync(int id)
        {
            await Init();
            return await db.Table<Workout>().FirstOrDefaultAsync(w => w.Id == id);
        }

        public static async Task UpdateWorkoutAsync(Workout workout)
        {
            await Init();
            await db.UpdateAsync(workout);
        }

        // ==== WorkoutExercise ====

        public static async Task<int> AddWorkoutExerciseAsync(WorkoutExercise exercise)
        {
            await Init();
            return await db.InsertAsync(exercise);
        }

        public static async Task<List<WorkoutExercise>> GetWorkoutExercisesAsync(int workoutId)
        {
            await Init();
            return await db.Table<WorkoutExercise>().Where(e => e.WorkoutId == workoutId).ToListAsync();
        }

        // ==== WorkoutSet ====

        public static async Task<int> AddWorkoutSetAsync(WorkoutSet set)
        {
            await Init();
            return await db.InsertAsync(set);
        }

        public static async Task<List<WorkoutSet>> GetWorkoutSetsAsync(int exerciseId)
        {
            await Init();
            return await db.Table<WorkoutSet>().Where(s => s.ExerciseId == exerciseId).ToListAsync();
        }

        public static async Task DeleteWorkoutAsync(int id)
        {
            await Init();
            await db.DeleteAsync<Workout>(id);
        }

        public static async Task ApplyTemplateToWorkoutAsync(int workoutId, WorkoutTemplate template)
        {
            await Init();

            foreach (var templateExercise in template.Exercises)
            {
                // Добавляем упражнение в тренировку
                var workoutExercise = new WorkoutExercise
                {
                    WorkoutId = workoutId,
                    Name = templateExercise.Name,
                    MuscleGroup = templateExercise.MuscleGroup
                };

                await db.InsertAsync(workoutExercise);

                // Получаем ID добавленного упражнения
                int exerciseId = workoutExercise.Id;

                // Добавляем подходы
                if (templateExercise.Sets != null)
                {
                    foreach (var set in templateExercise.Sets)
                    {
                        var workoutSet = new WorkoutSet
                        {
                            ExerciseId = exerciseId,
                            Reps = int.TryParse(set.Reps, out var reps) ? reps : 0,
                            Weight = int.TryParse(set.Weight, out var weight) ? weight : 0
                        };

                        await db.InsertAsync(workoutSet);
                    }
                }
            }
        }

    }
}
