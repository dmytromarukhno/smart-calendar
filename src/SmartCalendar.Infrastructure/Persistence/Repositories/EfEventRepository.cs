using Microsoft.EntityFrameworkCore;
using SmartCalendar.Domain.Entities;
using SmartCalendar.Domain.Interfaces;

namespace SmartCalendar.Infrastructure.Persistence.Repositories;

public sealed class EfEventRepository : IEventRepository
{
    private readonly SmartCalendarDbContext _db;

    public EfEventRepository(SmartCalendarDbContext db) => _db = db;

    public async Task<Event?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _db.Events
            .Include(e => e.Reminders)
            .Include(e => e.Schedules)
            .FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<IEnumerable<Event>> GetAllAsync(CancellationToken ct = default) =>
        await _db.Events
            .Include(e => e.Reminders)
            .Include(e => e.Schedules)
            .ToListAsync(ct);

    public async Task<IEnumerable<Event>> GetByDateAsync(DateTime date, CancellationToken ct = default) =>
        await _db.Events
            .Include(e => e.Reminders)
            .Include(e => e.Schedules)
            .Where(e => e.StartTime.Date == date.Date)
            .ToListAsync(ct);

    public async Task<IEnumerable<Event>> GetByDateRangeAsync(
        DateTime from, DateTime to, CancellationToken ct = default) =>
        await _db.Events
            .Include(e => e.Reminders)
            .Include(e => e.Schedules)
            .Where(e => e.StartTime.Date >= from.Date && e.StartTime.Date <= to.Date)
            .ToListAsync(ct);

    public async Task AddAsync(Event @event, CancellationToken ct = default) =>
        await _db.Events.AddAsync(@event, ct);

    public Task UpdateAsync(Event @event, CancellationToken ct = default)
    {
        _db.Events.Update(@event);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var @event = await GetByIdAsync(id, ct);
        if (@event is not null)
            _db.Events.Remove(@event);
    }

    public async Task AddScheduleAsync(Schedule schedule, CancellationToken ct = default) =>
        await _db.Schedules.AddAsync(schedule, ct);
}
