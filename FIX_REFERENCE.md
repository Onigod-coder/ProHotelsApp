# Исправление ошибки "Не удалось найти тип NotificationService"

## Проблема
Ошибка компиляции: "Не удалось найти тип или имя пространства имен 'NotificationService'"

## Решение

### Шаг 1: Проверьте, что проект HotelsApp компилируется
1. В Visual Studio: правой кнопкой на проект **HotelsApp**
2. Выберите **Build**
3. Убедитесь, что нет ошибок компиляции

### Шаг 2: Добавьте ссылку вручную
1. Правой кнопкой на проект **HotelsApp.API** в Solution Explorer
2. Выберите **Add → Reference...**
3. Перейдите на вкладку **Projects**
4. В списке **Solution** найдите **HotelsApp**
5. ✅ Убедитесь, что галочка установлена
6. Если галочки нет - установите её
7. Нажмите **OK**

### Шаг 3: Пересоберите solution
1. В меню: **Build → Rebuild Solution**
2. Дождитесь завершения сборки

### Шаг 4: Проверьте using директивы
Убедитесь, что в файле `BookingsController.cs` есть:
```csharp
using HotelsApp.Services;
```

### Если не помогло:

1. **Закройте и откройте Visual Studio**
2. **Очистите solution:**
   - Build → Clean Solution
   - Затем Build → Rebuild Solution

3. **Проверьте, что оба проекта в одном solution:**
   - В Solution Explorer должны быть видны оба проекта:
     - ✅ HotelsApp
     - ✅ HotelsApp.API

4. **Удалите и добавьте ссылку заново:**
   - Правой кнопкой на HotelsApp.API → Remove Reference → HotelsApp
   - Затем Add → Reference → Projects → Solution → HotelsApp

## Проверка

После выполнения всех шагов:
1. Откройте `BookingsController.cs`
2. Наведите курсор на `NotificationService` - не должно быть красной волнистой линии
3. Build → Build Solution - не должно быть ошибок

