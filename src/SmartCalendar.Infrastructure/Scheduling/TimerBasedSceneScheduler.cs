using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmartCalendar.Application.Services;
using SmartCalendar.Domain.Interfaces;

namespace SmartCalendar.Infrastructure.Scheduling;

public sealed class TimerBasedSceneScheduler : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<TimerBasedSceneScheduler> _log;

    private static readonly TimeSpan TickInterval = TimeSpan.FromSeconds(30);

    public TimerBasedSceneScheduler(IServiceScopeFactory scopeFactory,
        ILogger<TimerBasedSceneScheduler> log)
    {
        _scopeFactory = scopeFactory;
        _log = log;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _log.LogInformation("TimerBasedSceneScheduler started");

        using var timer = new PeriodicTimer(TickInterval);
        while (await timer.WaitForNextTickAsync(stoppingToken))
            await TickAsync(stoppingToken);
    }

    private async Task TickAsync(CancellationToken ct)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        await DispatchRemindersAsync(scope, ct);
        await TriggerScheduledScenesAsync(scope, ct);
    }

    private static async Task DispatchRemindersAsync(AsyncServiceScope scope, CancellationToken ct)
    {
        var dispatcher = scope.ServiceProvider.GetRequiredService<ReminderDispatcher>();
        await dispatcher.DispatchPendingAsync(ct);
    }

    private async Task TriggerScheduledScenesAsync(AsyncServiceScope scope, CancellationToken ct)
    {
        var events = scope.ServiceProvider.GetRequiredService<IEventRepository>();
        var executor = scope.ServiceProvider.GetRequiredService<SceneExecutor>();
        var now = DateTime.UtcNow;

        var allEvents = await events.GetAllAsync(ct);
        foreach (var @event in allEvents)
        {
            foreach (var schedule in @event.Schedules.Where(s => s.IsEnabled))
            {
                var triggerTime = @event.StartTime.AddMinutes(-schedule.TriggerOffsetMin);
                if (now >= triggerTime && now < triggerTime.AddSeconds(TickInterval.TotalSeconds))
                {
                    _log.LogInformation("Triggering scene {SceneId} for event '{Title}'",
                        schedule.SceneId, @event.Title);
                    await executor.ExecuteAsync(schedule.SceneId, ct);
                }
            }
        }
    }
}
