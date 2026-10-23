using EventServiceApi.Data;
using EventServiceApi.Data.Enum;

using Microsoft.EntityFrameworkCore;

namespace EventServiceApi.Application.Services;

/// <summary>
/// Фоновый процесс для бронирования
/// </summary>
/// <param name="context">Сервис хранения данных по бронированию</param>
/// <param name="logger">Логгер класса</param>
public class BookingBackgroundService(AppDbContext context,
                                ILogger<BookingBackgroundService> logger) : BackgroundService
{
    private readonly AppDbContext _context = context;
    private readonly ILogger<BookingBackgroundService> _logger = logger;

    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(2);
    private static readonly TimeSpan ProcessingDelay = TimeSpan.FromSeconds(2);

    /// <summary>
    /// Выполнение бронирования
    /// </summary>
    /// <param name="stoppingToken">токен отмены</param>
    /// <returns></returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var pending = _context.Bookings
                                      .Where(b => b.Status == BookingStatus.Pending)
                                      .ToList();

                foreach (var booking in pending)
                {
                    await Task.Delay(ProcessingDelay, stoppingToken); // «внешняя система»

                    booking.Status = BookingStatus.Confirmed;
                    booking.ProcessedAt = DateTime.UtcNow;
                    
                    _context.Bookings.Update(booking);
                    await _context.SaveChangesAsync();
                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogInformation("Booking {BookingId} -> Confirmed", booking.Id);
                    }
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Background processing error");
            }

            await Task.Delay(PollInterval, stoppingToken);
        }
    }
}