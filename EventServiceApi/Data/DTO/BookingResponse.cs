namespace EventServiceApi.Data.DTO;

/// <summary>
/// Структура для возврата данных по бронированию
/// </summary>
/// <param name="Id">id</param>
/// <param name="EventId">id события</param>
/// <param name="Status">Статус бронирования</param>
/// <param name="CreatedAt">Дата создания</param>
public record BookingResponse(
    Guid Id,
    Guid EventId,
    string Status,
    DateTime CreatedAt);
