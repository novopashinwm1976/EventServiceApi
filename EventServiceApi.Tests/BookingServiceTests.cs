using EventServiceApi.Data;
using EventServiceApi.Data.Enum;
using EventServiceApi.Data.Models;
using EventServiceApi.Tests.TestHelpers;
using Microsoft.EntityFrameworkCore;

namespace EventServiceApi.Tests;

public class BookingServiceTests
{
    // ---------- helpers ----------

    private static Event SeedEvent(AppDbContext context)
    {
        var ev = new Event("Test Event", "", DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2));
        context.Events.Add(ev);
        context.SaveChanges();
        return ev;
    }

    // ============================================================
    //  УСПЕШНЫЕ СЦЕНАРИИ
    // ============================================================

    [Fact]
    public async Task CreateBookingAsync_ExistingEvent_ReturnsBookingWithPendingStatus()
    {
        // Arrange
        var (service, context) = BookingServiceFactory.Create();
        var ev = SeedEvent(context);

        // Act
        var booking = await service.CreateBookingAsync(ev.Id);

        // Assert
        Assert.NotNull(booking);
        Assert.NotEqual(Guid.Empty, booking.Id);
        Assert.Equal(ev.Id, booking.EventId);
        Assert.Equal(BookingStatus.Pending, booking.Status);
        Assert.NotEqual(default, booking.CreatedAt);

        // Проверяем, что запись действительно попала в БД
        var saved = await context.Bookings.FindAsync(booking.Id);
        Assert.NotNull(saved);
        Assert.Equal(BookingStatus.Pending, saved!.Status);
    }

    [Fact]
    public async Task CreateBookingAsync_MultipleBookingsForSameEvent_AllHaveUniqueIds()
    {
        // Arrange
        var (service, context) = BookingServiceFactory.Create();
        var ev = SeedEvent(context);

        // Act
        var b1 = await service.CreateBookingAsync(ev.Id);
        var b2 = await service.CreateBookingAsync(ev.Id);
        var b3 = await service.CreateBookingAsync(ev.Id);

        // Assert
        var ids = new[] { b1?.Id, b2?.Id, b3?.Id };
        Assert.Equal(3, ids.Distinct().Count());

        Assert.All(new[] { b1, b2, b3 }, b =>
        {
            Assert.Equal(ev.Id, b?.EventId);
            Assert.Equal(BookingStatus.Pending, b?.Status);
        });

        Assert.Equal(3, await context.Bookings.CountAsync<Booking>(x => x.EventId == ev.Id));
    }

    [Fact]
    public async Task GetBookingByIdAsync_ExistingBooking_ReturnsCorrectInfo()
    {
        // Arrange
        var (service, context) = BookingServiceFactory.Create();
        var ev = SeedEvent(context);
        Booking created = await service.CreateBookingAsync(ev.Id);

        // Act
        Booking fetched = await service.GetBookingByIdAsync(created.Id);

        // Assert
        Assert.NotNull(fetched);
        Assert.Equal(created.Id, fetched!.Id);
        Assert.Equal(created.EventId, fetched.EventId);
        Assert.Equal(created.Status, fetched.Status);
        Assert.Equal(created.CreatedAt, fetched.CreatedAt);
    }

    [Fact]
    public async Task GetBookingByIdAsync_AfterStatusChange_ReflectsNewStatus()
    {
        // Arrange
        var (service, context) = BookingServiceFactory.Create();
        var ev = SeedEvent(context);
        var created = await service.CreateBookingAsync(ev.Id);

        // Меняем статус в БД так же, как это делает BookingBackgroundService
        var entity = await context.Bookings.FindAsync(created.Id);
        Assert.NotNull(entity);

        entity!.Status = BookingStatus.Confirmed;
        entity.ProcessedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();

        // Act + Assert (Confirmed)
        var afterConfirm = await service.GetBookingByIdAsync(created.Id);
        Assert.NotNull(afterConfirm);
        Assert.Equal(BookingStatus.Confirmed, afterConfirm!.Status);
        Assert.NotNull(afterConfirm.ProcessedAt);

        // Меняем на Rejected
        entity.Status = BookingStatus.Rejected;
        entity.ProcessedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();

        // Act + Assert (Rejected)
        var afterReject = await service.GetBookingByIdAsync(created.Id);
        Assert.NotNull(afterReject);
        Assert.Equal(BookingStatus.Rejected, afterReject!.Status);
    }

    // ============================================================
    //  НЕУСПЕШНЫЕ СЦЕНАРИИ
    // ============================================================

    [Fact]
    public async Task CreateBookingAsync_NonExistingEvent_Throws()
    {
        // Arrange
        var (service, _) = BookingServiceFactory.Create();
        var unknownEventId = Guid.NewGuid();

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => service.CreateBookingAsync(unknownEventId));
    }

    [Fact]
    public async Task CreateBookingAsync_DeletedEvent_Throws()
    {
        // Arrange
        var (service, context) = BookingServiceFactory.Create();
        var ev = SeedEvent(context);
        var deletedId = ev.Id;

        context.Events.Remove(ev);
        await context.SaveChangesAsync();

        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => service.CreateBookingAsync(deletedId));

        // Дополнительно убеждаемся, что бронь не создана
        Assert.Equal(0, await context.Bookings.CountAsync<Booking>());
    }

    [Fact]
    public async Task GetBookingByIdAsync_NonExistingId_ReturnsNull()
    {
        var (service, _) = BookingServiceFactory.Create();
        var result = await service.GetBookingByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }
}