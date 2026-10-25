using EventServiceApi.Application.Services;
using EventServiceApi.Data;

using Microsoft.Extensions.Logging.Abstractions;

namespace EventServiceApi.Tests.TestHelpers;

public class BookingServiceFactory
{
    /// <summary>
    /// Создаёт BookingService поверх изолированной in-memory БД.
    /// </summary>
    public static (BookingService Service, AppDbContext Context) Create()
    {
        var context = InMemoryDbContextFactory.Create();
        var logger = NullLogger<BookingService>.Instance;
        var service = new BookingService(context, logger);

        return new (service, context);
    }
}