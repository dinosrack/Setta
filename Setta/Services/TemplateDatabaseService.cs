using SQLite;
using Setta.Models;

namespace Setta.Services;

public static class TemplateDatabaseService
{
    private static SQLiteAsyncConnection _db;
    private static bool _initialized;

    public static async Task InitAsync()
    {
        if (_initialized) return;

        var path = Path.Combine(FileSystem.AppDataDirectory, "templates.db");
        _db = new SQLiteAsyncConnection(path);
        await _db.CreateTableAsync<WorkoutTemplate>();

        _initialized = true;
    }

    public static async Task<List<WorkoutTemplate>> GetTemplatesAsync()
    {
        await InitAsync();
        return await _db.Table<WorkoutTemplate>().ToListAsync();
    }

    public static async Task AddTemplateAsync(WorkoutTemplate template)
    {
        await InitAsync();
        await _db.InsertAsync(template);
    }

    public static async Task DeleteTemplateAsync(WorkoutTemplate template)
    {
        await InitAsync();
        await _db.DeleteAsync(template);
    }

    public static async Task UpdateTemplateAsync(WorkoutTemplate template)
    {
        await InitAsync();
        await _db.UpdateAsync(template);
    }
}
