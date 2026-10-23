using EventServiceApi.Data.DTO;
using EventServiceApi.Data.Models;

namespace EventServiceApi.Application.Interfaces;

/// <summary>
/// Интерфейс сервиса бронирования
/// </summary>
public interface IBookingService
{
    /// <summary>
    /// Создает новое бронирование для указанного события асинхронно.   
    /// </summary>
    /// <param name="eventId">Идентификатор события, для которого создается бронирование.</param>
    /// <param name="ct">Токен отмены для прерывания операции.</param>
    /// <returns>Задача, представляющая асинхронную операцию, возвращающая информацию о бронировании или null, если бронирование
    /// не удалось.</returns>
    Task<Booking?> CreateBookingAsync(Guid eventId, CancellationToken ct = default);

    /// <summary>
    /// Получает информацию о бронировании по его идентификатору асинхронно.
    /// </summary>
    /// <param name="bookingId">Идентификатор бронирования.</param>
    /// <param name="ct">Токен отмены для прерывания операции.</param>
    /// <returns>Задача, представляющая асинхронную операцию, возвращающая информацию о бронировании или null, если бронирование не найдено.</returns>
    Task<Booking?> GetBookingByIdAsync(Guid bookingId, CancellationToken ct = default);
}