using EventServiceApi.Data.DTO;
using EventServiceApi.Tests.TestHelpers;

namespace EventServiceApi.Tests;

public class EventServiceTests
{
    // ---------------------------------------------------------------------
    // Вспомогательные методы
    // ---------------------------------------------------------------------

    private static EventDto MakeDto(
        string title = "Тестовое событие",
        string? description = "Описание",
        DateTime? startAt = null,
        DateTime? endAt = null)
    {
        var start = startAt ?? new DateTime(2026, 5, 1, 10, 0, 0, DateTimeKind.Utc);
        var end = endAt ?? start.AddHours(2);

        return new EventDto
        {
            Title = title,
            Description = description,
            StartAt = start,
            EndAt = end
        };
    }

    private static EventFilterDto MakeFilter(
        string? title = null,
        DateTime? from = null,
        DateTime? to = null,
        int page = 1,
        int pageSize = 10)
        => new()
        {
            Title = title,
            From = from,
            To = to,
            Page = page,
            PageSize = pageSize
        };

    // =====================================================================
    // УСПЕШНЫЕ СЦЕНАРИИ
    // =====================================================================

    [Fact(DisplayName = "AddEventAsync: создаёт событие и возвращает его с Id")]
    public async Task AddEventAsync_CreatesEvent()
    {
        // Arrange
        var (service, _) = EventServiceFactory.Create();
        var dto = MakeDto(title: "Новое событие");

        // Act
        var created = await service.AddEventAsync(dto);

        // Assert
        Assert.NotNull(created);
        Assert.NotEqual(Guid.Empty, created.Id);
        Assert.Equal("Новое событие", created.Title);
        Assert.Equal(dto.StartAt, created.StartAt);
        Assert.Equal(dto.EndAt, created.EndAt);
    }

    [Fact(DisplayName = "GetAllAsync: возвращает все события, отсортированные по StartAt")]
    public async Task GetAllAsync_ReturnsAllEvents()
    {
        // Arrange
        var (service, _) = EventServiceFactory.Create();
        await service.AddEventAsync(MakeDto("A", startAt: new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc)));
        await service.AddEventAsync(MakeDto("B", startAt: new DateTime(2026, 5, 1, 10, 0, 0, DateTimeKind.Utc)));
        await service.AddEventAsync(MakeDto("C", startAt: new DateTime(2026, 7, 1, 10, 0, 0, DateTimeKind.Utc)));

        // Act
        var page = await service.GetAllAsync(MakeFilter(pageSize: 100));

        // Assert
        Assert.Equal(3, page.TotalCount);
        Assert.Equal(3, page.Items.Count);
        Assert.Equal(new[] { "B", "A", "C" }, page.Items.Select(e => e.Title).ToArray());
    }

    [Fact(DisplayName = "GetAsync: возвращает событие по существующему Id")]
    public async Task GetAsync_ReturnsEvent_WhenExists()
    {
        // Arrange
        var (service, _) = EventServiceFactory.Create();
        var created = await service.AddEventAsync(MakeDto("Найти меня"));

        // Act
        var found = await service.GetAsync(created.Id);

        // Assert
        Assert.NotNull(found);
        Assert.Equal(created.Id, found!.Id);
        Assert.Equal("Найти меня", found.Title);
    }

    [Fact(DisplayName = "UpdateEventAsync: обновляет существующее событие")]
    public async Task UpdateEventAsync_UpdatesExisting()
    {
        // Arrange
        var (service, _) = EventServiceFactory.Create();
        var created = await service.AddEventAsync(MakeDto("Старое"));

        var newStart = new DateTime(2026, 8, 1, 10, 0, 0, DateTimeKind.Utc);
        var newEnd = newStart.AddHours(3);
        var updateDto = MakeDto("Новое", "Новое описание", newStart, newEnd);

        // Act
        var updated = await service.UpdateEventAsync(created.Id, updateDto);

        // Assert
        Assert.NotNull(updated);
        Assert.Equal("Новое", updated!.Title);
        Assert.Equal("Новое описание", updated.Description);
        Assert.Equal(newStart, updated.StartAt);
        Assert.Equal(newEnd, updated.EndAt);
    }

    [Fact(DisplayName = "RemoveEventAsync: удаляет существующее событие")]
    public async Task RemoveEventAsync_RemovesExisting()
    {
        // Arrange
        var (service, _) = EventServiceFactory.Create();
        var created = await service.AddEventAsync(MakeDto("Удалить меня"));

        // Act
        var result = await service.RemoveEventAsync(created.Id);

        // Assert
        Assert.True(result);
        var found = await service.GetAsync(created.Id);
        Assert.Null(found);
    }

    [Fact(DisplayName = "GetAllAsync: фильтрует по названию (регистронезависимо, частично)")]
    public async Task GetAllAsync_FiltersByTitle()
    {
        // Arrange
        var (service, _) = EventServiceFactory.Create();
        await service.AddEventAsync(MakeDto("Ежегодная конференция DevCon"));
        await service.AddEventAsync(MakeDto("Воркшоп по C#"));
        await service.AddEventAsync(MakeDto("КОНФЕРЕНЦИЯ по кибербезопасности"));

        // Act
        var page = await service.GetAllAsync(MakeFilter(title: "конференция"));

        // Assert
        Assert.Equal(2, page.TotalCount);
        Assert.All(page.Items, e => Assert.Contains("онференц", e.Title.ToLower()));
    }

    [Fact(DisplayName = "GetAllAsync: фильтрует по датам (from/to)")]
    public async Task GetAllAsync_FiltersByDates()
    {
        // Arrange
        var (service, _) = EventServiceFactory.Create();
        await service.AddEventAsync(MakeDto("Раньше",
            startAt: new DateTime(2026, 1, 1, 10, 0, 0, DateTimeKind.Utc),
            endAt: new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc)));
        await service.AddEventAsync(MakeDto("В диапазоне",
            startAt: new DateTime(2026, 5, 1, 10, 0, 0, DateTimeKind.Utc),
            endAt: new DateTime(2026, 5, 1, 12, 0, 0, DateTimeKind.Utc)));
        await service.AddEventAsync(MakeDto("Позже",
            startAt: new DateTime(2026, 12, 1, 10, 0, 0, DateTimeKind.Utc),
            endAt: new DateTime(2026, 12, 1, 12, 0, 0, DateTimeKind.Utc)));

        var from = new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Utc);
        var to = new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc);

        // Act
        var page = await service.GetAllAsync(MakeFilter(from: from, to: to));

        // Assert
        Assert.Equal(1, page.TotalCount);
        Assert.Equal("В диапазоне", page.Items[0].Title);
    }

    [Fact(DisplayName = "GetAllAsync: пагинация возвращает корректные страницы")]
    public async Task GetAllAsync_Pagination_WorksCorrectly()
    {
        // Arrange
        var (service, _) = EventServiceFactory.Create();
        for (int i = 0; i < 14; i++)
        {
            await service.AddEventAsync(MakeDto($"Событие {i:D2}",
                startAt: new DateTime(2026, 5, 1, 10, 0, 0, DateTimeKind.Utc).AddDays(i)));
        }

        // Act — страница 1, размер 10
        var page1 = await service.GetAllAsync(MakeFilter(page: 1, pageSize: 10));
        // Act — страница 2, размер 10
        var page2 = await service.GetAllAsync(MakeFilter(page: 2, pageSize: 10));

        // Assert
        Assert.Equal(14, page1.TotalCount);
        Assert.Equal(10, page1.Items.Count);
        Assert.Equal(1, page1.Page);
        Assert.Equal(10, page1.PageSize);
        Assert.Equal(2, page1.TotalPages);
        Assert.True(page1.HasNextPage);
        Assert.False(page1.HasPreviousPage);

        Assert.Equal(14, page2.TotalCount);
        Assert.Equal(4, page2.Items.Count);
        Assert.Equal(2, page2.Page);
        Assert.True(page2.HasPreviousPage);
        Assert.False(page2.HasNextPage);
    }

    [Fact(DisplayName = "GetAllAsync: страница за пределами данных возвращает пустой Items")]
    public async Task GetAllAsync_Pagination_OutOfRange_ReturnsEmpty()
    {
        // Arrange
        var (service, _) = EventServiceFactory.Create();
        await service.AddEventAsync(MakeDto("Одно событие"));

        // Act
        var page = await service.GetAllAsync(MakeFilter(page: 5, pageSize: 10));

        // Assert
        Assert.Equal(1, page.TotalCount);
        Assert.Empty(page.Items);
        Assert.Equal(5, page.Page);
    }

    [Fact(DisplayName = "GetAllAsync: комбинированная фильтрация (title + from + to + пагинация)")]
    public async Task GetAllAsync_CombinedFiltersAndPagination()
    {
        // Arrange
        var (service, _) = EventServiceFactory.Create();

        // 3 совпадения по "конференция" в мае
        await service.AddEventAsync(MakeDto("Конференция A",
            startAt: new DateTime(2026, 5, 1, 10, 0, 0, DateTimeKind.Utc),
            endAt: new DateTime(2026, 5, 1, 12, 0, 0, DateTimeKind.Utc)));
        await service.AddEventAsync(MakeDto("Конференция B",
            startAt: new DateTime(2026, 5, 5, 10, 0, 0, DateTimeKind.Utc),
            endAt: new DateTime(2026, 5, 5, 12, 0, 0, DateTimeKind.Utc)));
        await service.AddEventAsync(MakeDto("Конференция C",
            startAt: new DateTime(2026, 5, 10, 10, 0, 0, DateTimeKind.Utc),
            endAt: new DateTime(2026, 5, 10, 12, 0, 0, DateTimeKind.Utc)));

        // Не подходит: конференция, но вне диапазона
        await service.AddEventAsync(MakeDto("Конференция D (июнь)",
            startAt: new DateTime(2026, 6, 15, 10, 0, 0, DateTimeKind.Utc),
            endAt: new DateTime(2026, 6, 15, 12, 0, 0, DateTimeKind.Utc)));

        // Не подходит: май, но не конференция
        await service.AddEventAsync(MakeDto("Воркшоп",
            startAt: new DateTime(2026, 5, 3, 10, 0, 0, DateTimeKind.Utc),
            endAt: new DateTime(2026, 5, 3, 12, 0, 0, DateTimeKind.Utc)));

        var filter = MakeFilter(
            title: "конференция",
            from: new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc),
            to: new DateTime(2026, 5, 31, 23, 59, 59, DateTimeKind.Utc),
            page: 1,
            pageSize: 2);

        // Act
        var page1 = await service.GetAllAsync(filter);
        var page2 = await service.GetAllAsync(MakeFilter(
            title: "конференция",
            from: filter.From,
            to: filter.To,
            page: 2,
            pageSize: 2));

        // Assert
        Assert.Equal(3, page1.TotalCount);
        Assert.Equal(2, page1.Items.Count);
        Assert.Equal(2, page1.TotalPages);
        Assert.True(page1.HasNextPage);

        Assert.Single(page2.Items);
        Assert.False(page2.HasNextPage);
        Assert.Equal(3, page2.TotalCount);
    }

    // =====================================================================
    // НЕУСПЕШНЫЕ СЦЕНАРИИ
    // =====================================================================

    [Fact(DisplayName = "GetAsync: возвращает null для несуществующего Id")]
    public async Task GetAsync_ReturnsNull_WhenNotFound()
    {
        // Arrange
        var (service, _) = EventServiceFactory.Create();

        // Act
        var found = await service.GetAsync(Guid.NewGuid());

        // Assert
        Assert.Null(found);
    }

    [Fact(DisplayName = "UpdateEventAsync: возвращает null для несуществующего Id")]
    public async Task UpdateEventAsync_ReturnsNull_WhenNotFound()
    {
        // Arrange
        var (service, _) = EventServiceFactory.Create();
        var dto = MakeDto("Обновление");

        // Act
        var updated = await service.UpdateEventAsync(Guid.NewGuid(), dto);

        // Assert
        Assert.Null(updated);
    }

    [Fact(DisplayName = "RemoveEventAsync: возвращает false для несуществующего Id")]
    public async Task RemoveEventAsync_ReturnsFalse_WhenNotFound()
    {
        // Arrange
        var (service, _) = EventServiceFactory.Create();

        // Act
        var result = await service.RemoveEventAsync(Guid.NewGuid());

        // Assert
        Assert.False(result);
    }

    [Fact(DisplayName = "AddEventAsync: пустой Title → ArgumentException")]
    public async Task AddEventAsync_EmptyTitle_Throws()
    {
        // Arrange
        var (service, _) = EventServiceFactory.Create();
        var dto = MakeDto(title: "");

        // Act + Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.AddEventAsync(dto));
    }

    [Fact(DisplayName = "AddEventAsync: Title из пробелов → ArgumentException")]
    public async Task AddEventAsync_WhitespaceTitle_Throws()
    {
        // Arrange
        var (service, _) = EventServiceFactory.Create();
        var dto = MakeDto(title: "   ");

        // Act + Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.AddEventAsync(dto));
    }

    [Fact(DisplayName = "AddEventAsync: StartAt == EndAt → ArgumentException")]
    public async Task AddEventAsync_EqualDates_Throws()
    {
        // Arrange
        var (service, _) = EventServiceFactory.Create();
        var dt = new DateTime(2026, 5, 1, 10, 0, 0, DateTimeKind.Utc);
        var dto = MakeDto(startAt: dt, endAt: dt);

        // Act + Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.AddEventAsync(dto));
    }

    [Fact(DisplayName = "AddEventAsync: EndAt раньше StartAt → ArgumentException")]
    public async Task AddEventAsync_EndBeforeStart_Throws()
    {
        // Arrange
        var (service, _) = EventServiceFactory.Create();
        var start = new DateTime(2026, 5, 1, 12, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2026, 5, 1, 10, 0, 0, DateTimeKind.Utc);
        var dto = MakeDto(startAt: start, endAt: end);

        // Act + Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.AddEventAsync(dto));
    }

    [Fact(DisplayName = "AddEventAsync: StartAt == null → ArgumentException")]
    public async Task AddEventAsync_NullStartAt_Throws()
    {
        // Arrange
        var (service, _) = EventServiceFactory.Create();
        var dto = new EventDto
        {
            Title = "Событие",
            Description = null,
            StartAt = null,
            EndAt = new DateTime(2026, 5, 1, 12, 0, 0, DateTimeKind.Utc)
        };

        // Act + Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.AddEventAsync(dto));
    }

    [Fact(DisplayName = "AddEventAsync: EndAt == null → ArgumentException")]
    public async Task AddEventAsync_NullEndAt_Throws()
    {
        // Arrange
        var (service, _) = EventServiceFactory.Create();
        var dto = new EventDto
        {
            Title = "Событие",
            Description = null,
            StartAt = new DateTime(2026, 5, 1, 12, 0, 0, DateTimeKind.Utc),
            EndAt = null
        };

        // Act + Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.AddEventAsync(dto));
    }

    [Fact(DisplayName = "UpdateEventAsync: EndAt раньше StartAt → ArgumentException (до поиска в БД)")]
    public async Task UpdateEventAsync_InvalidDates_Throws()
    {
        // Arrange
        var (service, _) = EventServiceFactory.Create();
        var created = await service.AddEventAsync(MakeDto("Валидное"));

        var start = new DateTime(2026, 6, 1, 12, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc);
        var badDto = MakeDto("Обновление", startAt: start, endAt: end);

        // Act + Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.UpdateEventAsync(created.Id, badDto));

        // Проверим, что исходное событие не изменилось
        var stillOld = await service.GetAsync(created.Id);
        Assert.NotNull(stillOld);
        Assert.Equal("Валидное", stillOld!.Title);
    }
}