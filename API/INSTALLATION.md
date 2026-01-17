# Инструкция по установке и настройке API

## Шаг 1: Создание проекта API

1. Откройте решение `HotelsApp.sln` в Visual Studio
2. Правой кнопкой на Solution → Add → New Project
3. Выберите **ASP.NET Web Application (.NET Framework)**
4. Назовите проект `HotelsApp.API`
5. Выберите шаблон **Web API**
6. Убедитесь, что выбрана версия .NET Framework 4.8

## Шаг 2: Установка NuGet пакетов

В Package Manager Console выполните:

```powershell
Install-Package Microsoft.AspNet.WebApi -Version 5.2.9
Install-Package Microsoft.AspNet.WebApi.Cors -Version 5.2.9
Install-Package EntityFramework -Version 6.5.1
Install-Package Newtonsoft.Json -Version 13.0.1
```

## Шаг 3: Добавление ссылок на существующие проекты

1. Правой кнопкой на проект `HotelsApp.API` → Add → Reference
2. Выберите Projects → Solution
3. Добавьте ссылки на:
   - `HotelsApp` (для доступа к Model и Services)

## Шаг 4: Копирование файлов

Скопируйте созданные файлы из папки `API/` в соответствующие места проекта:

### Структура файлов:
```
HotelsApp.API/
├── App_Start/
│   └── WebApiConfig.cs (скопировать из API/App_Start/)
├── Controllers/
│   ├── HotelsController.cs (скопировать из API/Controllers/)
│   ├── BookingsController.cs (скопировать из API/Controllers/)
│   └── RoomsController.cs (скопировать из API/Controllers/)
├── DTOs/
│   ├── HotelDTO.cs (скопировать из API/DTOs/)
│   ├── RoomDTO.cs (скопировать из API/DTOs/)
│   └── BookingDTO.cs (скопировать из API/DTOs/)
├── Filters/
│   └── ExceptionFilter.cs (скопировать из API/Filters/)
├── Global.asax (скопировать из API/)
├── Global.asax.cs (скопировать из API/)
└── Web.config (обновить connection string из API/Web.config)
```

## Шаг 5: Настройка Web.config

1. Откройте `Web.config` проекта API
2. Скопируйте секцию `<connectionStrings>` из `App.config` основного проекта
3. Скопируйте секцию `<appSettings>` из `App.config` основного проекта
4. Убедитесь, что connection string указывает на правильную базу данных

## Шаг 6: Настройка проекта как стартового

1. Правой кнопкой на проект `HotelsApp.API` → Set as StartUp Project
2. Нажмите F5 для запуска

## Шаг 7: Проверка работы API

После запуска откройте браузер и перейдите по адресу:
- `http://localhost:port/api/hotels` - должен вернуть список отелей
- `http://localhost:port/api/rooms` - должен вернуть список номеров

## Дополнительные настройки

### Настройка CORS
Если нужно разрешить доступ из других доменов, измените в `WebApiConfig.cs`:

```csharp
// Для разработки (разрешить все)
var cors = new EnableCorsAttribute("*", "*", "*");

// Для продакшена (указать конкретные домены)
var cors = new EnableCorsAttribute("https://yourdomain.com", "*", "*");
```

### Настройка порта
В свойствах проекта (Properties → Web) можно изменить порт, на котором будет работать API.

## Решение проблем

### Ошибка: "The type or namespace name 'ProHotelEntities' could not be found"
- Убедитесь, что добавлена ссылка на проект `HotelsApp`
- Проверьте, что в контроллерах есть `using HotelsApp.Model;`

### Ошибка подключения к базе данных
- Проверьте connection string в `Web.config`
- Убедитесь, что база данных доступна
- Проверьте права доступа к базе данных

### Ошибка: "Multiple types were found that match the controller"
- Убедитесь, что контроллеры находятся в папке `Controllers/`
- Проверьте, что классы контроллеров наследуются от `ApiController`

## Тестирование API

### Использование Postman или аналогичного инструмента:

1. **GET запросы:**
   - `GET http://localhost:port/api/hotels`
   - `GET http://localhost:port/api/hotels/1`
   - `GET http://localhost:port/api/rooms/available?checkIn=2024-01-01&checkOut=2024-01-05`

2. **POST запросы:**
   - `POST http://localhost:port/api/bookings`
   - Body (JSON):
     ```json
     {
       "customerID": 1,
       "roomID": 1,
       "checkInDate": "2024-01-01T00:00:00",
       "checkOutDate": "2024-01-05T00:00:00"
     }
     ```

3. **PUT запросы:**
   - `PUT http://localhost:port/api/bookings/1/cancel`

## Следующие шаги

1. Добавить аутентификацию (JWT токены)
2. Добавить Swagger для документации API
3. Добавить валидацию входных данных
4. Добавить логирование запросов
5. Настроить HTTPS

