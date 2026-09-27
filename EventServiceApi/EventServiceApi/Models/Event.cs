namespace EventServiceApi.Models;

/// <summary>
/// Класс содержащий информацию о событии
/// </summary>
public class Event
{
    /// <summary>
    /// Идентификатор события
    /// </summary>
    public Guid Id { get; private set; }
    
    /// <summary>
    /// Заголовок события
    /// </summary>
    public string Title { get; private set; }
    
    /// <summary>
    /// Описание события
    /// </summary>
    public string Description { get; private set; }
    
    /// <summary>
    /// Дата и время начала события 
    /// </summary>
    public DateTime StartAt { get; private set; }
    
    /// <summary>
    /// Дата и время окончания события
    /// </summary>
    public DateTime EndAt { get; private set; }

    public Event(string title, string description, DateTime startAt, DateTime endAt) 
    {
        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        StartAt = startAt;
        EndAt = endAt;
    }
}
