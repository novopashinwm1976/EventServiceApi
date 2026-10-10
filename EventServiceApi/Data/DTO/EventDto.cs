using System.ComponentModel.DataAnnotations;

namespace EventServiceApi.Data.DTO;

/// <summary>
/// DTO для передачи данных о событии
/// </summary>
public class EventDto : IValidatableObject
{
    /// <summary>
    /// Заголовок события
    /// </summary>
    [Required(ErrorMessage = "Заголовок события обязателен для заполнения.")]
    public string? Title { get; set; }

    /// <summary>
    /// Описание события (опционально)
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Дата начала мероприятия
    /// </summary>
    [Required(ErrorMessage = "Дата начала мероприятия обязательна для заполнения.")]
    public DateTime? StartAt { get; set; }

    /// <summary>
    /// Дата окончания мероприятия
    /// </summary>
    [Required(ErrorMessage = "Дата окончания мероприятия обязательна для заполнения.")]
    public DateTime? EndAt { get; set; }

    /// <inheritdoc />
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartAt is null || EndAt is null)
        {
            yield break;
        }

        if (StartAt.Value >= EndAt.Value)
        {
            yield return new ValidationResult(
                $"Дата начала события '{StartAt.Value:dd.MM.yyyy HH:mm}' должна быть строго раньше даты окончания '{EndAt.Value:dd.MM.yyyy HH:mm}'.",
                new[] { nameof(StartAt), nameof(EndAt) });
        }
    }
}