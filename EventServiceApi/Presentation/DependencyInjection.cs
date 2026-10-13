using System.Reflection;

namespace EventServiceApi.Presentation;

/// <summary>
/// Extension метод для регистрации сервисов презентационного слоя в контейнере зависимостей
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Регистрация сервисов презентационного слоя в контейнере зависимостей
    /// </summary>
    /// <param name="services">Контейнер зависимостей</param>
    /// <returns></returns>
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddSwaggerGen(options =>
        {
            // Путь к XML-файлу с документацией
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);
        });

        return services;
    }
}
