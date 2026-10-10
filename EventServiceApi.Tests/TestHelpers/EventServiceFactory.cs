using EventServiceApi.Application.Services;
using EventServiceApi.Data;

using Microsoft.Extensions.Logging.Abstractions;

namespace EventServiceApi.Tests.TestHelpers;

public static class EventServiceFactory
{
    public static (EventService Service, AppDbContext Context) Create()
    {
        var context = InMemoryDbContextFactory.Create();
        var logger = NullLogger<EventService>.Instance;
        var service = new EventService(context, logger);
        return (service, context);
    }
}