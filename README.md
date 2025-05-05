# HotelsApp

Приложение для бронирования отелей с современным интерфейсом и расширенным функционалом.

## Функциональность

- Поиск и фильтрация отелей
- Бронирование номеров
- Управление бронированиями
- Отзывы и рейтинги
- Избранные отели
- Уведомления о бронированиях
- Экспорт бронирований в PDF
- Двухфакторная аутентификация
- Интеграция с Google Maps
- Логирование действий

## Требования

- .NET Framework 4.7.2
- Visual Studio 2019 или новее
- SQL Server LocalDB
- Google Maps API ключ
- SMTP сервер для отправки уведомлений

## Установка

1. Клонируйте репозиторий:
```bash
git clone https://github.com/yourusername/HotelsApp.git
```

2. Откройте решение в Visual Studio

3. Восстановите NuGet пакеты:
```bash
nuget restore
```

4. Обновите конфигурацию в App.config:
- Укажите строку подключения к базе данных
- Добавьте Google Maps API ключ
- Настройте параметры SMTP сервера
- Настройте параметры логирования

5. Создайте базу данных:
```bash
Update-Database
```

6. Запустите приложение

## Тестирование

Для запуска тестов используйте Test Explorer в Visual Studio или выполните команду:
```bash
dotnet test
```

## Структура проекта

- `Services/` - Сервисные классы
  - `HotelService.cs` - Управление отелями
  - `BookingService.cs` - Управление бронированиями
  - `NotificationService.cs` - Отправка уведомлений
  - `SecurityService.cs` - Безопасность и аутентификация
  - `MapService.cs` - Работа с картами
  - `LoggingService.cs` - Логирование

- `Tests/` - Тесты
  - `HotelServiceTests.cs`
  - `BookingServiceTests.cs`
  - `SecurityServiceTests.cs`
  - `MapServiceTests.cs`
  - `LoggingServiceTests.cs`

## Лицензия

MIT 