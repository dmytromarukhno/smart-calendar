# IMPLEMENTATION_SCOPE.md — що реальне, що mock, що відкладено

> Звіти ЛР1-ЛР5 описують повну заплановану архітектуру системи «Календар для розумного будинку». Поточний інкремент реалізує **підмножину** цієї архітектури — обсяг, реалістичний для одного розробника за 2-3 тижні. Ця таблиця фіксує точну відповідність.

## Загальна таблиця відповідності

| Аспект | Звіт (запланована архітектура) | Поточний інкремент (реалізація) | Статус |
|---|---|---|---|
| Платформа | .NET 8 | .NET 8 | ✅ як у звіті |
| Мова | C# 12 | C# 12 | ✅ як у звіті |
| Серверна частина | ASP.NET Core Web API | ASP.NET Core Web API | ✅ як у звіті |
| Клієнт | .NET MAUI Blazor Hybrid (Android, iOS, Windows) | **Blazor Server** (веб-сторінка у тому самому процесі) | 🔄 спрощено |
| Веб-портал | Blazor Server | Blazor Server | ✅ як у звіті |
| ORM | Entity Framework Core 8 | Entity Framework Core 8 | ✅ як у звіті |
| База клієнта | SQLite | — (немає окремого клієнта) | ⏸ відкладено |
| База сервера | PostgreSQL у Docker | **SQLite-файл** | 🔄 спрощено |
| Брокер IoT | Mosquitto MQTT-брокер у Docker | **`ConsoleMqttPublisher` — mock у консоль** | 🎭 mock |
| MQTT-клієнт | MQTTnet | — (не потрібен для mock) | ⏸ відкладено |
| Планувальник | Quartz.NET з job-ами | **`BackgroundService` + `System.Threading.Timer`** | 🔄 спрощено |
| Real-time push | SignalR Hub | `Console.WriteLine` при спрацьовуванні | 🎭 mock |
| Зовнішні екосистеми | Home Assistant REST API, Matter SDK | — | ⏸ відкладено |
| Auth | JWT + ASP.NET Core Identity | Single mock-user або базова форма | 🔄 спрощено |
| Логування | Serilog + Seq | Serilog → консоль | 🔄 спрощено |
| Контейнеризація | Docker + docker-compose | Опціонально `Dockerfile`, без compose | 🔄 спрощено |
| CI/CD | GitHub Actions з SonarCloud | GitHub Actions з build + test | 🔄 спрощено |
| Тести | xUnit + Moq + FluentAssertions | xUnit + Moq + FluentAssertions | ✅ як у звіті |
| Покриття тестами | ≥ 70 % | ≥ 70 % (ціль) | ✅ ціль |

## Як це обґрунтовується на захисті

> «Архітектура спроектована повна — клієнт MAUI, MQTT-брокер, Quartz, SignalR, PostgreSQL, інтеграція з Home Assistant. У поточному MVP-інкременті реалізовано доменне ядро, сервіси, REST API та веб-інтерфейс на Blazor Server. IoT-публікації мокуються через `ConsoleMqttPublisher`, що дозволяє продемонструвати поведінку без реального MQTT-брокера. Перехід на реальний MQTT, Quartz, MAUI та PostgreSQL — питання заміни одного класу інфраструктурного шару, бо вся бізнес-логіка вже працює через інтерфейси (`IMqttPublisher`, `ISceneRepository` тощо)».

Це **чесна** позиція: ви демонструєте розуміння архітектури та реалізацію, що покриває ключові use case-и системи.

## Що буде у демонстрації

1. Запуск через `dotnet run` → відкриття `https://localhost:5001` у браузері.
2. Сторінка з місячним календарем, ліва панель з 5 mock-пристроями.
3. Кнопка «+ Подія» → форма → подія з'являється на сітці.
4. Прив'язка сцени до події (форма редагування з ЛР4 § 8, рисунок 4).
5. При настанні часу + offset до події у консолі видно:
   ```
   [MQTT-mock] smart/livingroom/light → {"action":"dim","value":"20"}
   [MQTT-mock] smart/livingroom/curtain → {"action":"close"}
   [MQTT-mock] smart/livingroom/av → {"action":"cinemaMode"}
   ```
6. Swagger UI на `/swagger` показує REST-API.
7. `dotnet test` → 70%+ покриття.

## Послідовність робіт

| Спринт | Тривалість | Артефакти |
|---|---|---|
| Sprint 1 | 3 дні | Solution + Domain-шар (Event, Reminder, Scene, Command, Schedule, Device, User, винятки, константи). Тести на доменну логіку. |
| Sprint 2 | 2 дні | Infrastructure: `EFCoreDbContext`, репозиторії, `ConsoleMqttPublisher`. Міграції EF. |
| Sprint 3 | 2 дні | Application: `EventCommandService`, `EventQueryService`, `SceneExecutor` (рефакторений), `ReminderDispatcher`. Тести з Moq. |
| Sprint 4 | 2 дні | API: контролери (або minimal API), Swagger, базова auth. |
| Sprint 5 | 3 дні | Blazor Server: сторінка календаря, форма події, форма сцени, ліва панель пристроїв. |
| Sprint 6 | 1 день | `TimerBasedSceneScheduler` як `BackgroundService`. Тест: створив подію → за хвилину побачив команди у консолі. |
| Sprint 7 | 1 день | Polish: README, Dockerfile, GitHub Actions workflow, фінальні скріни. |
