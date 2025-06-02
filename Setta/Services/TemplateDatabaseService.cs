using SQLite;
using Setta.Models;

/// <summary>
/// Сервис для работы с базой данных шаблонов тренировок.
/// Выполняет инициализацию, добавление, получение, обновление и удаление шаблонов с помощью SQLiteAsyncConnection.
/// Используется для хранения и управления пользовательскими шаблонами тренировок.
/// </summary>

namespace Setta.Services;

public static class TemplateDatabaseService
{
    private static SQLiteAsyncConnection _db;
    private static bool _initialized;

    // Инициализация базы данных и создание таблицы шаблонов
    public static async Task InitAsync()
    {
        if (_initialized) return;

        var path = Path.Combine(FileSystem.AppDataDirectory, "templates.db");
        _db = new SQLiteAsyncConnection(path);
        await _db.CreateTableAsync<WorkoutTemplate>();

        _initialized = true;
    }

    // Получить все шаблоны
    public static async Task<List<WorkoutTemplate>> GetTemplatesAsync()
    {
        await InitAsync();
        return await _db.Table<WorkoutTemplate>().ToListAsync();
    }

    // Добавить новый шаблон
    public static async Task AddTemplateAsync(WorkoutTemplate template)
    {
        await InitAsync();
        await _db.InsertAsync(template);
    }

    // Удалить шаблон
    public static async Task DeleteTemplateAsync(WorkoutTemplate template)
    {
        await InitAsync();
        await _db.DeleteAsync(template);
    }

    // Обновить шаблон
    public static async Task UpdateTemplateAsync(WorkoutTemplate template)
    {
        await InitAsync();
        await _db.UpdateAsync(template);
    }
}
