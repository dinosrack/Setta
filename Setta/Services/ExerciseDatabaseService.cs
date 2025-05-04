using SQLite;
using Setta.Models;
using System.Collections.ObjectModel;

namespace Setta.Services;

public class ExerciseDatabaseService
{
    private static SQLiteAsyncConnection _database;
    private static bool _initialized;

    public static async Task InitAsync()
    {
        if (_initialized) return;

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "exercises.db");
        _database = new SQLiteAsyncConnection(dbPath);
        await _database.CreateTableAsync<Exercise>();
        _initialized = true;
    }

    public static async Task AddExerciseAsync(Exercise exercise)
    {
        await InitAsync();
        await _database.InsertAsync(exercise);
    }

    public static async Task<List<Exercise>> GetExercisesAsync()
    {
        await InitAsync();
        return await _database.Table<Exercise>().ToListAsync();
    }

    public static async Task UpdateExerciseAsync(Exercise exercise)
    {
        await InitAsync();
        await _database.UpdateAsync(exercise);
    }

    public static async Task DeleteExerciseAsync(Exercise exercise)
    {
        await InitAsync();
        await _database.DeleteAsync(exercise);
    }
}
