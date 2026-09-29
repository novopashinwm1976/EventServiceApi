using EventServiceApi.Data;

using Microsoft.EntityFrameworkCore;

namespace EventServiceApi.Infrastruture;

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
}
