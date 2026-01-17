# Требования для создания API для HotelsApp

## Обзор
Необходимо создать REST API для существующего WPF приложения HotelsApp, которое работает с базой данных отелей через Entity Framework 6.5.1.

## Технические требования

### 1. Технологический стек
- **.NET Framework 4.8** (совместимо с существующим проектом)
- **ASP.NET Web API 2** (совместим с .NET Framework 4.8)
- **Entity Framework 6.5.1** (уже используется в проекте)
- **Newtonsoft.Json** (уже установлен)

### 2. Структура проекта API

#### Вариант 1: Отдельный проект (рекомендуется)
Создать новый проект **ASP.NET Web API 2** в том же solution:
- `HotelsApp.API` - основной проект API
- Использовать общие модели из `HotelsApp.Model`
- Использовать общие сервисы из `HotelsApp.Services`

#### Вариант 2: Встроенный API
Добавить API контроллеры в существующий проект (не рекомендуется для WPF)

### 3. Необходимые компоненты

#### 3.1. NuGet пакеты
```
Microsoft.AspNet.WebApi (5.2.9 или выше)
Microsoft.AspNet.WebApi.Cors (5.2.9)
Microsoft.AspNet.WebApi.Owin (если нужна OWIN интеграция)
```

#### 3.2. Структура папок API проекта
```
HotelsApp.API/
├── Controllers/
│   ├── HotelsController.cs
│   ├── BookingsController.cs
│   ├── RoomsController.cs
│   ├── CustomersController.cs
│   ├── ReviewsController.cs
│   └── AuthController.cs
├── DTOs/
│   ├── HotelDTO.cs
│   ├── BookingDTO.cs
│   └── ...
├── Filters/
│   └── ExceptionFilter.cs
├── App_Start/
│   └── WebApiConfig.cs
├── Global.asax
└── Web.config
```

### 4. Исправления в существующем коде

#### Проблема: Несоответствие имен контекста
В сервисах используется `Entities`, но в контексте определен `ProHotelEntities`.

**Решение:**
- Вариант 1: Добавить using алиас в сервисах
- Вариант 2: Переименовать `ProHotelEntities` в `Entities` (требует изменения в edmx)
- Вариант 3: Исправить сервисы для использования `ProHotelEntities`

### 5. Основные endpoints

#### Hotels API
- `GET /api/hotels` - список отелей с фильтрацией
- `GET /api/hotels/{id}` - детали отеля
- `GET /api/hotels/search?cityId=1&minStars=3` - поиск отелей
- `GET /api/hotels/{id}/rooms` - номера отеля

#### Bookings API
- `GET /api/bookings` - список бронирований (требует авторизации)
- `GET /api/bookings/{id}` - детали бронирования
- `POST /api/bookings` - создание бронирования
- `PUT /api/bookings/{id}/cancel` - отмена бронирования

#### Rooms API
- `GET /api/rooms` - список номеров
- `GET /api/rooms/{id}` - детали номера
- `GET /api/rooms/available?checkIn=2024-01-01&checkOut=2024-01-05` - доступные номера

#### Auth API
- `POST /api/auth/login` - вход
- `POST /api/auth/register` - регистрация
- `POST /api/auth/logout` - выход

### 6. Конфигурация

#### Web.config
- Настройка connection string (скопировать из App.config)
- Настройка CORS (если нужен доступ из браузера)
- Настройка маршрутизации API

#### WebApiConfig.cs
- Настройка маршрутов
- Настройка JSON сериализации
- Настройка CORS
- Регистрация фильтров

### 7. Безопасность

- Аутентификация (JWT токены или сессии)
- Авторизация для защищенных endpoints
- Валидация входных данных
- Обработка ошибок

### 8. DTOs (Data Transfer Objects)

Создать DTOs для:
- Избежания циклических ссылок в JSON
- Контроля над данными, возвращаемыми клиенту
- Оптимизации размера ответов

### 9. Дополнительные возможности

- Swagger/OpenAPI документация
- Логирование запросов
- Кэширование (опционально)
- Пагинация для списков
- Фильтрация и сортировка

## Шаги реализации

1. ✅ Создать документацию (этот файл)
2. ⏳ Исправить несоответствие Entities/ProHotelEntities
3. ⏳ Создать структуру проекта API
4. ⏳ Настроить базовую конфигурацию
5. ⏳ Создать контроллеры
6. ⏳ Создать DTOs
7. ⏳ Настроить аутентификацию
8. ⏳ Тестирование

## Примечания

- API будет работать параллельно с WPF приложением
- Оба приложения будут использовать одну и ту же базу данных
- Необходимо обеспечить thread-safety при работе с Entity Framework контекстом

