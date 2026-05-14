# CODE_FRAGMENTS.md — обов'язкові код-фрагменти

> Ці фрагменти **дослівно** взяті зі звітів ЛР4 та ЛР5. Реалізація проекту має містити **саме ці класи зі саме цими сигнатурами**. Це забезпечує 1:1 відповідність між звітами та реальним кодом.

## Доменна сутність Event (з ЛР4 § 7)

Розташування: `src/SmartCalendar.Domain/Entities/Event.cs`

```csharp
public sealed class Event
{
    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public bool IsRecurring { get; private set; }
    private readonly List<Reminder> _reminders = new();
    public IReadOnlyCollection<Reminder> Reminders => _reminders;

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
    }

    public void AddReminder(int offsetMinutes)
    {
        _reminders.Add(new Reminder(Id, offsetMinutes));
    }
}
```

## Сервіс SceneExecutor (з ЛР4 § 7)

Розташування: `src/SmartCalendar.Application/Services/SceneExecutor.cs`

```csharp
public sealed class SceneExecutor
{
    private readonly IMqttPublisher _mqtt;
    private readonly ISceneRepository _scenes;
    private readonly ILogger<SceneExecutor> _log;

    public SceneExecutor(IMqttPublisher mqtt,
                         ISceneRepository scenes,
                         ILogger<SceneExecutor> log)
    {
        _mqtt = mqtt; _scenes = scenes; _log = log;
    }

    public async Task ExecuteAsync(Guid sceneId, CancellationToken ct)
    {
        var scene = await _scenes.GetByIdAsync(sceneId, ct)
            ?? throw new NotFoundException(nameof(Scene), sceneId);

        foreach (var cmd in scene.Commands.OrderBy(c => c.Order))
        {
            var topic = $"smart/home/{cmd.Device.Location}/{cmd.Device.Id}";
            var payload = new { action = cmd.Action, value = cmd.Value };
            await _mqtt.PublishAsync(topic, payload, ct);
            _log.LogInformation("Published {Topic} {@Payload}",topic,payload);
        }
    }
}
```

> **Примітка з ЛР5 (Рекомендація 1):** після первинної реалізації цей метод належить рефакторити, винісши `GetSceneOrThrowAsync`, `PublishCommandAsync`, `BuildMqttTopic` в окремі методи. Рефакторована версія наведена нижче.

## SceneTriggerJob (з ЛР4 § 7)

> **Увага:** у поточному інкременті Quartz.NET не використовується; замість нього — `BackgroundService` з `System.Threading.Timer`. Однак клас `SceneTriggerJob` лишається у звіті як архітектурне рішення «що буде у майбутньому». Тому **не реалізовуй цей клас у коді поточного інкременту**. Замість нього створи `TimerBasedSceneScheduler : BackgroundService` (інтерфейс — той самий, виклик `_executor.ExecuteAsync(sceneId, ct)`).

Архітектурна референція:

```csharp
[DisallowConcurrentExecution]
public class SceneTriggerJob : IJob
{
    private readonly SceneExecutor _executor;
    public SceneTriggerJob(SceneExecutor executor) => _executor = executor;

    public async Task Execute(IJobExecutionContext context)
    {
        var sceneId = (Guid)context.MergedJobDataMap["sceneId"];
        await _executor.ExecuteAsync(sceneId, context.CancellationToken);
    }
}
```

## Minimal API endpoint (з ЛР4 § 7)

Розташування: `src/SmartCalendar.Api/Program.cs`

```csharp
app.MapPost("/api/events", async (CreateEventDto dto,
                                 IEventService svc) =>
{
    var id = await svc.CreateAsync(dto);
    return Results.Created($"/api/events/{id}", new { id });
})
.RequireAuthorization()
.WithName("CreateEvent")
.WithOpenApi();
```

---

# Рефакторовані версії з ЛР5

Наступні фрагменти — обов'язкові, бо так виглядає код **після** застосування рекомендацій з ЛР5.

## Рефакторений SceneExecutor (з ЛР5 § 4, Рекомендація 1)

Замість оригінальної версії з ЛР4 — використай цю:

```csharp
public async Task ExecuteAsync(Guid sceneId, CancellationToken ct)
{
    var scene = await GetSceneOrThrowAsync(sceneId, ct);
    foreach (var command in scene.Commands.OrderBy(c => c.Order))
        await PublishCommandAsync(command, ct);
}

private async Task<Scene> GetSceneOrThrowAsync(Guid id, CancellationToken ct)
    => await _scenes.GetByIdAsync(id, ct)
       ?? throw new SceneNotFoundException(id);

private async Task PublishCommandAsync(Command cmd, CancellationToken ct)
{
    var topic = BuildMqttTopic(cmd.Device);
    var payload = new MqttCommandPayload(cmd.Action, cmd.Value);
    await _mqtt.PublishAsync(topic, payload, ct);
    _log.LogInformation("Published {Topic} {@Payload}", topic, payload);
}

private static string BuildMqttTopic(Device d)
    => $"smart/home/{d.Location}/{d.Id}";
```

## Іменовані константи ReminderDefaults (з ЛР5 § 4, Рекомендація 2)

Розташування: `src/SmartCalendar.Domain/Constants/ReminderDefaults.cs`

```csharp
public static class ReminderDefaults
{
    public const int FirstWarningMinutes = 10;
    public const int FinalWarningMinutes = 1;
    public const int SceneTriggerOffsetMinutes = 1;
}

event.AddReminder(ReminderDefaults.FirstWarningMinutes);
event.AddReminder(ReminderDefaults.FinalWarningMinutes);
scheduler.ScheduleTrigger(event.Id,
    event.StartTime.AddMinutes(-ReminderDefaults.SceneTriggerOffsetMinutes));
```

## Доменні винятки (з ЛР5 § 4, Рекомендація 3)

Розташування: `src/SmartCalendar.Domain/Exceptions/`

```csharp
public sealed class DeviceOfflineException : DomainException
{
    public Guid DeviceId { get; }
    public DeviceOfflineException(Guid deviceId)
        : base($"Device {deviceId} is offline")
        => DeviceId = deviceId;
}

// у виклику:
if (device is null) throw new DeviceNotFoundException(deviceId);
if (!device.IsOnline) throw new DeviceOfflineException(device.Id);
```

Базовий клас:

```csharp
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message) { }
}

public sealed class SceneNotFoundException : DomainException
{
    public Guid SceneId { get; }
    public SceneNotFoundException(Guid id)
        : base($"Scene {id} not found") => SceneId = id;
}

public sealed class DeviceNotFoundException : DomainException
{
    public Guid DeviceId { get; }
    public DeviceNotFoundException(Guid id)
        : base($"Device {id} not found") => DeviceId = id;
}
```

## Розбиття EventService за SRP (з ЛР5 § 4, Рекомендація 4)

Замість одного `EventService` — три класи:

```csharp
public class EventCommandService    // CUD-операції
{
    public Task<Guid> CreateAsync(CreateEventDto dto);
    public Task UpdateAsync(UpdateEventDto dto);
    public Task DeleteAsync(Guid id);
}

public class EventQueryService      // Read-операції (CQRS)
{
    public Task<EventDto> GetByIdAsync(Guid id);
    public Task<IEnumerable<EventDto>> GetForDateAsync(DateTime date);
    public Task<EventStatistics> GetStatisticsAsync(DateRange range);
}

public class EventDigestNotifier    // Періодичні нагадування
{
    public Task SendUpcomingDigestAsync(Guid userId);
}
```

## DTO замість довгого списку параметрів (з ЛР5 § 4, Рекомендація 5)

```csharp
public sealed record UpdateEventDto(
    Guid Id,
    string Title,
    DateTime Start,
    DateTime End,
    bool IsRecurring,
    string RecurrencePattern,
    int ReminderOffsetMin);

await eventService.UpdateAsync(new UpdateEventDto(
    id: eventId,
    Title: "Кіно",
    Start: new DateTime(2026, 5, 13, 21, 0, 0),
    End: new DateTime(2026, 5, 13, 23, 0, 0),
    IsRecurring: true,
    RecurrencePattern: "FREQ=WEEKLY;BYDAY=TU,SA",
    ReminderOffsetMin: 10));
```

---

# Резюме: які класи мають з'явитися у коді

| Файл | Джерело |
|---|---|
| `Domain/Entities/Event.cs` | ЛР4 фрагмент 1 |
| `Domain/Entities/Reminder.cs` | ЛР3 UML (за аналогією Event) |
| `Domain/Entities/Scene.cs` | ЛР3 UML |
| `Domain/Entities/Command.cs` | ЛР3 UML |
| `Domain/Entities/Schedule.cs` | ЛР3 UML |
| `Domain/Entities/Device.cs` | ЛР3 UML |
| `Domain/Entities/User.cs` + `UserRole.cs` | ЛР3 UML |
| `Domain/Constants/ReminderDefaults.cs` | ЛР5 Рекомендація 2 |
| `Domain/Exceptions/DomainException.cs` + `SceneNotFoundException.cs` + `DeviceNotFoundException.cs` + `DeviceOfflineException.cs` | ЛР5 Рекомендація 3 |
| `Application/Services/SceneExecutor.cs` | ЛР5 Рекомендація 1 (рефакторована) |
| `Application/Services/EventCommandService.cs` | ЛР5 Рекомендація 4 |
| `Application/Services/EventQueryService.cs` | ЛР5 Рекомендація 4 |
| `Application/Dtos/UpdateEventDto.cs` + `CreateEventDto.cs` | ЛР5 Рекомендація 5 |
| `Api/Program.cs` (minimal API) | ЛР4 фрагмент 4 |
| `Infrastructure/Scheduling/TimerBasedSceneScheduler.cs` (BackgroundService) | замість ЛР4 фрагмент 3 |
| `Infrastructure/Mqtt/ConsoleMqttPublisher.cs` | mock замість MQTTnet |
