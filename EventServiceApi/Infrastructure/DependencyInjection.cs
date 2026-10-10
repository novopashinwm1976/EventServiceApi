using EventServiceApi.Data;
using EventServiceApi.Infrastructure.Seed;

using Microsoft.EntityFrameworkCore;

namespace EventServiceApi.Infrastructure;

/// <summary>
/// Extension метод для регистрации инфраструктурных сервисов в контейнере зависимостей
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Регистрация инфраструктурных сервисов в контейнере зависимостей
    /// </summary>
    /// <param name="services">Контейнер зависимостей</param>
    /// <returns></returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // База данных
        services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("EventsDb"));

        return services;
    }

    /// <summary>
    /// Заполнить БД тестовыми данными при старте приложения
    /// </summary>
    public static async Task<IApplicationBuilder> SeedDatabaseAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await DbSeeder.SeedEventsAsync(context);
        return app;
    }
}
