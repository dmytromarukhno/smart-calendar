using Microsoft.Extensions.Logging;
using SmartCalendar.Domain.Interfaces;

namespace SmartCalendar.Application.Services;

public sealed class ReminderDispatcher
{
    private readonly IEventRepository _events;
    private readonly IUnitOfWork _uow;
    private readonly ILogger<ReminderDispatcher> _log;

    public ReminderDispatcher(IEventRepository events, IUnitOfWork uow,
        ILogger<ReminderDispatcher> log)
    {
        _events = events;
        _uow = uow;
        _log = log;
    }

    public async Task DispatchPendingAsync(CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var events = await _events.GetAllAsync(ct);

        foreach (var @event in events)
        {
            foreach (var reminder in @event.Reminders.Where(r => !r.IsSent))
            {
                var triggerTime = @event.StartTime.AddMinutes(-reminder.OffsetMinutes);
                if (now >= triggerTime)
                {
                    reminder.MarkSent();
                    _log.LogInformation("[Reminder] Event '{Title}' starts in {Offset} min",
                        @event.Title, reminder.OffsetMinutes);
                }
            }
        }

        await _uow.SaveChangesAsync(ct);
    }
}
