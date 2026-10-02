using EventServiceApi.Application.Interfaces;
using EventServiceApi.Data;
using EventServiceApi.Data.DTO;
using EventServiceApi.Data.Models;

using Microsoft.EntityFrameworkCore;

namespace EventServiceApi.Application.Services;

/// <summary>
/// Класс CRUD операций событий
/// </summary>
public class EventService(AppDbContext context, ILogger<EventService> logger) : IEventService
{
    private readonly AppDbContext _context = context;
    private readonly ILogger<EventService> _logger = logger;

    /// <inheritdoc />
    public async Task<Event> AddEventAsync(EventDto eventNew)
    {
        var entity = new Event(eventNew.Title!, eventNew.Description, eventNew.StartAt, eventNew.EndAt);

        _context.Events.Add(entity);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Создано событие {Id}", entity.Id);
        return entity;
    }

    /// <inheritdoc />
    public async Task<Event?> UpdateEventAsync(Guid id, EventDto eventChange)
    {
        var entity = await _context.Events.FirstOrDefaultAsync(x => x.Id == id);
        if (entity is null)
        {
            _logger.LogWarning("Событие {Id} не найдено для обновления", id);
            return null;
        }

        entity.Description = eventChange.Description;
        entity.Title = eventChange.Title!;
        entity.StartAt = eventChange.StartAt;
        entity.EndAt = eventChange.EndAt;

        await _context.SaveChangesAsync();
        return entity;
    }

    /// <inheritdoc />
    public Task<Event?> GetAsync(Guid id)
    {
        return _context.Events.FirstOrDefaultAsync(x => x.Id == id);
    }

    /// <inheritdoc />
    public Task<List<Event>> GetAllAsync()
    {
        return _context.Events
            .OrderBy(e => e.StartAt)
            .ThenBy(e => e.Id)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<bool> RemoveEventAsync(Guid id)
    {
        var entity = await _context.Events.FirstOrDefaultAsync(e => e.Id == id);
        if (entity is null)
        {
            _logger.LogWarning("Событие {Id} не найдено для удаления", id);
            return false;
        }

        _context.Events.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}