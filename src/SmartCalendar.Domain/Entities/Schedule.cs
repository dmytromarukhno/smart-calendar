namespace SmartCalendar.Domain.Entities;

public sealed class Schedule
{
    public Guid Id { get; private set; }
    public Guid EventId { get; private set; }
    public Guid SceneId { get; private set; }
    public int TriggerOffsetMin { get; private set; }
    public bool IsEnabled { get; private set; }
    public DateTime? LastTriggeredAt { get; private set; }

    public Event? Event { get; private set; }
    public Scene? Scene { get; private set; }

    // Required by EF Core
    private Schedule() { }

    public Schedule(Guid eventId, Guid sceneId, int triggerOffsetMin)
    {
        Id = Guid.NewGuid();
        EventId = eventId;
        SceneId = sceneId;
        TriggerOffsetMin = triggerOffsetMin;
        IsEnabled = true;
    }

    public void Enable() => IsEnabled = true;
    public void Disable() => IsEnabled = false;
    public void MarkTriggered(DateTime at) => LastTriggeredAt = at;
}
