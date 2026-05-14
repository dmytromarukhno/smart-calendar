using SmartCalendar.Domain.Entities;

namespace SmartCalendar.Tests.TestHelpers;

public static class EventFactory
{
    public static Event CreateValid(
        string title = "Test Event",
        DateTime? start = null,
        DateTime? end = null)
    {
        var s = start ?? DateTime.UtcNow.AddHours(1);
        var e = end ?? s.AddHours(2);
        return new Event(title, s, e);
    }
}
