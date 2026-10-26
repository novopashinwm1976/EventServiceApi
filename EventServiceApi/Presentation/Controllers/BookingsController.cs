using EventServiceApi.Application.Interfaces;
using EventServiceApi.Data.DTO;
using EventServiceApi.Data.Models;

using Microsoft.AspNetCore.Mvc;

namespace EventServiceApi.Presentation.Controllers;

/// <summary>
/// Контроллер бронирования
/// </summary>
/// <summary>
/// Контроллер для управления бронированиями событий.
/// </summary>
[ApiController]
[Route("")]
public class BookingsController(IBookingService _bookingService) : ControllerBase
{

    /// <summary>
    /// Создает новое бронирование для события.
    /// </summary>
    /// <param name="id">Идентификатор события.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Информация о бронировании или NotFound, если событие не найдено.</returns>
    [HttpPost("events/{id:guid}/book")]
    [ProducesResponseType(typeof(Booking), StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateBooking(Guid id, CancellationToken ct)
    {
        var booking = await _bookingService.CreateBookingAsync(id, ct);
        if (booking is null) return NotFound();

        // AcceptedAtAction: 202 + Location: /bookings/{bookingId}
        return AcceptedAtAction(nameof(GetBooking), new { id = booking.Id }, booking);
    }

    /// <summary>
    /// Получает информацию о бронировании по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор бронирования.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Информация о бронировании или NotFound, если не найдено.</returns>
    [HttpGet("bookings/{id:guid}")]
    [ProducesResponseType(typeof(Booking), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBooking(Guid id, CancellationToken ct)
    {
        var booking = await _bookingService.GetBookingByIdAsync(id, ct);
        return booking is null ? NotFound() : Ok(booking);
    }
}