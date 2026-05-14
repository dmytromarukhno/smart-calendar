# Календар для розумного будинку (Smart Home Calendar)

[![CI](https://github.com/dmytro-marukhnо/smart-calendar/actions/workflows/ci.yml/badge.svg)](https://github.com/dmytro-marukhnо/smart-calendar/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

> Навчальний проект з дисципліни «Основи програмної інженерії», ВНТУ, 2026.
> Автор: ст. 6ПІ-24б Марухно Дмитро.

## Зміст

- [Опис продукту](#опис-продукту)
- [Можливості поточного інкременту](#можливості-поточного-інкременту)
- [Скріншоти](#скріншоти)
- [Тех-стек](#тех-стек)
- [Архітектура](#архітектура)
- [Швидкий старт](#швидкий-старт)
- [API Endpoints](#api-endpoints)
- [Тестування](#тестування)
- [Структура репозиторію](#структура-репозиторію)
- [Roadmap](#roadmap)
- [Документація](#документація)
- [Ліцензія](#ліцензія)

---

## Опис продукту

**«Календар для розумного будинку»** — інформаційна система, що поєднує функції планувальника подій з автоматизованим керуванням побутовими IoT-пристроями (освітленням, кліматом, шторами, охоронною сигналізацією) за розкладом, заданим у календарі.

Користувач створює подію (наприклад, «Кіно 21:00») та прив'язує до неї сцену автоматизації — набір команд для пристроїв (приглушити світло, опустити штори, увімкнути телевізор у режимі «кіно»). За заданий offset до початку події фоновий планувальник автоматично виконує сцену, публікуючи команди через MQTT.

**Особливості:**
- робота у режимі офлайн з локальною базою даних (SQLite)
- мінімалістичний Blazor Server інтерфейс без зайніх залежностей
- IoT-публікація через mock `ConsoleMqttPublisher` (заміна на реальний MQTT — питання одного класу)
- орієнтація на приватність — усі дані зберігаються локально

**Цільова аудиторія:** користувачі розумного будинку, які хочуть автоматизувати свої щоденні ритуали без ручного перемикання кожного пристрою.

---

## Можливості поточного інкременту

| Функція | Статус |
|---|---|
| CRUD-операції з подіями календаря | ✅ |
| Місячний візуальний календар (Blazor Server) | ✅ |
| Налаштування нагадувань (offset у хвилинах) | ✅ |
| Управління сценаріями автоматизації | ✅ |
| Прив'язка сцени до події з налаштуванням offset | ✅ |
| Автоматичне спрацьовування сцени (`BackgroundService`) | ✅ |
| Mock IoT-публікація (`ConsoleMqttPublisher`) | ✅ (mock) |
| REST API + Swagger UI | ✅ |
| Sidebar з 5 mock-пристроями та їх статусом | ✅ |
| Модульні та інтеграційні тести (28+) | ✅ |
| GitHub Actions CI | ✅ |

---

## Скріншоти

> Реальні скріни автор додасть після фінального розгортання.

| Екран | Опис |
|---|---|
| ![Calendar](docs/screenshots/calendar.png) | Місячна сітка календаря з подіями |
| ![Event Editor](docs/screenshots/event-editor.png) | Модальне вікно створення/редагування події |
| ![Scheduler Log](docs/screenshots/scheduler-log.png) | Лог планувальника з MQTT-публікаціями |

---

## Тех-стек

| Призначення | Технологія |
|---|---|
| Платформа | .NET 8 |
| Мова | C# 12 (nullable reference types) |
| Веб-сервер та API | ASP.NET Core Web API (Kestrel) |
| UI | Blazor Server |
| ORM | Entity Framework Core 8 |
| База даних | SQLite (`smart-calendar.db`) |
| Планувальник | `BackgroundService` + `PeriodicTimer` |
| IoT-publisher | `ConsoleMqttPublisher` (mock → консоль) |
| Логування | Serilog → консоль |
| Тестування | xUnit + Moq + FluentAssertions |
| API-документація | Swashbuckle / OpenAPI (Swagger UI) |
| CI | GitHub Actions |

---

## Архітектура

Проект побудований за принципами **Clean Architecture** з чотирма шарами:

```
┌─────────────────────────────────────────────────────────────┐
│  SmartCalendar.Api  (Blazor Server + Minimal API + Swagger) │
├─────────────────────────────────────────────────────────────┤
│  SmartCalendar.Application  (сервіси, DTO, інтерфейси)      │
├──────────────────────────┬──────────────────────────────────┤
│  SmartCalendar.Domain    │  SmartCalendar.Infrastructure    │
│  (сутності, винятки,     │  (EF Core, SQLite, ConsoleMQTT,  │
│   інтерфейси репоз.)     │   BackgroundService, SeedData)   │
└──────────────────────────┴──────────────────────────────────┘
```

Бізнес-логіка повністю незалежна від інфраструктури через інтерфейси (`IMqttPublisher`, `ISceneRepository`, `IClock`). Заміна SQLite → PostgreSQL або mock-MQTT → реальний MQTTnet — питання одного класу у шарі Infrastructure.

---

## Швидкий старт

### Передумови

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Git

### Кроки

```bash
# 1. Клонуйте репозиторій
git clone https://github.com/dmytro-marukhnо/smart-calendar.git
cd smart-calendar

# 2. Відновіть залежності
dotnet restore

# 3. Застосуйте міграції бази даних (БД створюється автоматично при першому запуску,
#    але явна міграція рекомендована для production-подібного старту)
dotnet ef database update \
  --project src/SmartCalendar.Infrastructure \
  --startup-project src/SmartCalendar.Api

# 4. Запустіть застосунок
dotnet run --project src/SmartCalendar.Api
```

Відкрийте у браузері: **http://localhost:5123**

Swagger UI доступний за: **http://localhost:5123/swagger**

> При першому запуску автоматично сідуються 5 mock-пристроїв та demo-сцена «Кіно» з командою `dim` для світла вітальні.

---

## API Endpoints

Базовий URL: `http://localhost:5123`

### Events

| Метод | Маршрут | Опис |
|---|---|---|
| `GET` | `/api/events` | Список усіх подій (з фільтром `?from=&to=`) |
| `GET` | `/api/events/{id}` | Подія за ID |
| `POST` | `/api/events` | Створити подію |
| `PUT` | `/api/events/{id}` | Оновити подію |
| `DELETE` | `/api/events/{id}` | Видалити подію |
| `POST` | `/api/events/{eventId}/scenes/{sceneId}` | Прив'язати сцену до події (`?offsetMinutes=N`) |

### Scenes

| Метод | Маршрут | Опис |
|---|---|---|
| `GET` | `/api/scenes` | Список усіх сцен (з кількістю команд) |
| `POST` | `/api/scenes` | Створити сцену |

### Devices

| Метод | Маршрут | Опис |
|---|---|---|
| `GET` | `/api/devices` | Список усіх пристроїв |

Повна документація з прикладами запитів/відповідей — у **Swagger UI** (`/swagger`).

---

## Тестування

```bash
# Усі тести
dotnet test

# З детальним виводом
dotnet test --verbosity normal

# З покриттям коду
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

**Поточний стан:** 28 тестів, 0 failed.

Структура тестів:

| Категорія | Файли | Що перевіряється |
|---|---|---|
| Domain | `EventTests`, `SceneTests`, `ReminderTests` | Конструктор-валідація, бізнес-логіка |
| Application | `EventCommandServiceTests`, `SceneExecutorTests`, `ReminderDispatcherTests` | Use case-сервіси з Moq |
| Scheduler | `SceneSchedulerTests` | GetDueAsync: due/future/already-triggered |
| Integration | `EventsApiTests`, `DueSchedulerIntegrationTest` | WebApplicationFactory + real SQLite + capturing MQTT |

---

## Структура репозиторію

```
smart-calendar/
├── src/
│   ├── SmartCalendar.Domain/          # POCO-сутності, інтерфейси, винятки
│   ├── SmartCalendar.Application/     # use case-сервіси, DTO, IClock
│   ├── SmartCalendar.Infrastructure/  # EF Core, SQLite, ConsoleMqtt, Scheduler
│   ├── SmartCalendar.Api/             # ASP.NET Core + Blazor Server UI
│   └── SmartCalendar.Tests/           # xUnit, Moq, FluentAssertions
├── docs/
│   ├── screenshots/                   # скріншоти UI (додати після розгортання)
│   └── labs/                          # звіти ЛР1-ЛР5 (.docx)
├── .github/
│   └── workflows/
│       └── ci.yml                     # GitHub Actions: build + test
├── CHANGELOG.md
├── LICENSE
└── SmartCalendar.sln
```

---

## Roadmap

Заплановані інкременти після MVP (з матеріалів лабораторних робіт):

- [ ] Заміна `ConsoleMqttPublisher` на реальний MQTT-клієнт (MQTTnet + Mosquitto)
- [ ] Перехід з SQLite на PostgreSQL у Docker
- [ ] Інтеграція з Home Assistant REST API
- [ ] Підтримка протоколу Matter
- [ ] .NET MAUI Blazor Hybrid клієнт для Android/iOS
- [ ] Real-time push-нагадування через SignalR
- [ ] JWT-авторизація та керування ролями (Owner / Member / Guest / Admin)
- [ ] Складніші розклади повторюваних подій через Quartz.NET
- [ ] SonarCloud аналіз якості коду у CI

---

## Документація

| Документ | Тема |
|---|---|
| `docs/labs/` | Звіти ЛР1-ЛР5 |
| `SPEC.md` | Функціональна специфікація, use cases, доменна модель |
| `IMPLEMENTATION_SCOPE.md` | Таблиця: що реалізовано / що mock / що відкладено |
| `CLAUDE.md` | Контекст проекту для Claude Code |
| `CHANGELOG.md` | Журнал змін за версіями |

---

## Ліцензія

[MIT](LICENSE) © 2026 Марухно Дмитро
