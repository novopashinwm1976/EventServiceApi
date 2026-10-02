namespace EventServiceApi.Data.DTO;

/// <summary>
/// DTO для ответа с данными о событии
/// </summary>
public class EventResponse
{
    /// <summary>
    /// Идентификатор события
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Заголовок события
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Описание события
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Дата и время начала события
    /// </summary>
    public DateTime? StartAt { get; set; }

    /// <summary>
    /// Дата и время окончания события
    /// </summary>
    public DateTime? EndAt { get; set; }
}