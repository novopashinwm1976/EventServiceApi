using EventServiceApi.Data;
using EventServiceApi.Data.Enum;

using Microsoft.EntityFrameworkCore;

namespace EventServiceApi.Application.Services;

/// <summary>
/// Фоновый процесс для бронирования
/// </summary>
/// <param name="scopeFactory">фабричный метод вызова scoped процедур</param>
/// <param name="logger">Логгер</param>
public class BookingBackgroundService(
    IServiceScopeFactory scopeFactory,
    ILogger<BookingBackgroundService> logger) : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly ILogger<BookingBackgroundService> _logger = logger;

    private readonly TimeSpan PollInterval = TimeSpan.FromSeconds(2);

    /// <summary>
    /// Выполняет фоновую задачу, которая периодически проверяет наличие бронирований со статусом Pending и обновляет их статус на Confirmed.
    /// </summary>
    /// <param name="stoppingToken">Токен отмены</param>
    /// <returns></returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                // ... вся работа с context внутри scope
                var pendingBookings = await context.Bookings
                    .Where(b => b.Status == BookingStatus.Pending)
                    .ToListAsync(stoppingToken);

                foreach (var booking in pendingBookings)
                {
                    await Task.Delay(2000, stoppingToken); // имитация внешнего вызова
                    booking.Status = BookingStatus.Confirmed;
                    booking.ProcessedAt = DateTime.UtcNow;
                }

                await context.SaveChangesAsync(stoppingToken);
            }

            await Task.Delay(PollInterval, stoppingToken);
        }
    }
}