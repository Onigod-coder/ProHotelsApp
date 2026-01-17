# Инструкция: Добавление проекта API в Solution

## Проблема
Вы создали проект на .NET 8, а HotelsApp использует .NET Framework 4.8. Они несовместимы.

## Решение: Создать правильный проект ASP.NET Web API 2

### Шаг 1: Откройте solution HotelsApp.sln
1. Откройте Visual Studio
2. File → Open → Project/Solution
3. Выберите `C:\Users\onigo\Desktop\HotelsApp\HotelsApp.sln`

### Шаг 2: Добавьте новый проект в solution
1. Правой кнопкой на **Solution 'HotelsApp'** в Solution Explorer
2. Выберите **Add → New Project...**

### Шаг 3: Создайте проект ASP.NET Web Application
1. В поиске введите "ASP.NET Web Application"
2. Выберите **ASP.NET Web Application (.NET Framework)** (НЕ .NET!)
3. Нажмите **Next**

### Шаг 4: Настройте проект
1. **Project name:** `HotelsApp.API`
2. **Location:** `C:\Users\onigo\Desktop\HotelsApp` (та же папка, где HotelsApp)
3. **Solution:** Add to solution
4. Нажмите **Create**

### Шаг 5: Выберите шаблон
1. Выберите **Web API** (пустой шаблон с Web API)
2. Убедитесь, что выбрано **.NET Framework 4.8**
3. Нажмите **Create**

### Шаг 6: Добавьте ссылку на проект HotelsApp
1. Правой кнопкой на проект **HotelsApp.API** в Solution Explorer
2. Выберите **Add → Reference...**
3. В открывшемся окне выберите вкладку **Projects**
4. Установите галочку напротив **HotelsApp**
5. Нажмите **OK**

### Шаг 7: Установите NuGet пакеты
1. Правой кнопкой на проект **HotelsApp.API**
2. Выберите **Manage NuGet Packages...**
3. Перейдите на вкладку **Browse**
4. Установите следующие пакеты:
   - `Microsoft.AspNet.WebApi` (версия 5.2.9)
   - `Microsoft.AspNet.WebApi.Cors` (версия 5.2.9)
   - `EntityFramework` (версия 6.5.1)
   - `Newtonsoft.Json` (версия 13.0.1)

### Шаг 8: Скопируйте файлы из папки API
Скопируйте файлы из папки `API/` в соответствующие места проекта:

1. **WebApiConfig.cs:**
   - Скопируйте содержимое `API/App_Start/WebApiConfig.cs`
   - Вставьте в `HotelsApp.API/App_Start/WebApiConfig.cs` (замените существующий)

2. **Global.asax.cs:**
   - Скопируйте содержимое `API/Global.asax.cs`
   - Вставьте в `HotelsApp.API/Global.asax.cs` (замените существующий)

3. **Контроллеры:**
   - Скопируйте все файлы из `API/Controllers/`
   - Вставьте в `HotelsApp.API/Controllers/`

4. **DTOs:**
   - Создайте папку `HotelsApp.API/DTOs/`
   - Скопируйте все файлы из `API/DTOs/`
   - Вставьте в `HotelsApp.API/DTOs/`

5. **Filters:**
   - Создайте папку `HotelsApp.API/Filters/`
   - Скопируйте все файлы из `API/Filters/`
   - Вставьте в `HotelsApp.API/Filters/`

6. **Web.config:**
   - Откройте `HotelsApp.API/Web.config`
   - Скопируйте секцию `<connectionStrings>` из `HotelsApp/App.config`
   - Скопируйте секцию `<appSettings>` из `HotelsApp/App.config`
   - Добавьте секцию `<entityFramework>` из `HotelsApp/App.config`

### Шаг 9: Проверьте namespace
Убедитесь, что во всех скопированных файлах namespace правильный:
- Контроллеры должны быть в `HotelsApp.API.Controllers`
- DTOs должны быть в `HotelsApp.API.DTOs`
- Filters должны быть в `HotelsApp.API.Filters`

### Шаг 10: Запустите проект
1. Правой кнопкой на проект **HotelsApp.API**
2. Выберите **Set as StartUp Project**
3. Нажмите **F5** для запуска

## Альтернативный вариант (если хотите использовать .NET 8 проект)

Если вы хотите использовать созданный .NET 8 проект, вам нужно:

1. Скопировать модели и сервисы в новый проект (не ссылка, а копия файлов)
2. Или создать общую библиотеку классов (.NET Standard 2.0), которую смогут использовать оба проекта

Но это сложнее и не рекомендуется для вашего случая.

## Проверка работы

После выполнения всех шагов откройте браузер:
- `http://localhost:port/api/hotels` - должен вернуть список отелей

Если возникают ошибки, проверьте:
- Правильность connection string в Web.config
- Наличие всех using директив в контроллерах
- Правильность namespace во всех файлах

