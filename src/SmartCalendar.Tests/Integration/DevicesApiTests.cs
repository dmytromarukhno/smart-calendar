using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SmartCalendar.Application.Dtos;
using SmartCalendar.Tests.TestHelpers;

namespace SmartCalendar.Tests.Integration;

public sealed class DevicesApiTests : IAsyncLifetime
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
    public async Task GetDevices_ReturnsFiveMockDevices()
    {
        var response = await _client.GetAsync("/api/devices");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var devices = await response.Content.ReadFromJsonAsync<List<DeviceDto>>();
        devices.Should().NotBeNull().And.HaveCount(5);
    }

    [Fact]
    public async Task GetDevices_AllDevicesAreOnline()
    {
        var response = await _client.GetAsync("/api/devices");

        var devices = await response.Content.ReadFromJsonAsync<List<DeviceDto>>();
        devices.Should().AllSatisfy(d => d.IsOnline.Should().BeTrue());
    }

    [Fact]
    public async Task GetDevices_ContainsExpectedLocations()
    {
        var response = await _client.GetAsync("/api/devices");

        var devices = await response.Content.ReadFromJsonAsync<List<DeviceDto>>();
        var locations = devices!.Select(d => d.Location).Distinct().ToList();
        locations.Should().Contain("livingroom").And.Contain("bedroom").And.Contain("hallway");
    }
}
