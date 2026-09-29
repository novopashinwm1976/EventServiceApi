using EventServiceApi.Application.Interfaces;
using EventServiceApi.Data.DTO;
using EventServiceApi.Data.Models;

using Microsoft.AspNetCore.Mvc;

using System.Net;

namespace EventServiceApi.Presentation.Controllers;

/// <summary>
/// Контроллер для работы  событиями
/// </summary>
/// <param name="_eventService"></param>
/// <param name="_logger"></param>
[ApiController]
[Route("api/[controller]")]
public class EventsController(IEventService _eventService, ILogger<EventsController> _logger) : ControllerBase
{
    /// <summary>
    /// Получаем все события
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task< ApiResult<List<Event>>> GetEvents() 
    {
        _logger.Log(LogLevel.Information, "Получаем информацию о всех событиях");
        return new ApiResult<List<Event>>
        {
            Data = await _eventService.GetAllAsync(),
            Success = true,
            StatusCode = HttpStatusCode.OK,
            Message = "Получаем информацию о всех событиях"
        };
    }

    /// <summary>
    /// Получить информацию о событии по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор события</param>
    /// <returns></returns>
    [HttpGet("{id:guid}")]
    public async Task<ApiResult<Event>> GetEventById(Guid id)
    {
        _logger.Log(LogLevel.Information, "Получаем информацию о событии {id}", id);
        var eventOne = await _eventService.GetAsync(id);
        if (eventOne == null)
        {
            return new ApiResult<Event>
            {
                Data = null,
                Success = false,
                StatusCode = HttpStatusCode.NotFound,
                Message = "Событие не найдено"
            };
        }

        return new ApiResult<Event>
        {
            Data = eventOne,
            Success = true,
            StatusCode = HttpStatusCode.OK,
            Message = "Получаем информацию о событии"
        };
    }

    /// <summary>
    /// Создание события
    /// </summary>
    /// <param name="eventDto">Входящий запрос</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ApiResult<Event>> CreateEvent([FromBody] EventDto eventDto) 
    {
        _logger.Log(LogLevel.Information, "Создание события {eventDto}", eventDto);

        if (eventDto.StartAt > eventDto.EndAt) 
        {
            return new ApiResult<Event>
            {
                Data = null,
                Success = false,
                StatusCode = HttpStatusCode.BadRequest,
                Message = $"Дата начала события '{eventDto.StartAt:dd.MM.yyyy HH:mm}' больше даты окончания события {eventDto.EndAt:dd.MM.yyyy HH:mm}"
            };
        }
        
        var eventNew = new Event(eventDto.Title, eventDto.Description, eventDto.StartAt, eventDto.EndAt);
        await _eventService.AddEventAsync(eventNew);
        
        return new ApiResult<Event>
        { 
            Data = eventNew,
            Success = true,
            StatusCode= HttpStatusCode.OK,
            Message = "Создан объект событие"
        };
    }

    /// <summary>
    /// Изменение события по идентификатору
    /// </summary>
    /// <param name="id">Идентификтор события</param>
    /// <param name="eventDto">Измененные данные события</param>
    /// <returns></returns>
    [HttpPut("{id:guid}")]
    public async Task<ApiResult<EventDto>> UpdateEvent(Guid id, [FromBody] EventDto eventDto)
    {
        _logger.Log(LogLevel.Information, "Обновление события {id}", id);

        var eventOne = await _eventService.GetAsync(id);
        if (eventOne == null)
        {
            return new ApiResult<EventDto>
            {
                Data = null,
                Success = false,
                StatusCode = HttpStatusCode.NotFound,
                Message = "Событие не найдено"
            };
        }

        if (eventDto.StartAt > eventDto.EndAt)
        {
            return new ApiResult<EventDto>
            {
                Data = null,
                Success = false,
                StatusCode = HttpStatusCode.BadRequest,
                Message = $"Дата начала события '{eventDto.StartAt:dd.MM.yyyy HH:mm}' больше даты окончания события {eventDto.EndAt:dd.MM.yyyy HH:mm}"
            };
        }

        await _eventService.UpdateEventAsync(id, eventDto);

        return new ApiResult<EventDto>
        {
            Data = eventDto,
            Success = true,
            StatusCode = HttpStatusCode.OK,
            Message = "Событие обновлено"
        };
    }

    /// <summary>
    /// Удаление события по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор события</param>
    /// <returns></returns>
    [HttpDelete("{id:guid}")]
    public async Task<ApiResult<Event>> DeleteEvent(Guid id)
    {
        _logger.Log(LogLevel.Information, "Удаление события {id}", id);
        var eventOne = await _eventService.GetAsync(id);
        if (eventOne == null)
        {
            return new ApiResult<Event>
            {
                Data = eventOne,
                Success = false,
                StatusCode = HttpStatusCode.NotFound,
                Message = "Событие не найдено"
            };
        }
        await _eventService.RemoveEventAsync(id);
        return new ApiResult<Event>
        {
            Data = eventOne,
            Success = true,
            StatusCode = HttpStatusCode.OK,
            Message = "Событие удалено"
        };
    }
}
