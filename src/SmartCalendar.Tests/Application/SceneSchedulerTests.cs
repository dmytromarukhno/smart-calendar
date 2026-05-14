using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SmartCalendar.Domain.Entities;
using SmartCalendar.Infrastructure.Persistence;
using SmartCalendar.Infrastructure.Persistence.Repositories;

namespace SmartCalendar.Tests.Application;

public sealed class SceneSchedulerTests
{
    private static SmartCalendarDbContext CreateDb()
    {
        var opts = new DbContextOptionsBuilder<SmartCalendarDbContext>()
            .UseInMemoryDatabase($"sched-test-{Guid.NewGuid():N}")
            .Options;
        return new SmartCalendarDbContext(opts);
    }

    private static async Task<(Scene scene, Event @event)> SeedAsync(
        SmartCalendarDbContext db, DateTime eventStart)
    {
        var device = new Device("Light", "livingroom");
        db.Devices.Add(device);

        var scene = new Scene("Cinema");
        scene.AddCommand(new Command(scene.Id, device, "dim", "20", 1));
        db.Scenes.Add(scene);

        var @event = new Event("Test Event", eventStart, eventStart.AddHours(1));
        db.Events.Add(@event);

        await db.SaveChangesAsync();
        return (scene, @event);
    }

    [Fact]
    public async Task GetDueAsync_WhenTriggerTimeReached_ReturnsDueSchedule()
    {
        // Event in 2 min, offset = 2 min → triggerTime = UtcNow → due
        using var db = CreateDb();
        var now = DateTime.UtcNow;
        var (scene, @event) = await SeedAsync(db, now.AddMinutes(2));

        var schedule = new Schedule(@event.Id, scene.Id, triggerOffsetMin: 2);
        db.Schedules.Add(schedule);
        await db.SaveChangesAsync();

        var sut = new EfScheduleRepository(db);
        var due = await sut.GetDueAsync(now);

        due.Should().ContainSingle(s => s.Id == schedule.Id);
    }

    [Fact]
    public async Task GetDueAsync_WhenTriggerTimeInFuture_ReturnsEmpty()
    {
        // Event in 10 min, offset = 1 min → triggerTime = UtcNow+9min → not due
        using var db = CreateDb();
        var now = DateTime.UtcNow;
        var (scene, @event) = await SeedAsync(db, now.AddMinutes(10));

        var schedule = new Schedule(@event.Id, scene.Id, triggerOffsetMin: 1);
        db.Schedules.Add(schedule);
        await db.SaveChangesAsync();

        var sut = new EfScheduleRepository(db);
        var due = await sut.GetDueAsync(now);

        due.Should().BeEmpty();
    }

    [Fact]
    public async Task GetDueAsync_WhenAlreadyTriggered_ReturnsEmpty()
    {
        // Due schedule but already marked as triggered — must not fire again
        using var db = CreateDb();
        var now = DateTime.UtcNow;
        var (scene, @event) = await SeedAsync(db, now.AddMinutes(2));

        var schedule = new Schedule(@event.Id, scene.Id, triggerOffsetMin: 2);
        schedule.MarkTriggered(now.AddHours(-1)); // already triggered an hour ago
        db.Schedules.Add(schedule);
        await db.SaveChangesAsync();

        var sut = new EfScheduleRepository(db);
        var due = await sut.GetDueAsync(now);

        due.Should().BeEmpty();
    }
}
