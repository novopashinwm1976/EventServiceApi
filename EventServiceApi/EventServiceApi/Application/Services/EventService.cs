using EventServiceApi.Application.Interfaces;
using EventServiceApi.Data;
using EventServiceApi.Data.DTO;
using EventServiceApi.Data.Models;

using Microsoft.EntityFrameworkCore;

namespace EventServiceApi.Application.Services;

/// <summary>
/// Класс CRUD оперций событий
/// </summary>
public class EventService(AppDbContext context, ILogger<EventService> logger) : IEventService
{
    private readonly AppDbContext _context = context;
    private readonly ILogger<EventService> _logger = logger;

    /// <summary>
    /// Добавление события
    /// </summary>
    /// <param name="eventNew"></param>
    /// <returns></returns>
    public async Task AddEventAsync(Event eventNew)
    {
        _context.Events.Add(eventNew);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Обновление события
    /// </summary>
    /// <param name="Id">Идентификатор события</param>
    /// <param name="eventChange">Измененные данные события</param>
    /// <returns></returns>
    public async Task UpdateEventAsync(Guid Id, EventDto eventChange)
    {
        var entity = _context.Events.FirstOrDefault(x => x.Id == Id);
        if (entity != null)
        {
            entity.Description = eventChange.Description;
            entity.Title = eventChange.Title;
            entity.StartAt = eventChange.StartAt;
            entity.EndAt = eventChange.EndAt;
            _context.Events.Update(entity);
        }
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Получение события по идентификатору
    /// </summary>
    /// <param name="Id">Идентификатор события</param>
    /// <returns></returns>
    public Task<Event> GetAsync(Guid Id)
    {
        return _context.Events.FirstAsync(x => x.Id == Id);       
    }

    /// <summary>
    /// Получение всех событий
    /// </summary>
    /// <returns></returns>
    public Task<List<Event>> GetAllAsync()
    {
        return _context.Events.ToListAsync();
    }

    /// <summary>
    /// Удаление события по идентификатору
    /// </summary>
    /// <param name="deleteId">Идентификатор события</param>
    /// <returns></returns>
    public async Task RemoveEventAsync(Guid deleteId)
    {
        var indexToRemove = await _context.Events.FirstAsync(e => e.Id == deleteId);
        _context.Events.Remove(indexToRemove);
        await _context.SaveChangesAsync();
    }
}
