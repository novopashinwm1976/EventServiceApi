using EventServiceApi.Application.Services;

using Microsoft.Extensions.Logging.Abstractions;

namespace EventServiceApi.Tests.TestHelpers;

public static class EventServiceFactory
{
    public static EventService Create()
    {
        var context = InMemoryDbContextFactory.Create();
        var logger = NullLogger<EventService>.Instance;
        return new EventService(context, logger);
    }
}