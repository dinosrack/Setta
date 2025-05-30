using SQLite;
using Setta.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

namespace Setta.Services
{
    public class WorkoutDatabaseService
    {
        private readonly SQLiteAsyncConnection _database;

        public WorkoutDatabaseService(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Workout>().Wait();
        }

        // Получить все тренировки
        public Task<List<Workout>> GetWorkoutsAsync()
        {
            return _database.Table<Workout>().OrderByDescending(w => w.Date).ToListAsync();
        }

        // Получить по id
        public Task<Workout> GetWorkoutAsync(int id)
        {
            return _database.Table<Workout>().Where(w => w.Id == id).FirstOrDefaultAsync();
        }

        // Сохранить новую тренировку
        public Task<int> SaveWorkoutAsync(Workout workout)
        {
            if (workout.Id == 0)
                return _database.InsertAsync(workout);
            else
                return _database.UpdateAsync(workout);
        }

        // Удалить тренировку
        public Task<int> DeleteWorkoutAsync(Workout workout)
        {
            return _database.DeleteAsync(workout);
        }
    }
}
