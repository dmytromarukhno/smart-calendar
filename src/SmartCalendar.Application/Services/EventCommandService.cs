using SmartCalendar.Application.Dtos;
using SmartCalendar.Domain.Constants;
using SmartCalendar.Domain.Entities;
using SmartCalendar.Domain.Exceptions;
using SmartCalendar.Domain.Interfaces;

namespace SmartCalendar.Application.Services;

public sealed class EventCommandService
{
    private readonly IEventRepository _events;
    private readonly IUnitOfWork _uow;

    public EventCommandService(IEventRepository events, IUnitOfWork uow)
    {
        _events = events;
        _uow = uow;
    }

    public async Task<Guid> CreateAsync(CreateEventDto dto, CancellationToken ct = default)
    {
        var @event = new Event(dto.Title, dto.Start, dto.End);
        await _events.AddAsync(@event, ct);
        await _uow.SaveChangesAsync(ct);
        return @event.Id;
    }

    public async Task UpdateAsync(UpdateEventDto dto, CancellationToken ct = default)
    {
        var @event = await GetEventOrThrowAsync(dto.Id, ct);
        @event.Update(dto.Title, dto.Start, dto.End, dto.IsRecurring, dto.RecurrencePattern);
        if (dto.ReminderOffsetMin > 0)
            @event.AddReminder(dto.ReminderOffsetMin);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await GetEventOrThrowAsync(id, ct);
        await _events.DeleteAsync(id, ct);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task AttachSceneAsync(Guid eventId, Guid sceneId, CancellationToken ct = default)
    {
        var @event = await GetEventOrThrowAsync(eventId, ct);
        var schedule = new Schedule(eventId, sceneId, ReminderDefaults.SceneTriggerOffsetMinutes);
        @event.AddSchedule(schedule);
        await _uow.SaveChangesAsync(ct);
    }

    private async Task<Event> GetEventOrThrowAsync(Guid id, CancellationToken ct)
        => await _events.GetByIdAsync(id, ct)
           ?? throw new EventNotFoundException(id);
}
