using EventServiceApi.Application.Interfaces;
using EventServiceApi.Application.Services;

namespace EventServiceApi.Application;

/// <summary>
/// Extension метод для регистрации сервисов приложения в контейнере зависимостей
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Регистрация сервисов приложения в контейнере зависимостей
    /// </summary>
    /// <param name="services">Контейнер зависимостей</param>
    /// <returns></returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IEventService, EventService>();

        return services;
    }
}
