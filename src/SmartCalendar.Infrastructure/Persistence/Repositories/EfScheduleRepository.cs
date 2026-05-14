using Microsoft.EntityFrameworkCore;
using SmartCalendar.Domain.Entities;
using SmartCalendar.Domain.Interfaces;

namespace SmartCalendar.Infrastructure.Persistence.Repositories;

public sealed class EfScheduleRepository : IScheduleRepository
{
    private readonly SmartCalendarDbContext _db;

    public EfScheduleRepository(SmartCalendarDbContext db) => _db = db;

    public async Task<IReadOnlyList<Schedule>> GetDueAsync(
        DateTime now, CancellationToken ct = default)
    {
        var candidates = await _db.Schedules
            .Include(s => s.Event)
            .Include(s => s.Scene)
            .Where(s => s.IsEnabled && s.LastTriggeredAt == null)
            .ToListAsync(ct);

        return candidates
            .Where(s => s.Event!.StartTime.AddMinutes(-s.TriggerOffsetMin) <= now)
            .ToList()
            .AsReadOnly();
    }

    public async Task MarkTriggeredAsync(Guid scheduleId, DateTime at, CancellationToken ct = default) =>
        await _db.Schedules
            .Where(s => s.Id == scheduleId)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.LastTriggeredAt, at), ct);
}
