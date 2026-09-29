using EventServiceApi.Data.Models;

using Microsoft.EntityFrameworkCore;

namespace EventServiceApi.Data;

/// <summary>
/// Контекст базы данных приложения
/// </summary>
/// <param name="options"></param>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    /// <summary>
    /// DbSet для работы с событиями
    /// </summary>
    public DbSet<Event> Events { get; set; }
}
