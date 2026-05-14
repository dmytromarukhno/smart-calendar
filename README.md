# Календар для розумного будинку (Smart Home Calendar)

Інформаційна система, що поєднує функції планувальника подій з автоматизованим керуванням побутовими IoT-пристроями за розкладом, заданим у календарі.

> Навчальний проект з дисципліни «Основи програмної інженерії», ВНТУ, 2026.
> Автор: ст. 6ПІ-24б Марухно Дмитро.

## Зміст

- [Опис](#опис)
- [Можливості поточного інкременту](#можливості-поточного-інкременту)
- [Архітектура](#архітектура)
- [Тех-стек](#тех-стек)
- [Швидкий старт](#швидкий-старт)
- [Структура репозиторію](#структура-репозиторію)
- [Тестування](#тестування)
- [Roadmap](#roadmap)
- [Документація](#документація)
- [Ліцензія](#ліцензія)

## Опис

Користувач створює подію у календарі (наприклад, «Кіно 21:00») та прив'язує до неї сцену автоматизації — набір команд для IoT-пристроїв (приглушити світло, опустити штори, увімкнути ТВ у режимі «кіно»). За хвилину до початку події система автоматично виконує сцену.

## Можливості поточного інкременту

- ✅ CRUD-операції з подіями календаря
- ✅ Місячний візуальний календар (Blazor Server)
- ✅ Налаштування нагадувань (offset у хвилинах до події)
- ✅ Управління сценаріями автоматизації
- ✅ Прив'язка сценарію до події
- ✅ Автоматичне спрацьовування сценарію за `BackgroundService`-таймером
- ✅ Mock IoT-публікація через `ConsoleMqttPublisher`
- ✅ REST API + Swagger UI
- ✅ Модульні та інтеграційні тести (xUnit)

## Архітектура

Проект побудований за принципами **Clean Architecture** з п'ятьма шарами:

```
┌────────────────────────────────────────────────┐
│  SmartCalendar.Api  (Blazor Server + Web API)  │
├────────────────────────────────────────────────┤
│  SmartCalendar.Application  (use cases)        │
├──────────────────────┬─────────────────────────┤
│  SmartCalendar.      │  SmartCalendar.         │
│  Domain  (entities)  │  Infrastructure         │
│                      │  (EF Core, Console MQTT)│
└──────────────────────┴─────────────────────────┘
```

Детальна архітектура — у `docs/lab4-architecture.md`.

## Тех-стек

- **.NET 8** + **C# 12** з увімкненими nullable reference types
- **ASP.NET Core Web API** (Kestrel)
- **Blazor Server** для UI
- **Entity Framework Core 8** + **SQLite**
- **`BackgroundService`** з `System.Threading.Timer` як планувальник
- **Serilog** для логування
- **xUnit + Moq + FluentAssertions** для тестування
- **Swashbuckle.AspNetCore** для OpenAPI/Swagger

## Швидкий старт

### Передумови

- .NET 8 SDK
- Git
- Bash або PowerShell

### Кроки

```bash
# 1. Клонуйте репозиторій
git clone https://github.com/<your-username>/smart-calendar.git
cd smart-calendar

# 2. Відновіть залежності
dotnet restore

# 3. Застосуйте міграції бази даних
dotnet ef database update --project src/SmartCalendar.Infrastructure --startup-project src/SmartCalendar.Api

# 4. Запустіть застосунок
dotnet run --project src/SmartCalendar.Api
```

Відкрийте у браузері `https://localhost:5001`. Swagger UI доступний за `/swagger`.

## Структура репозиторію

```
smart-calendar/
├── src/
│   ├── SmartCalendar.Domain/          # POCO-сутності, інтерфейси
│   ├── SmartCalendar.Application/     # use case-сервіси
│   ├── SmartCalendar.Infrastructure/  # EF Core, MQTT mock
│   ├── SmartCalendar.Api/             # ASP.NET Core + Blazor Server
│   └── SmartCalendar.Tests/           # xUnit-тести
├── docs/                              # звіти ЛР1-ЛР5
├── .github/workflows/                 # CI/CD
└── SmartCalendar.sln
```

## Тестування

```bash
# Усі тести
dotnet test

# З покриттям
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# Звіт у HTML
reportgenerator -reports:**/coverage.opencover.xml -targetdir:coverage-report
```

Поточне покриття: **71.8 %** (ціль ≥ 70 %).

## Roadmap

Заплановані інкременти (за матеріалами лабораторних робіт):

- [ ] Заміна `ConsoleMqttPublisher` на реальний MQTT-клієнт (MQTTnet + Mosquitto)
- [ ] Перехід з SQLite на PostgreSQL
- [ ] Інтеграція з Home Assistant REST API
- [ ] Підтримка протоколу Matter
- [ ] .NET MAUI Blazor Hybrid клієнт для Android/iOS
- [ ] Real-time push-нагадування через SignalR
- [ ] JWT-авторизація та керування ролями (Owner/Member/Guest/Admin)
- [ ] Інтеграція з Quartz.NET для складніших розкладів

## Документація

| Документ | Тема |
|---|---|
| `docs/lab1-environment.md` | Середовище розробки, життєвий цикл |
| `docs/lab2-zachman.md` | Карта Захмана, методологія Scrum |
| `docs/lab3-uml.md` | UML-діаграми: use case, classes, sequence |
| `docs/lab4-architecture.md` | Magic Quadrant, архітектура, технології |
| `docs/lab5-clean-code.md` | Аналіз чистоти коду за Р. Мартином |

## Ліцензія

MIT License — див. `LICENSE`.
