using EventServiceApi.Application.Interfaces;
using EventServiceApi.Data.DTO;

using Microsoft.AspNetCore.Mvc;

namespace EventServiceApi.Presentation.Controllers;

/// <summary>
/// Контроллер для работы с событиями
/// </summary>
[ApiController]
[Route("events")]
public class EventsController(IEventService eventService, ILogger<EventsController> logger) : ControllerBase
{
    /// <summary>
    /// Получаем все события
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EventResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<EventResponse>>> GetEvents()
    {
        logger.LogInformation("Получаем информацию о всех событиях");
        var events = await eventService.GetAllAsync();
        return Ok(events.Select(ToResponse));
    }

    /// <summary>
    /// Получить информацию о событии по идентификатору
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EventResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EventResponse>> GetEventById(Guid id)
    {
        logger.LogInformation("Получаем информацию о событии {Id}", id);
        var eventOne = await eventService.GetAsync(id);
        if (eventOne is null)
        {
            return NotFoundProblem(id);
        }

        return Ok(ToResponse(eventOne));
    }

    /// <summary>
    /// Создание события
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(EventResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EventResponse>> CreateEvent([FromBody] EventDto eventDto)
    {
        logger.LogInformation("Создание события {Title}", eventDto.Title);

        var created = await eventService.AddEventAsync(eventDto);
        var response = ToResponse(created);

        return CreatedAtAction(nameof(GetEventById), new { id = response.Id }, response);
    }

    /// <summary>
    /// Изменение события по идентификатору
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(EventResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EventResponse>> UpdateEvent(Guid id, [FromBody] EventDto eventDto)
    {
        logger.LogInformation("Обновление события {Id}", id);

        var updated = await eventService.UpdateEventAsync(id, eventDto);
        if (updated is null)
        {
            return NotFoundProblem(id);
        }

        return Ok(ToResponse(updated));
    }

    /// <summary>
    /// Удаление события по идентификатору
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteEvent(Guid id)
    {
        logger.LogInformation("Удаление события {Id}", id);

        var removed = await eventService.RemoveEventAsync(id);
        if (!removed)
        {
            return NotFoundProblem(id);
        }

        return NoContent();
    }

    private ObjectResult NotFoundProblem(Guid id) => NotFound(new ProblemDetails
    {
        Status = StatusCodes.Status404NotFound,
        Title = "Событие не найдено",
        Detail = $"Событие с идентификатором {id} не найдено."
    });

    private static EventResponse ToResponse(Data.Models.Event e) => new()
    {
        Id = e.Id,
        Title = e.Title,
        Description = e.Description,
        StartAt = e.StartAt,
        EndAt = e.EndAt
    };
}