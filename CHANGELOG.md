# Changelog

Усі значні зміни цього проекту фіксуються тут.
Формат базується на [Keep a Changelog](https://keepachangelog.com/uk/1.0.0/).

## [Unreleased]

### Заплановано (Roadmap)
- Заміна `ConsoleMqttPublisher` на реальний MQTT-клієнт (MQTTnet + Mosquitto)
- Перехід з SQLite на PostgreSQL
- Інтеграція з Home Assistant REST API
- Підтримка протоколу Matter
- .NET MAUI Blazor Hybrid клієнт для Android/iOS
- Real-time push-нагадування через SignalR
- JWT-авторизація та керування ролями (Owner / Member / Guest / Admin)
- Складніші розклади повторюваних подій через Quartz.NET

## [0.1.0] — 2026-05-14

### MVP: Calendar UI + Scenes + BackgroundService Scheduler

#### Додано
- **Domain-шар:** сутності `Event`, `Reminder`, `Scene`, `Command`, `Schedule`, `Device`; доменні винятки; `MqttCommandPayload` value object; `ReminderDefaults` константи
- **Infrastructure:** `SmartCalendarDbContext` (EF Core 8 + SQLite); репозиторії `EfEventRepository`, `EfSceneRepository`, `EfDeviceRepository`, `EfScheduleRepository`; EF-міграції; `ConsoleMqttPublisher` (mock IoT); `SeedData` (5 mock-пристроїв + demo-сцена «Кіно»)
- **Application:** `EventCommandService`, `EventQueryService`, `SceneExecutor`, `ReminderDispatcher`; IClock + SystemClock для тестованого часу; `SchedulerOptions` (IOptions)
- **API:** minimal API-ендпоінти (Events CRUD, Scenes, Devices); Swagger / OpenAPI
- **Blazor Server UI:** місячна сітка календаря; sidebar з пристроями та сценами; модальне вікно `EventEditorModal` (Title, Start/End, IsRecurring, нагадування, сцена)
- **BackgroundService:** `TimerBasedSceneScheduler` з `PeriodicTimer`; demo-friendly логування
- **Тести:** 28 тестів — домен, application (з Moq), integration (WebApplicationFactory + real SQLite), scheduler unit tests; покриття ≥ 70 %
- **CI:** GitHub Actions workflow (build + test на ubuntu-latest)
- **Polish:** README, LICENSE (MIT), CHANGELOG, docs/screenshots placeholder

[Unreleased]: https://github.com/dmytro-marukhnо/smart-calendar/compare/v0.1.0...HEAD
[0.1.0]: https://github.com/dmytro-marukhnó/smart-calendar/releases/tag/v0.1.0
