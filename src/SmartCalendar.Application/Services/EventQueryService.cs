using SmartCalendar.Application.Dtos;
using SmartCalendar.Domain.Exceptions;
using SmartCalendar.Domain.Interfaces;

namespace SmartCalendar.Application.Services;

public sealed class EventQueryService
{
    private readonly IEventRepository _events;

    public EventQueryService(IEventRepository events) => _events = events;

    public async Task<EventDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var @event = await _events.GetByIdAsync(id, ct)
            ?? throw new EventNotFoundException(id);
        return MapToDto(@event);
    }

    public async Task<IEnumerable<EventDto>> GetForDateAsync(DateTime date, CancellationToken ct = default)
    {
        var events = await _events.GetByDateAsync(date, ct);
        return events.Select(MapToDto);
    }

    public async Task<IEnumerable<EventDto>> GetAllAsync(CancellationToken ct = default)
    {
        var events = await _events.GetAllAsync(ct);
        return events.Select(MapToDto);
    }

    public async Task<IEnumerable<EventDto>> GetForDateRangeAsync(
        DateTime from, DateTime to, CancellationToken ct = default)
    {
        var events = await _events.GetByDateRangeAsync(from, to, ct);
        return events.Select(MapToDto);
    }

    private static EventDto MapToDto(Domain.Entities.Event e) =>
        new(e.Id, e.Title, e.StartTime, e.EndTime, e.IsRecurring, e.RecurrencePattern,
            e.Schedules.Any());
}
