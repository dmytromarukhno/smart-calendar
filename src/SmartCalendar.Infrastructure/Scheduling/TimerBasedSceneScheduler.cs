using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartCalendar.Application.Interfaces;
using SmartCalendar.Application.Services;
using SmartCalendar.Domain.Interfaces;

namespace SmartCalendar.Infrastructure.Scheduling;

public sealed class TimerBasedSceneScheduler : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IClock _clock;
    private readonly TimeSpan _tickInterval;
    private readonly ILogger<TimerBasedSceneScheduler> _log;

    public TimerBasedSceneScheduler(
        IServiceScopeFactory scopeFactory,
        IClock clock,
        IOptions<SchedulerOptions> options,
        ILogger<TimerBasedSceneScheduler> log)
    {
        _scopeFactory = scopeFactory;
        _clock = clock;
        _tickInterval = options.Value.TickInterval;
        _log = log;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _log.LogInformation("TimerBasedSceneScheduler started (interval {Interval}s)",
            (int)_tickInterval.TotalSeconds);

        using var timer = new PeriodicTimer(_tickInterval);
        while (await timer.WaitForNextTickAsync(stoppingToken))
            await TickAsync(stoppingToken);
    }

    private async Task TickAsync(CancellationToken ct)
    {
        var now = _clock.UtcNow;
        _log.LogInformation("[Scheduler] tick {Time:HH:mm:ss} — checking due schedules",
            now.ToLocalTime());

        await using var scope = _scopeFactory.CreateAsyncScope();
        await DispatchRemindersAsync(scope, ct);
        await TriggerDueScenesAsync(scope, now, ct);
    }

    private static async Task DispatchRemindersAsync(AsyncServiceScope scope, CancellationToken ct)
    {
        var dispatcher = scope.ServiceProvider.GetRequiredService<ReminderDispatcher>();
        await dispatcher.DispatchPendingAsync(ct);
    }

    private async Task TriggerDueScenesAsync(AsyncServiceScope scope, DateTime now, CancellationToken ct)
    {
        var scheduleRepo = scope.ServiceProvider.GetRequiredService<IScheduleRepository>();
        var executor = scope.ServiceProvider.GetRequiredService<SceneExecutor>();

        var due = await scheduleRepo.GetDueAsync(now, ct);
        foreach (var schedule in due)
        {
            var sceneName = schedule.Scene?.Name ?? schedule.SceneId.ToString("N")[..8];
            var eventTitle = schedule.Event?.Title ?? schedule.EventId.ToString("N")[..8];

            _log.LogInformation(
                "[Scheduler] firing scene '{SceneName}' for event '{EventTitle}' (triggers in {Offset} min before start)",
                sceneName, eventTitle, schedule.TriggerOffsetMin);

            await executor.ExecuteAsync(schedule.SceneId, ct);
            await scheduleRepo.MarkTriggeredAsync(schedule.Id, now, ct);
        }
    }
}
