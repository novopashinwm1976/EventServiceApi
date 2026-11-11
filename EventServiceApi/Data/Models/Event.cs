namespace EventServiceApi.Data.Models;

/// <summary>
/// Класс события
/// </summary>
/// <param name="title">Заголовок события</param>
/// <param name="description">Описание события</param>
/// <param name="startAt">Дата и время начала события</param>
/// <param name="endAt">Дата и время окончания события</param>
public class Event(string title, string? description, DateTime? startAt, DateTime? endAt)
{
    private int _avaibleSeats;

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
    public string? Description { get; set; } = description;

    /// <summary>
    /// Дата и время начала события 
    /// </summary>
    public DateTime? StartAt { get; set; } = startAt;

    /// <summary>
    /// Дата и время окончания события
    /// </summary>
    public DateTime? EndAt { get; set; } = endAt;

    /// <summary>
    ///  общее количество мест на событии
    /// </summary>
    public int TotalSeats { get; set; }

    /// <summary>
    /// текущее количество свободных мест; при создании равно
    /// </summary>
    public int AvailableSeats { 
        get=> _avaibleSeats;
        set 
        {
            if (value > 0) 
            {
                _avaibleSeats = value;
            }
        } 
    }

    public bool TryReserveSeats(int count = 1)
    {
        if (count <= 0) throw new ArgumentOutOfRangeException(nameof(count));
        if (_avaibleSeats < count) return false;

        Interlocked.Add(ref _avaibleSeats, count);
        return true;
    }
}