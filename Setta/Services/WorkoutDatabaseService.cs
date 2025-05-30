using SQLite;
using Setta.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

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

        public Task<int> SaveWorkoutAsync(Workout workout)
        {
            if (workout.Id == 0)
                return _database.InsertAsync(workout);
            else
                return _database.UpdateAsync(workout);
        }

        public Task<List<Workout>> GetAllWorkoutsAsync()
        {
            return _database.Table<Workout>().ToListAsync();
        }

        public Task<List<Workout>> GetWorkoutsAsync()
        {
            return _database.Table<Workout>().ToListAsync();
        }

        public Task<Workout> GetWorkoutByIdAsync(int id)
        {
            return _database.Table<Workout>().FirstOrDefaultAsync(w => w.Id == id);
        }

        public Task<int> DeleteWorkoutAsync(Workout workout)
        {
            return _database.DeleteAsync(workout);
        }
    }
}
