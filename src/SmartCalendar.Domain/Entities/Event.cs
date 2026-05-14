using SmartCalendar.Domain.Exceptions;

namespace SmartCalendar.Domain.Entities;

public sealed class Event
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = null!;
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public bool IsRecurring { get; private set; }
    public string RecurrencePattern { get; private set; } = string.Empty;

    private readonly List<Reminder> _reminders = new();
    public IReadOnlyCollection<Reminder> Reminders => _reminders;

    private readonly List<Schedule> _schedules = new();
    public IReadOnlyCollection<Schedule> Schedules => _schedules;

    // Required by EF Core
    private Event() { }

    public Event(string title, DateTime start, DateTime end)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Title is required");
        if (end <= start)
            throw new DomainException("End must be after start");

        Id = Guid.NewGuid();
        Title = title;
        StartTime = start;
        EndTime = end;
        RecurrencePattern = string.Empty;
    }

    public void AddReminder(int offsetMinutes)
    {
        _reminders.Add(new Reminder(Id, offsetMinutes));
    }

    public void ReplaceReminders(IEnumerable<int> offsets)
    {
        _reminders.Clear();
        foreach (var offset in offsets)
            _reminders.Add(new Reminder(Id, offset));
    }

    public bool IsActiveAt(DateTime dt) =>
        dt >= StartTime && dt <= EndTime;

    public void Update(string title, DateTime start, DateTime end,
        bool isRecurring, string recurrencePattern)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Title is required");
        if (end <= start)
            throw new DomainException("End must be after start");

        Title = title;
        StartTime = start;
        EndTime = end;
        IsRecurring = isRecurring;
        RecurrencePattern = recurrencePattern;
    }

    public void AddSchedule(Schedule schedule) =>
        _schedules.Add(schedule);
}
