using System.ComponentModel.DataAnnotations;

namespace EventServiceApi.Data.DTO;

/// <summary>
/// DTO с параметрами фильтрации и пагинации событий.
/// Все параметры опциональны. Фильтры комбинируются по логическому И.
/// </summary>
public class EventFilterDto : IValidatableObject
{
    /// <summary>
    /// Поиск по названию (регистронезависимый, частичное совпадение).
    /// </summary>
    /// <example>конференция</example>
    public string? Title { get; set; }

    /// <summary>
    /// События, которые начинаются не раньше указанной даты (включительно).
    /// </summary>
    /// <example>2026-05-01T00:00:00Z</example>
    public DateTime? From { get; set; }

    /// <summary>
    /// События, которые заканчиваются не позже указанной даты (включительно).
    /// </summary>
    /// <example>2026-06-01T00:00:00Z</example>
    public DateTime? To { get; set; }

    /// <summary>
    /// Номер страницы (начиная с 1). По умолчанию 1.
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Параметр 'page' должен быть больше или равен 1.")]
    public int Page { get; set; } = 1;

    /// <summary>
    /// Количество элементов на странице. По умолчанию 10. Максимум 100.
    /// </summary>
    [Range(1, 100, ErrorMessage = "Параметр 'pageSize' должен быть в диапазоне от 1 до 100.")]
    public int PageSize { get; set; } = 10;

    /// <inheritdoc />
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (From is not null && To is not null && From.Value > To.Value)
        {
            yield return new ValidationResult(
                $"Параметр 'from' ({From.Value:dd.MM.yyyy HH:mm}) не может быть больше параметра 'to' ({To.Value:dd.MM.yyyy HH:mm}).",
                new[] { nameof(From), nameof(To) });
        }
    }
}