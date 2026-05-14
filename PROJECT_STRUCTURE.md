# PROJECT_STRUCTURE.md — структура папок і файлів

> Точна структура solution-а, який Claude Code має створити з нуля.

## Кореневі файли

```
smart-calendar/
├── .editorconfig
├── .gitignore
├── README.md                            # для GitHub
├── LICENSE                              # MIT
├── SmartCalendar.sln
├── global.json                          # фіксує SDK 8.0
├── Directory.Build.props                # спільні налаштування для всіх проектів
├── Dockerfile                           # опціонально
├── docs/
│   ├── lab1-environment.md
│   ├── lab2-zachman.md
│   ├── lab3-uml.md
│   ├── lab4-architecture.md
│   └── lab5-clean-code.md
├── .github/
│   └── workflows/
│       └── ci.yml                       # build + test
└── src/
    ├── SmartCalendar.Domain/
    ├── SmartCalendar.Application/
    ├── SmartCalendar.Infrastructure/
    ├── SmartCalendar.Api/
    └── SmartCalendar.Tests/
```

## SmartCalendar.Domain

```
SmartCalendar.Domain/
├── SmartCalendar.Domain.csproj          # net8.0, Nullable enable
├── Entities/
│   ├── User.cs
│   ├── UserRole.cs                      # enum
│   ├── Event.cs                         # код з CODE_FRAGMENTS.md § 1
│   ├── Reminder.cs
│   ├── Scene.cs
│   ├── Command.cs
│   ├── Schedule.cs
│   └── Device.cs
├── Constants/
│   └── ReminderDefaults.cs              # код з CODE_FRAGMENTS.md
├── Exceptions/
│   ├── DomainException.cs               # абстрактний базовий
│   ├── SceneNotFoundException.cs
│   ├── DeviceNotFoundException.cs
│   └── DeviceOfflineException.cs
├── Interfaces/
│   ├── IEventRepository.cs
│   ├── ISceneRepository.cs
│   ├── IDeviceRepository.cs
│   └── IUnitOfWork.cs
└── ValueObjects/
    └── MqttCommandPayload.cs            # record
```

**NuGet:** жодних залежностей (POCO-only).

## SmartCalendar.Application

```
SmartCalendar.Application/
├── SmartCalendar.Application.csproj
├── Services/
│   ├── EventCommandService.cs           # ЛР5 SRP розбиття
│   ├── EventQueryService.cs
│   ├── SceneExecutor.cs                 # рефакторений варіант (ЛР5 Рек.1)
│   └── ReminderDispatcher.cs
├── Dtos/
│   ├── CreateEventDto.cs                # record
│   ├── UpdateEventDto.cs                # record, ЛР5 Рек.5
│   ├── EventDto.cs
│   └── SceneDto.cs
├── Interfaces/
│   ├── IMqttPublisher.cs
│   └── ISceneScheduler.cs
└── DependencyInjection.cs               # AddApplication extension
```

**NuGet:** Microsoft.Extensions.DependencyInjection.Abstractions, Microsoft.Extensions.Logging.Abstractions.
**Залежності проектів:** Domain.

## SmartCalendar.Infrastructure

```
SmartCalendar.Infrastructure/
├── SmartCalendar.Infrastructure.csproj
├── Persistence/
│   ├── SmartCalendarDbContext.cs
│   ├── Configurations/
│   │   ├── EventConfiguration.cs        # IEntityTypeConfiguration<Event>
│   │   ├── SceneConfiguration.cs
│   │   └── DeviceConfiguration.cs
│   ├── Repositories/
│   │   ├── EfEventRepository.cs
│   │   ├── EfSceneRepository.cs
│   │   └── EfDeviceRepository.cs
│   ├── Migrations/                      # генеруються dotnet ef
│   └── SeedData.cs                      # 5 mock-пристроїв при першому запуску
├── Mqtt/
│   └── ConsoleMqttPublisher.cs          # пише в Console.WriteLine
├── Scheduling/
│   └── TimerBasedSceneScheduler.cs      # BackgroundService
└── DependencyInjection.cs               # AddInfrastructure extension
```

**NuGet:** Microsoft.EntityFrameworkCore, Microsoft.EntityFrameworkCore.Sqlite, Microsoft.EntityFrameworkCore.Design (для tools), Microsoft.Extensions.Hosting (для BackgroundService).
**Залежності проектів:** Domain, Application.

## SmartCalendar.Api

```
SmartCalendar.Api/
├── SmartCalendar.Api.csproj
├── Program.cs                           # ASP.NET Core + Blazor Server + minimal API
├── appsettings.json
├── appsettings.Development.json
├── Components/
│   ├── App.razor                        # Blazor App
│   ├── Routes.razor
│   ├── _Imports.razor
│   ├── Layout/
│   │   ├── MainLayout.razor
│   │   └── NavMenu.razor
│   └── Pages/
│       ├── Home.razor                   # головна
│       ├── Calendar.razor               # місячний календар (UI з ЛР4 рис.3)
│       ├── EventEditor.razor            # форма події (UI з ЛР4 рис.4)
│       └── Scenes.razor                 # список сцен
└── wwwroot/
    └── css/
        └── site.css
```

**NuGet:** Microsoft.AspNetCore.App (вбудовано), Swashbuckle.AspNetCore (Swagger), Serilog.AspNetCore, Serilog.Sinks.Console.
**Залежності проектів:** Application, Infrastructure.

## SmartCalendar.Tests

```
SmartCalendar.Tests/
├── SmartCalendar.Tests.csproj
├── Domain/
│   ├── EventTests.cs                    # AddReminder, IsActiveAt, конструктор-валідація
│   ├── SceneTests.cs
│   └── ReminderTests.cs
├── Application/
│   ├── EventCommandServiceTests.cs      # з Moq для IEventRepository
│   ├── SceneExecutorTests.cs            # з Moq для IMqttPublisher, ISceneRepository
│   └── ReminderDispatcherTests.cs
├── Integration/
│   └── EventsApiTests.cs                # WebApplicationFactory
└── TestHelpers/
    └── EventFactory.cs                  # створює валідні Event для тестів
```

**NuGet:** xunit, xunit.runner.visualstudio, Microsoft.NET.Test.Sdk, Moq, FluentAssertions, Microsoft.AspNetCore.Mvc.Testing, Microsoft.EntityFrameworkCore.InMemory.
**Залежності проектів:** Domain, Application, Infrastructure, Api.

## Глобальні конфіги

### `.editorconfig` (з ЛР5)
```ini
root = true

[*]
indent_style = space
indent_size = 4
end_of_line = lf
charset = utf-8
trim_trailing_whitespace = true
insert_final_newline = true
max_line_length = 120

[*.cs]
dotnet_diagnostic.CA1062.severity = warning
dotnet_diagnostic.CA2007.severity = warning
csharp_style_var_for_built_in_types = true:suggestion
csharp_style_namespace_declarations = file_scoped:suggestion
```

### `global.json`
```json
{
  "sdk": {
    "version": "8.0.0",
    "rollForward": "latestMinor"
  }
}
```

### `Directory.Build.props`
```xml
<Project>
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <LangVersion>12</LangVersion>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
  </PropertyGroup>
</Project>
```
