using SmartCalendar.Domain.Entities;

namespace SmartCalendar.Domain.Interfaces;

public interface IEventRepository
{
    Task<Event?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Event>> GetAllAsync(CancellationToken ct = default);
    Task<IEnumerable<Event>> GetByDateAsync(DateTime date, CancellationToken ct = default);
    Task<IEnumerable<Event>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken ct = default);
    Task AddAsync(Event @event, CancellationToken ct = default);
    Task UpdateAsync(Event @event, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task ReplaceScheduleAsync(Guid eventId, Guid sceneId, int triggerOffsetMin, CancellationToken ct = default);
    Task ReplaceRemindersAsync(Guid eventId, IReadOnlyList<int> offsets, CancellationToken ct = default);
}
