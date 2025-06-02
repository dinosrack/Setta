using SQLite;
using Setta.Models;

/// <summary>
/// Сервис работы с базой данных тренировок.
/// Реализует добавление, получение, обновление и удаление тренировок, упражнений в тренировках и подходов.
/// Поддерживает работу с шаблонами, автоматическую инициализацию и каскадное удаление.
/// Используется для всей логики хранения истории тренировок, структуры и связей между сущностями.
/// </summary>

namespace Setta.Services
{
    public class WorkoutDatabaseService
    {
        private static SQLiteAsyncConnection db;

        // Инициализация базы и создание таблиц (однократно)
        private static async Task Init()
        {
            if (db != null)
                return;

            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "workouts.db");
            db = new SQLiteAsyncConnection(databasePath);

            await db.CreateTableAsync<Workout>();
            await db.CreateTableAsync<WorkoutExercise>();
            await db.CreateTableAsync<WorkoutSet>();
        }

        // Сущность Workout

        // Добавить тренировку
        public static async Task<int> AddWorkoutAsync(Workout workout)
        {
            await Init();
            await db.InsertAsync(workout);
            return workout.Id;
        }

        // Получить список тренировок (по убыванию даты)
        public static async Task<List<Workout>> GetWorkoutsAsync()
        {
            await Init();
            return await db.Table<Workout>().OrderByDescending(w => w.StartDateTime).ToListAsync();
        }

        // Получить тренировку по id
        public static async Task<Workout> GetWorkoutByIdAsync(int id)
        {
            await Init();
            return await db.Table<Workout>().FirstOrDefaultAsync(w => w.Id == id);
        }

        // Обновить тренировку
        public static async Task UpdateWorkoutAsync(Workout workout)
        {
            await Init();
            await db.UpdateAsync(workout);
        }

        // Сущность WorkoutExercise

        // Добавить упражнение к тренировке
        public static async Task<int> AddWorkoutExerciseAsync(WorkoutExercise exercise)
        {
            await Init();
            await db.InsertAsync(exercise);
            return exercise.Id;
        }

        // Получить упражнения по id тренировки
        public static async Task<List<WorkoutExercise>> GetWorkoutExercisesAsync(int workoutId)
        {
            await Init();
            return await db.Table<WorkoutExercise>().Where(e => e.WorkoutId == workoutId).ToListAsync();
        }

        // Сущность WorkoutSet

        // Добавить подход
        public static async Task<int> AddWorkoutSetAsync(WorkoutSet set)
        {
            await Init();
            await db.InsertAsync(set);
            return set.Id;
        }

        // Получить подходы по id упражнения
        public static async Task<List<WorkoutSet>> GetWorkoutSetsAsync(int exerciseId)
        {
            await Init();
            return await db.Table<WorkoutSet>().Where(s => s.ExerciseId == exerciseId).ToListAsync();
        }

        // Удалить тренировку и все связанные упражнения и подходы
        public static async Task DeleteWorkoutAsync(int workoutId)
        {
            await Init();

            var exercises = await GetWorkoutExercisesAsync(workoutId);
            foreach (var ex in exercises)
            {
                await db.Table<WorkoutSet>().DeleteAsync(s => s.ExerciseId == ex.Id);
            }

            await db.Table<WorkoutExercise>().DeleteAsync(e => e.WorkoutId == workoutId);
            await db.DeleteAsync<Workout>(workoutId);
        }

        // Удалить упражнение с подходами по id упражнения в тренировке
        public static async Task DeleteWorkoutExerciseWithSetsAsync(int workoutExerciseId)
        {
            await Init();
            await db.Table<WorkoutSet>().DeleteAsync(s => s.ExerciseId == workoutExerciseId);
            await db.Table<WorkoutExercise>().DeleteAsync(e => e.Id == workoutExerciseId);
        }

        // Применить шаблон к тренировке (добавить упражнения и подходы из шаблона)
        public static async Task ApplyTemplateToWorkoutAsync(int workoutId, WorkoutTemplate template, int existingCount = 0)
        {
            await Init();
            int totalAdded = existingCount;

            foreach (var templateExercise in template.Exercises)
            {
                if (totalAdded >= 7) break;

                var workoutExercise = new WorkoutExercise
                {
                    WorkoutId = workoutId,
                    Name = templateExercise.Name,
                    MuscleGroup = templateExercise.MuscleGroup
                };

                await db.InsertAsync(workoutExercise);
                int exerciseId = workoutExercise.Id;

                if (templateExercise.Sets != null)
                {
                    foreach (var set in templateExercise.Sets)
                    {
                        await db.InsertAsync(new WorkoutSet
                        {
                            ExerciseId = exerciseId,
                            Reps = int.TryParse(set.Reps, out var reps) ? reps : 0,
                            Weight = int.TryParse(set.Weight, out var weight) ? weight : 0
                        });
                    }
                }

                totalAdded++;
            }
        }
    }
}
