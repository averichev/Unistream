# App (.NET 8) Clean Architecture Template

Этот репозиторий содержит минимальный каркас многоуровневого приложения на .NET 8, демонстрирующий полный путь запроса от Web API до базы данных SQL Server через доменный слой. Проект предназначен как основа для CRUD-сервисов и включает готовую инфраструктуру для локального запуска и запуска в Docker.

## Структура решений

```
App.sln
├── src
│   ├── App.Domain    # Доменная модель, ошибки и сервисы
│   ├── App.Data      # Работа с EF Core и реализация репозиториев
│   └── App.WebApi    # ASP.NET Core Web API, DI, контроллеры, логирование
└── tests
    ├── App.Domain.Tests        # Unit-тесты доменного слоя
    └── App.IntegrationTests    # Интеграционный CRUD сценарий
```

## Основные возможности

- **Entity:** `Item { Id, Name, Price, CreatedAt }`.
- **Бизнес-правила:** проверки имени и цены, защита от дубликатов.
- **API:** CRUD-эндпойнты `/api/v1/items` + `/health`.
- **Инфраструктура:** EF Core (SQL Server), миграции, автоматическое применение миграций при старте.
- **Валидация:** FluentValidation на уровне DTO + доменная валидация.
- **Логирование:** Serilog (консоль + rolling-файлы).
- **Health Checks:** проверка состояния БД.
- **Тесты:** 5 unit-кейсов на доменную валидацию и интеграционный CRUD-тест на SQLite in-memory.

## Предварительные требования

- .NET SDK 8.0+
- Docker и Docker Compose (для контейнерного запуска)
- SQL Server (локально или в контейнере) — при локальном запуске можно использовать Docker

## Конфигурация

Основная строка подключения находится в `src/App.WebApi/appsettings.json` и может быть переопределена переменной окружения `ConnectionStrings__Default`.

Пример `.env` для Docker:

```
SA_PASSWORD=Your_password123
API_PORT=8080
MSSQL_PORT=1433
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__Default=Server=mssql,1433;Database=AppDb;User Id=sa;Password=Your_password123;TrustServerCertificate=True;
```

Скопируйте файл `.env.example` в `.env` и при необходимости измените значения.

## Локальный запуск

```bash
dotnet restore
dotnet ef database update --project src/App.Data --startup-project src/App.WebApi
dotnet run --project src/App.WebApi
```

После запуска API доступно по адресу `http://localhost:5000` (порт зависит от настроек `ASPNETCORE_URLS`).

### Примеры запросов

```bash
# Создать элемент
curl -X POST http://localhost:5000/api/v1/items \
  -H "Content-Type: application/json" \
  -d '{"name":"Notebook","price":199.99}'

# Получить элемент
curl http://localhost:5000/api/v1/items/{id}

# Обновить элемент
curl -X PUT http://localhost:5000/api/v1/items/{id} \
  -H "Content-Type: application/json" \
  -d '{"name":"Notebook 2","price":249.99}'

# Удалить элемент
curl -X DELETE http://localhost:5000/api/v1/items/{id}

# Health check
curl http://localhost:5000/health
```

## Запуск в Docker

```bash
cp .env.example .env
# при необходимости обновите пароли и порты
docker compose up --build
```

Docker Compose поднимает два сервиса:

- `mssql` — SQL Server 2022 с healthcheck.
- `api` — Web API, стартует после готовности БД и автоматически применяет миграции.

API будет доступно по адресу `http://localhost:${API_PORT}`.

## Тестирование

```bash
dotnet test
```

Тесты покрывают доменную валидацию и полный CRUD-сценарий через HTTP.

## Слои и зависимости

- `App.WebApi` зависит только от `App.Domain` (через сервисы) и `App.Data` (через DI).
- `App.Data` реализует `IItemRepository` и не знает о Web API.
- `App.Domain` содержит чистые модели и бизнес-логику без привязки к инфраструктуре.

Такой подход упрощает замену инфраструктуры, модульное тестирование и поддержку проекта.
