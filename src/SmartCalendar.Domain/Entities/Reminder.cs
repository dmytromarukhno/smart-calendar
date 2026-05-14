namespace SmartCalendar.Domain.Entities;

public sealed class Reminder
{
    public Guid Id { get; private set; }
    public Guid EventId { get; private set; }
    public int OffsetMinutes { get; private set; }
    public bool IsSent { get; private set; }

    // Required by EF Core
    private Reminder() { }

    public Reminder(Guid eventId, int offsetMinutes)
    {
        Id = Guid.NewGuid();
        EventId = eventId;
        OffsetMinutes = offsetMinutes;
        IsSent = false;
    }

    public void MarkSent() => IsSent = true;
}
