using SQLite;
using Setta.Models;
using System.Collections.ObjectModel;

/// <summary>
/// Сервис работы с базой данных упражнений.
/// Реализует инициализацию базы, добавление, получение, обновление и удаление упражнений через SQLiteAsyncConnection.
/// Используется для хранения и управления пользовательскими упражнениями в приложении.
/// </summary>

namespace Setta.Services;

public class ExerciseDatabaseService
{
    private static SQLiteAsyncConnection _database;
    private static bool _initialized;

    // Инициализация базы данных и создание таблицы упражнений
    public static async Task InitAsync()
    {
        if (_initialized) return;

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "exercises.db");
        _database = new SQLiteAsyncConnection(dbPath);
        await _database.CreateTableAsync<Exercise>();
        _initialized = true;
    }

    // Добавить упражнение в базу
    public static async Task AddExerciseAsync(Exercise exercise)
    {
        await InitAsync();
        await _database.InsertAsync(exercise);
    }

    // Получить список всех упражнений из базы
    public static async Task<List<Exercise>> GetExercisesAsync()
    {
        await InitAsync();
        return await _database.Table<Exercise>().ToListAsync();
    }

    // Обновить упражнение в базе
    public static async Task UpdateExerciseAsync(Exercise exercise)
    {
        await InitAsync();
        await _database.UpdateAsync(exercise);
    }

    // Удалить упражнение из базы
    public static async Task DeleteExerciseAsync(Exercise exercise)
    {
        await InitAsync();
        await _database.DeleteAsync(exercise);
    }
}
