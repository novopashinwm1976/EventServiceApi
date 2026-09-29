using EventServiceApi.Data.DTO;
using EventServiceApi.Data.Models;

namespace EventServiceApi.Application.Interfaces
{
    /// <summary>
    /// Интерфейс по работе с CRUD операциями событий
    /// </summary>
    public interface IEventService
    {
        /// <summary>
        /// Получить все события
        /// </summary>
        /// <returns></returns>
        Task<List<Event>> GetAllAsync();

        /// <summary>
        /// Получить событие по Id
        /// </summary>
        /// <param name="Id">id события</param>
        /// <returns></returns>
        Task<Event> GetAsync(Guid Id);

        /// <summary>
        /// Добавить событие
        /// </summary>
        /// <param name="eventNew"></param>
        /// <returns></returns>
        Task AddEventAsync(Event eventNew);

        /// <summary>
        /// Обновить событие
        /// </summary>
        /// <param name="Id">id события</param>
        /// <param name="eventChange"></param>
        /// <returns></returns>
        Task UpdateEventAsync(Guid Id, EventDto eventChange);

        /// <summary>
        /// Удалить событие
        /// </summary>
        /// <param name="eventDelete"></param>
        /// <returns></returns>
        Task RemoveEventAsync(Guid eventDelete);
    }
}
