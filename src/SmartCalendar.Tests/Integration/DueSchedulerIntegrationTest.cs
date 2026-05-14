using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SmartCalendar.Application.Interfaces;
using SmartCalendar.Domain.Entities;
using SmartCalendar.Domain.ValueObjects;
using SmartCalendar.Infrastructure.Persistence;
using SmartCalendar.Infrastructure.Scheduling;

namespace SmartCalendar.Tests.Integration;

public sealed class DueSchedulerIntegrationTest : IAsyncLifetime
{
    private readonly List<string> _capturedTopics = new();
    private SchedulerIntegrationFactory _factory = null!;

    public Task InitializeAsync()
    {
        _factory = new SchedulerIntegrationFactory(_capturedTopics);
        _factory.CreateClient(); // start app + migrations + seed
        return Task.CompletedTask;
    }

    public async Task DisposeAsync() => await _factory.DisposeAsync();

    [Fact]
    public async Task WhenScheduleIsDue_PublishesMqttCommand()
    {
        // Arrange: scene with 1 command, event at UtcNow+2min, offset=2min
        // → triggerTime = UtcNow → immediately due on first scheduler tick
        await using var scope = _factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<SmartCalendarDbContext>();

        var device = await db.Devices.FirstAsync(d => d.Location == "livingroom");

        var scene = new Scene("Cinema");
        scene.AddCommand(new Command(scene.Id, device, "dim", "20", 1));
        db.Scenes.Add(scene);

        var eventStart = DateTime.UtcNow.AddMinutes(2);
        var @event = new Event("Test-Cinema", eventStart, eventStart.AddHours(1));
        db.Events.Add(@event);
        await db.SaveChangesAsync();

        var schedule = new Schedule(@event.Id, scene.Id, triggerOffsetMin: 2);
        db.Schedules.Add(schedule);
        await db.SaveChangesAsync();

        // Act: scheduler fires every 1 s → at least 2 ticks within 3 s
        await Task.Delay(TimeSpan.FromSeconds(3));

        // Assert
        _capturedTopics.Should().NotBeEmpty("scheduler must publish the scene command");
        _capturedTopics.Should().Contain(t => t.StartsWith("smart/livingroom/"),
            "topic must follow smart/{location}/{device} pattern");
    }
}

// ── Test factory ──────────────────────────────────────────────────────────────

internal sealed class SchedulerIntegrationFactory : WebApplicationFactory<Program>, IAsyncDisposable
{
    private readonly string _dbPath =
        Path.Combine(Path.GetTempPath(), $"sc-sched-{Guid.NewGuid():N}.db");

    private readonly List<string> _capturedTopics;

    public SchedulerIntegrationFactory(List<string> capturedTopics)
    {
        _capturedTopics = capturedTopics;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, cfg) =>
            cfg.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = $"Data Source={_dbPath}"
            }));

        builder.ConfigureServices(services =>
        {
            services.Configure<SchedulerOptions>(opts =>
                opts.TickInterval = TimeSpan.FromSeconds(1));

            services.RemoveAll<IMqttPublisher>();
            services.AddSingleton<IMqttPublisher>(
                new CapturingMqttPublisher(_capturedTopics));
        });
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        await base.DisposeAsync();
        foreach (var suffix in new[] { "", "-shm", "-wal" })
        {
            var path = _dbPath + suffix;
            if (File.Exists(path)) File.Delete(path);
        }
    }

    private sealed class CapturingMqttPublisher : IMqttPublisher
    {
        private readonly List<string> _topics;

        public CapturingMqttPublisher(List<string> topics) => _topics = topics;

        public Task PublishAsync(string topic, MqttCommandPayload payload, CancellationToken ct = default)
        {
            lock (_topics) _topics.Add(topic);
            return Task.CompletedTask;
        }
    }
}
