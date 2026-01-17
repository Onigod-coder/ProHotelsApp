# HotelsApp

Приложение для бронирования отелей с современным интерфейсом и расширенным функционалом.

## Структура проекта

- **HotelsApp** - WPF приложение для управления отелями
- **HotelsApp.API** - Web API для RESTful доступа к данным

## Быстрый старт

### Web API

1. Установите зависимости: `Build → Restore NuGet Packages`
2. Настройте строку подключения в `HotelsApp.API/Web.config`
3. Установите HotelsApp.API как стартовый проект
4. Запустите проект (F5)
5. Откройте `http://localhost:8080/` для проверки API

### Документация

- [CHANGELOG.md](CHANGELOG.md) - история изменений
- [API_CHANGES.md](API_CHANGES.md) - последние изменения в API
- [HotelsApp.API/README.md](HotelsApp.API/README.md) - документация по API
- [PROJECT_STATUS.md](PROJECT_STATUS.md) - текущий статус проекта

## Последние обновления

### Исправление ошибки 403.14 в Web API

- ✅ Добавлен `HomeController` для обработки корневого URL
- ✅ Обновлена конфигурация маршрутизации
- ✅ Исправлена настройка Web.config для extensionless URL
- ✅ Корневой URL теперь возвращает информацию об API

Подробности см. в [API_CHANGES.md](API_CHANGES.md) и [CHANGELOG.md](CHANGELOG.md).
