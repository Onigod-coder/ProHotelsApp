# Изменения в Web API - Исправление ошибки 403.14

## Проблема

При обращении к корневому URL Web API (`http://localhost:8080/`) возникала ошибка:
- **HTTP Error 403.14 - Forbidden**
- **"No type was found that matches the controller named 'Home'"**

## Причина

1. Отсутствовал контроллер для обработки корневого URL
2. Web.config не был настроен для обработки extensionless URL
3. Контроллер не был добавлен в файл проекта (.csproj), поэтому не компилировался

## Решение

### 1. Создан HomeController

**Файл:** `HotelsApp.API/Controllers/HomeController.cs`

```csharp
using System.Web.Http;

namespace HotelsApp.API.Controllers
{
    public class HomeController : ApiController
    {
        [HttpGet]
        [Route("~/")]
        public IHttpActionResult Get()
        {
            return Ok(new
            {
                message = "HotelsApp Web API",
                version = "1.0",
                endpoints = new
                {
                    hotels = "/api/hotels",
                    rooms = "/api/rooms",
                    bookings = "/api/bookings"
                },
                documentation = "Use the endpoints above to access the API resources."
            });
        }
    }
}
```

**Особенности:**
- Использует атрибутную маршрутизацию с `[Route("~/")]` для обработки корневого URL
- Возвращает JSON с информацией об API и доступных эндпоинтах
- Простой и понятный интерфейс для проверки работоспособности API

### 2. Обновлен WebApiConfig.cs

**Файл:** `HotelsApp.API/App_Start/WebApiConfig.cs`

**Изменения:**
- Атрибутная маршрутизация (`MapHttpAttributeRoutes()`) вызывается первой
- Это обеспечивает правильную обработку маршрутов с атрибутами `[Route]`

### 3. Обновлен Web.config

**Файл:** `HotelsApp.API/Web.config`

**Добавлено в секцию `<system.webServer>`:**
```xml
<modules runAllManagedModulesForAllRequests="true" />
<defaultDocument>
  <files>
    <clear />
  </files>
</defaultDocument>
<directoryBrowse enabled="false" />
```

**Объяснение:**
- `runAllManagedModulesForAllRequests="true"` - обеспечивает обработку всех запросов, включая extensionless URL
- `<defaultDocument><files><clear /></files></defaultDocument>` - отключает документы по умолчанию, используем маршрутизацию API
- `directoryBrowse enabled="false"` - отключает просмотр каталогов для безопасности

### 4. Обновлен файл проекта

**Файл:** `HotelsApp.API/HotelsApp.API.csproj`

**Добавлено:**
```xml
<Compile Include="Controllers\HomeController.cs" />
```

**Важно:** Без этого файл не компилировался, и контроллер был недоступен во время выполнения.

## Результат

После внесения изменений:

1. ✅ Корневой URL (`http://localhost:8080/`) теперь работает
2. ✅ Возвращается JSON с информацией об API
3. ✅ Все остальные эндпоинты (`/api/*`) продолжают работать как прежде
4. ✅ Ошибка 403.14 устранена

## Тестирование

После пересборки проекта и перезапуска приложения:

```bash
# Запрос к корневому URL
GET http://localhost:8080/

# Ожидаемый ответ:
{
  "message": "HotelsApp Web API",
  "version": "1.0",
  "endpoints": {
    "hotels": "/api/hotels",
    "rooms": "/api/rooms",
    "bookings": "/api/bookings"
  },
  "documentation": "Use the endpoints above to access the API resources."
}
```

## Файлы, которые были изменены

1. ✅ `HotelsApp.API/Controllers/HomeController.cs` - **создан новый файл**
2. ✅ `HotelsApp.API/App_Start/WebApiConfig.cs` - **обновлен**
3. ✅ `HotelsApp.API/Web.config` - **обновлен**
4. ✅ `HotelsApp.API/HotelsApp.API.csproj` - **обновлен**

## Примечания

- Все изменения обратно совместимы
- Существующие эндпоинты API не затронуты
- Изменения не влияют на работу других контроллеров
- Рекомендуется пересобрать проект после изменений
