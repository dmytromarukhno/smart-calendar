using SmartCalendar.Application.Interfaces;

namespace SmartCalendar.Infrastructure.System;

public sealed class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
