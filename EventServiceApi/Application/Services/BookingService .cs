using EventServiceApi.Application.Interfaces;
using EventServiceApi.Data;
using EventServiceApi.Data.Enum;
using EventServiceApi.Data.Models;

using Microsoft.EntityFrameworkCore;

namespace EventServiceApi.Application.Services;

/// <summary>
/// Предоставляет операции для создания и получения бронирований событий.
/// </summary>
/// <remarks>
/// Конструктор создания сервиса бронирования
/// </remarks>
/// <param name="context">Хранилище для бронирований</param>
/// <param name="logger">Логгер</param>
public class BookingService(AppDbContext context, ILogger<BookingService> logger) : IBookingService
{
    private readonly AppDbContext _context = context;
    private readonly ILogger<BookingService> _logger = logger;

    /// <summary>
    /// Создание бронирования для события
    /// </summary>
    /// <param name="eventId">Id события</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns></returns>
    public async Task<Booking?> CreateBookingAsync(Guid eventId, CancellationToken ct = default)
    {

        // Удалённое событие = отсутствует в хранилище
        if (await _context.Events.FirstOrDefaultAsync(x => x.Id == eventId,ct) is null)
            return null;

        var entity = new Booking
        {
            EventId = eventId,
            Status = BookingStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            ProcessedAt = null
        };

        _context.Bookings.Add(entity);
        await _context.SaveChangesAsync(ct);
        _logger.LogInformation("Создано событие {Id}", entity.Id);
        return entity;
    }

    /// <summary>
    /// Получение данных по бронирования
    /// </summary>
    /// <param name="bookingId">Id бронирования</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns></returns>
    public async Task<Booking?> GetBookingByIdAsync(Guid bookingId, CancellationToken ct = default)
    {
        return await _context.Bookings.FirstOrDefaultAsync(x => x.Id == bookingId, ct);
    }
}