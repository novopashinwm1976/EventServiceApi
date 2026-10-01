namespace EventServiceApi.Data.Models;

/// <summary>
/// Класс события
/// </summary>
/// <param name="title">Заголовок события</param>
/// <param name="description">Описание события</param>
/// <param name="startAt">Дата и время начала события</param>
/// <param name="endAt">Дата и время окончания события</param>
public class Event(string title, string description, DateTime? startAt, DateTime? endAt)
{
    /// <summary>
    /// Идентификатор события
    /// </summary>
    public Guid Id { get; private set; } = Guid.NewGuid();

    /// <summary>
    /// Заголовок события
    /// </summary>
    public string Title { get; set; } = title;

    /// <summary>
    /// Описание события
    /// </summary>
    public string Description { get; set; } = description;

    /// <summary>
    /// Дата и время начала события 
    /// </summary>
    public DateTime? StartAt { get; set; } = startAt;

    /// <summary>
    /// Дата и время окончания события
    /// </summary>
    public DateTime? EndAt { get; set; } = endAt;
}
