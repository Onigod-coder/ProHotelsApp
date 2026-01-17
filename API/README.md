# HotelsApp API

REST API для приложения управления отелями.

## Структура проекта

```
API/
├── App_Start/
│   └── WebApiConfig.cs          # Конфигурация маршрутизации и CORS
├── Controllers/
│   ├── HomeController.cs        # Корневой контроллер API (информация об API)
│   ├── HotelsController.cs      # Управление отелями
│   ├── BookingsController.cs    # Управление бронированиями
│   └── RoomsController.cs       # Управление номерами
├── DTOs/
│   ├── HotelDTO.cs              # DTO для отелей
│   ├── RoomDTO.cs               # DTO для номеров
│   └── BookingDTO.cs            # DTO для бронирований
├── Filters/
│   └── ExceptionFilter.cs       # Обработка исключений
├── Global.asax                  # Точка входа приложения
├── Global.asax.cs               # Код Global.asax
├── Web.config                   # Конфигурация приложения
├── INSTALLATION.md              # Инструкция по установке
└── README.md                    # Этот файл
```

## Основные endpoints

### Home (Информация об API)

- `GET /` - Получить информацию об API
  - Возвращает JSON с версией API и списком доступных эндпоинтов
  - Пример ответа:
  ```json
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

### Hotels (Отели)

- `GET /api/hotels` - Получить список отелей с фильтрацией
  - Параметры: `cityId`, `minStars`, `maxStars`, `minPrice`, `maxPrice`, `amenities`, `page`, `pageSize`, `sortBy`, `ascending`
- `GET /api/hotels/{id}` - Получить детали отеля
- `GET /api/hotels/{id}/rooms` - Получить номера отеля
  - Параметры: `checkIn`, `checkOut` (опционально)

### Rooms (Номера)

- `GET /api/rooms` - Получить список номеров
  - Параметры: `hotelId` (опционально)
- `GET /api/rooms/{id}` - Получить детали номера
- `GET /api/rooms/available` - Получить доступные номера
  - Параметры: `checkIn`, `checkOut`, `hotelId` (опционально)

### Bookings (Бронирования)

- `GET /api/bookings` - Получить список бронирований
  - Параметры: `customerId` (опционально)
- `GET /api/bookings/{id}` - Получить детали бронирования
- `POST /api/bookings` - Создать бронирование
  - Body: `{ customerID, roomID, checkInDate, checkOutDate }`
- `PUT /api/bookings/{id}/cancel` - Отменить бронирование

## Примеры использования

### Получить список отелей

```http
GET /api/hotels?cityId=1&minStars=3&page=1&pageSize=10
```

### Создать бронирование

```http
POST /api/bookings
Content-Type: application/json

{
  "customerID": 1,
  "roomID": 5,
  "checkInDate": "2024-01-15T00:00:00",
  "checkOutDate": "2024-01-20T00:00:00"
}
```

### Получить доступные номера

```http
GET /api/rooms/available?checkIn=2024-01-15&checkOut=2024-01-20&hotelId=1
```

## Формат ответов

### Успешный ответ

```json
{
  "data": [...],
  "totalCount": 100,
  "page": 1,
  "pageSize": 10,
  "totalPages": 10
}
```

### Ошибка

```json
{
  "error": "Сообщение об ошибке",
  "details": "Детали ошибки",
  "timestamp": "2024-01-01T12:00:00"
}
```

## Технологии

- ASP.NET Web API 2
- Entity Framework 6.5.1
- .NET Framework 4.8
- Newtonsoft.Json

## Дополнительная информация

См. файл `API_REQUIREMENTS.md` в корне проекта для полного списка требований и `API/INSTALLATION.md` для инструкций по установке.

