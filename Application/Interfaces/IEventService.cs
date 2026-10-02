using EventServiceApi.Data.DTO;
using EventServiceApi.Data.Models;

namespace EventServiceApi.Application.Interfaces;

/// <summary>
/// Интерфейс по работе с CRUD операциями событий
/// </summary>
public interface IEventService
{
    /// <summary>
    /// Получить все события
    /// </summary>
    Task<List<Event>> GetAllAsync();

    /// <summary>
    /// Получить событие по Id
    /// </summary>
    Task<Event?> GetAsync(Guid id);

    /// <summary>
    /// Добавить событие
    /// </summary>
    Task<Event> AddEventAsync(EventDto eventNew);

    /// <summary>
    /// Обновить событие
    /// </summary>
    /// <returns>Обновлённое событие, либо null, если запись не найдена</returns>
    Task<Event?> UpdateEventAsync(Guid id, EventDto eventChange);

    /// <summary>
    /// Удалить событие
    /// </summary>
    /// <returns>true, если запись найдена и удалена; иначе false</returns>
    Task<bool> RemoveEventAsync(Guid id);
}