namespace SmartCalendar.Infrastructure.Scheduling;

public sealed class SchedulerOptions
{
    public TimeSpan TickInterval { get; set; } = TimeSpan.FromSeconds(30);
}
