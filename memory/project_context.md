---
name: Smart Calendar project context
description: Tech stack, constraints, and current state of the Smart Home Calendar project
type: project
---

Smart Home Calendar — an IoT calendar that executes automation scenes (lighting, climate, curtains) triggered by calendar events.

**Solution:** `SmartCalendar.sln` in repo root; 5 projects in `src/`.

**Tech stack (current increment):** .NET 8, C# 12, ASP.NET Core Web API + Blazor Server (same process), EF Core 8 + SQLite, BackgroundService+Timer, ConsoleMqttPublisher (mock), Serilog→Console, xUnit+Moq+FluentAssertions.

**Constraints (must NOT add):** MAUI, real MQTT broker, Home Assistant, PostgreSQL, Quartz.NET, SignalR.

**Project structure:**
- `SmartCalendar.Domain` — entities, exceptions, interfaces, constants (no NuGet deps)
- `SmartCalendar.Application` — services (EventCommandService, EventQueryService, SceneExecutor, ReminderDispatcher), DTOs
- `SmartCalendar.Infrastructure` — EF Core DbContext, repositories, ConsoleMqttPublisher, TimerBasedSceneScheduler
- `SmartCalendar.Api` — Program.cs (Blazor Server + minimal API), Razor components
- `SmartCalendar.Tests` — xUnit domain/application/integration tests

**Current state (Sprint 2 done):** Full skeleton + EF migration `20260514030728_Initial` created. `dotnet run` verified: DB auto-migrates, SeedData inserts 5 mock devices, Swagger available at `/swagger`. 8 tables: Commands, Devices, Events, Reminders, Scenes, Schedules, Users, __EFMigrationsHistory.

**Why:** `Directory.Build.props` has `NoWarn=CA2007;CA1062` — CA2007 (ConfigureAwait) not needed in ASP.NET Core; CA1062 (null check) redundant with Nullable enable.
**How to apply:** Next step is `dotnet ef migrations add Initial` in Infrastructure, then implement full features sprint by sprint per IMPLEMENTATION_SCOPE.md.
