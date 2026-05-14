using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SmartCalendar.Application.Dtos;
using SmartCalendar.Tests.TestHelpers;

namespace SmartCalendar.Tests.Integration;

public sealed class EventsApiTests : IAsyncLifetime
{
    private readonly TestWebApplicationFactory _factory = new();
    private HttpClient _client = null!;

    public Task InitializeAsync()
    {
        _client = _factory.CreateClient();
        return Task.CompletedTask;
    }

    public async Task DisposeAsync() =>
        await ((IAsyncDisposable)_factory).DisposeAsync();

    [Fact]
    public async Task PostEvent_ReturnsCreated()
    {
        var dto = new CreateEventDto("New Event", DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2));

        var response = await _client.PostAsJsonAsync("/api/events", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<IdResponse>();
        body!.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetEvents_ReturnsOkWithList()
    {
        // Arrange: create one event first
        var dto = new CreateEventDto("List Test", DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2));
        await _client.PostAsJsonAsync("/api/events", dto);

        // Act
        var response = await _client.GetAsync("/api/events");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var events = await response.Content.ReadFromJsonAsync<List<EventDto>>();
        events.Should().NotBeNull().And.NotBeEmpty();
    }

    [Fact]
    public async Task GetEvents_WithDateRange_ReturnsFilteredList()
    {
        var start = DateTime.UtcNow.AddHours(1);
        var dto = new CreateEventDto("Range Event", start, start.AddHours(1));
        await _client.PostAsJsonAsync("/api/events", dto);

        var from = start.Date.ToString("yyyy-MM-dd");
        var to = start.Date.ToString("yyyy-MM-dd");
        var response = await _client.GetAsync($"/api/events?from={from}&to={to}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var events = await response.Content.ReadFromJsonAsync<List<EventDto>>();
        events.Should().NotBeNull().And.Contain(e => e.Title == "Range Event");
    }

    [Fact]
    public async Task GetEventById_WhenExists_ReturnsOk()
    {
        var dto = new CreateEventDto("Single Event", DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2));
        var created = await _client.PostAsJsonAsync("/api/events", dto);
        var body = await created.Content.ReadFromJsonAsync<IdResponse>();

        var response = await _client.GetAsync($"/api/events/{body!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var eventDto = await response.Content.ReadFromJsonAsync<EventDto>();
        eventDto!.Title.Should().Be("Single Event");
    }

    [Fact]
    public async Task GetEventById_WhenNotFound_Returns404()
    {
        var response = await _client.GetAsync($"/api/events/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteEvent_WhenExists_Returns204()
    {
        var dto = new CreateEventDto("To Delete", DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2));
        var created = await _client.PostAsJsonAsync("/api/events", dto);
        var body = await created.Content.ReadFromJsonAsync<IdResponse>();

        var response = await _client.DeleteAsync($"/api/events/{body!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteEvent_WhenNotFound_Returns404()
    {
        var response = await _client.DeleteAsync($"/api/events/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private sealed record IdResponse(Guid Id);
}
