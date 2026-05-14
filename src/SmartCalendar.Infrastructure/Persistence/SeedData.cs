using Microsoft.Extensions.Logging;
using SmartCalendar.Domain.Entities;

namespace SmartCalendar.Infrastructure.Persistence;

public static class SeedData
{
    public static async Task InitializeAsync(SmartCalendarDbContext db, ILogger logger,
        CancellationToken ct = default)
    {
        if (db.Devices.Any())
            return;

        var devices = new[]
        {
            new Device("Living Room Light", "livingroom"),
            new Device("Living Room Curtain", "livingroom"),
            new Device("Living Room AV", "livingroom"),
            new Device("Bedroom Climate", "bedroom"),
            new Device("Hallway Alarm", "hallway"),
        };

        await db.Devices.AddRangeAsync(devices, ct);
        await db.SaveChangesAsync(ct);
        logger.LogInformation("Seeded {Count} mock devices", devices.Length);

        var kinoScene = new Scene("Кіно");
        kinoScene.AddCommand(new Command(kinoScene.Id, devices[0], "dim", "20", 1));
        await db.Scenes.AddAsync(kinoScene, ct);
        await db.SaveChangesAsync(ct);
        logger.LogInformation("Seeded demo scene '{Name}' with {Count} command(s)", kinoScene.Name, 1);
    }
}
