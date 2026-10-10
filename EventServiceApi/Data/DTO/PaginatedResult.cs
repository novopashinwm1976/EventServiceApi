namespace EventServiceApi.Data.DTO;

/// <summary>
/// Универсальный результат постраничной выборки.
/// </summary>
/// <typeparam name="T">Тип элементов страницы</typeparam>
public class PaginatedResult<T>
{
    /// <summary>
    /// Общее количество записей, удовлетворяющих фильтрам (без учёта пагинации).
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Элементы текущей страницы.
    /// </summary>
    public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();

    /// <summary>
    /// Номер текущей страницы (начиная с 1).
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Количество элементов на текущей странице.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Общее количество страниц при текущем <see cref="PageSize"/>.
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Признак наличия предыдущей страницы.
    /// </summary>
    public bool HasPreviousPage => Page > 1;

    /// <summary>
    /// Признак наличия следующей страницы.
    /// </summary>
    public bool HasNextPage => Page < TotalPages;
}