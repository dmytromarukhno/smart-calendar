using SmartCalendar.Domain.Entities;

namespace SmartCalendar.Domain.Interfaces;

public interface IScheduleRepository
{
    Task<IReadOnlyList<Schedule>> GetDueAsync(DateTime now, CancellationToken ct = default);
    Task MarkTriggeredAsync(Guid scheduleId, DateTime at, CancellationToken ct = default);
}
