# Changelog - HotelsApp

Все значимые изменения в проекте документируются в этом файле.

## [Неопубликовано] - 2024-12-XX

### Добавлено

#### Web API - Исправление ошибки 403.14
- **HomeController** (`HotelsApp.API/Controllers/HomeController.cs`)
  - Добавлен новый контроллер для обработки корневого URL API
  - Реализован метод `Get()`, возвращающий информацию об API и доступных эндпоинтах
  - Использует атрибутную маршрутизацию с `[Route("~/")]` для обработки корневого URL
  - Возвращает JSON с информацией о версии API и списком доступных эндпоинтов

#### Конфигурация Web API
- **WebApiConfig.cs** (`HotelsApp.API/App_Start/WebApiConfig.cs`)
  - Обновлена конфигурация маршрутизации
  - Атрибутная маршрутизация (`MapHttpAttributeRoutes()`) вызывается первой для правильной обработки корневого URL

- **Web.config** (`HotelsApp.API/Web.config`)
  - Добавлена настройка `runAllManagedModulesForAllRequests="true"` для корректной обработки extensionless URL
  - Отключен просмотр каталогов (`directoryBrowse enabled="false"`)
  - Очищены документы по умолчанию для использования маршрутизации API

#### Проект
- **HotelsApp.API.csproj**
  - Добавлен `HomeController.cs` в список компилируемых файлов проекта
  - Исправлена проблема, когда контроллер не компилировался из-за отсутствия в файле проекта

### Исправлено

#### Критические ошибки
- **HTTP Error 403.14 - Forbidden**
  - Исправлена ошибка при обращении к корневому URL (`http://localhost:8080/`)
  - Теперь корневой URL корректно обрабатывается и возвращает информацию об API
  - Устранена проблема "No type was found that matches the controller named 'Home'"

### Изменено

#### Структура API
- Корневой URL (`/`) теперь доступен и возвращает JSON с информацией об API
- Все эндпоинты API остаются доступными через префикс `/api/`

---

## Предыдущие изменения

### Добавлено

#### Web API Проект
- **HotelsApp.API** - новый проект Web API
  - Структура контроллеров (HotelsController, RoomsController, BookingsController)
  - DTOs для всех основных сущностей
  - ExceptionFilter для обработки ошибок
  - Настройка CORS для кросс-доменных запросов
  - Конфигурация маршрутизации и форматирования JSON

#### Документация
- `API_REQUIREMENTS.md` - требования к API
- `API_SUMMARY.md` - краткое описание API
- `API/README.md` - документация по API
- `API/INSTALLATION.md` - инструкции по установке
- `PROJECT_STATUS.md` - статус проекта
- `QUICK_START.md` - быстрый старт
- `CHECKLIST.md` - чеклист задач
- `ADD_PROJECT_TO_SOLUTION.md` - инструкции по добавлению проекта
- `FIX_REFERENCE.md` - инструкции по исправлению ссылок

#### Services
- `BookingService.cs` - сервис для работы с бронированиями
- `HotelService.cs` - сервис для работы с отелями
- `SecurityService.cs` - сервис безопасности
- `MapService.cs` - сервис для работы с картами
- `LoggingService.cs` - сервис логирования
- `NotificationService.cs` - сервис уведомлений

#### Тесты
- `BookingServiceTests.cs` - тесты для BookingService
- `HotelServiceTests.cs` - тесты для HotelService
- `SecurityServiceTests.cs` - тесты для SecurityService
- `MapServiceTests.cs` - тесты для MapService
- `LoggingServiceTests.cs` - тесты для LoggingService

### Исправлено

#### Модели данных
- Исправлены все использования `MaxOccupancy` → `Capacity` в DTOs и контроллерах
- Исправлена работа с nullable типами в сервисах

#### Зависимости
- Добавлены ссылки на необходимые NuGet пакеты
- Исправлены ссылки между проектами HotelsApp и HotelsApp.API
- Заменен `System.Text.Json` на `Newtonsoft.Json` для совместимости с .NET Framework 4.8

#### Совместимость
- Исправлены проблемы совместимости с C# 7.3
- Заменены switch expressions на if-else конструкции
- Исправлена работа с nullable типами

---

## Формат версионирования

Проект использует [Semantic Versioning](https://semver.org/):
- **MAJOR** - несовместимые изменения API
- **MINOR** - новая функциональность с обратной совместимостью
- **PATCH** - исправления ошибок с обратной совместимостью

---

## Примечания

- Все даты в формате YYYY-MM-DD
- Изменения группируются по категориям: Добавлено, Изменено, Исправлено, Удалено
- Критические изменения и breaking changes выделяются отдельно
