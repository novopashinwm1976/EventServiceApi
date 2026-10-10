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
    public async Task<PaginatedResult<Event>> GetAllAsync(EventFilterDto filter)
    {
        IQueryable<Event> query = _context.Events;

        // Фильтр по названию: регистронезависимый, частичное совпадение
        if (!string.IsNullOrWhiteSpace(filter.Title))
        {
            var title = filter.Title.Trim().ToLower();
            query = query.Where(e => e.Title.ToLower().Contains(title));
        }

        // Фильтр: событие начинается не раньше указанной даты
        if (filter.From is not null)
        {
            query = query.Where(e => e.StartAt != null && e.StartAt >= filter.From);
        }

        // Фильтр: событие заканчивается не позже указанной даты
        if (filter.To is not null)
        {
            query = query.Where(e => e.EndAt != null && e.EndAt <= filter.To);
        }

        // Общее количество записей под фильтром (до пагинации)
        var totalCount = await query.CountAsync();

        // Сортировка обязательна перед Skip/Take — иначе страницы нестабильны
        var items = await query
            .OrderBy(e => e.StartAt)
            .ThenBy(e => e.Id)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        var totalPages = totalCount == 0
            ? 0
            : (int)Math.Ceiling(totalCount / (double)filter.PageSize);

        _logger.LogInformation(
            "Постраничная выборка: Page={Page}, PageSize={PageSize}, TotalCount={TotalCount}, TotalPages={TotalPages}",
            filter.Page, filter.PageSize, totalCount, totalPages);

        return new PaginatedResult<Event>
        {
            TotalCount = totalCount,
            Items = items,
            Page = filter.Page,
            PageSize = filter.PageSize,
            TotalPages = totalPages
        };
    }

    /// <inheritdoc />
    public async Task<Event> AddEventAsync(EventDto eventNew)
    {
        Validate(eventNew);

        var entity = new Event(eventNew.Title!, eventNew.Description, eventNew.StartAt, eventNew.EndAt);

        _context.Events.Add(entity);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Создано событие {Id}", entity.Id);
        return entity;
    }

    /// <inheritdoc />
    public async Task<Event?> UpdateEventAsync(Guid id, EventDto eventChange)
    {
        Validate(eventChange);

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

    /// <summary>
    /// Проверка инвариантов события.
    /// Выбрасывает <see cref="ArgumentException"/> — middleware превратит его в 400 Bad Request.
    /// </summary>
    private static void Validate(EventDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
        {
            throw new ArgumentException("Заголовок события обязателен для заполнения.", nameof(dto.Title));
        }

        if (dto.StartAt is null)
        {
            throw new ArgumentException("Дата начала мероприятия обязательна для заполнения.", nameof(dto.StartAt));
        }

        if (dto.EndAt is null)
        {
            throw new ArgumentException("Дата окончания мероприятия обязательна для заполнения.", nameof(dto.EndAt));
        }

        if (dto.StartAt.Value >= dto.EndAt.Value)
        {
            throw new ArgumentException(
                $"Дата начала события '{dto.StartAt.Value:dd.MM.yyyy HH:mm}' должна быть строго раньше даты окончания '{dto.EndAt.Value:dd.MM.yyyy HH:mm}'.",
                nameof(dto.StartAt));
        }
    }
}