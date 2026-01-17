# Быстрый старт: Добавление проекта API

## ⚠️ Важно
Вы создали проект на **.NET 8**, а HotelsApp использует **.NET Framework 4.8**. Они несовместимы!

## ✅ Правильное решение

### Вариант 1: Создать правильный проект (РЕКОМЕНДУЕТСЯ)

1. **Откройте solution HotelsApp.sln**
   - File → Open → Project/Solution
   - Выберите `C:\Users\onigo\Desktop\HotelsApp\HotelsApp.sln`

2. **Добавьте новый проект:**
   - Правой кнопкой на **Solution 'HotelsApp'** (в Solution Explorer)
   - **Add → New Project...**

3. **Выберите шаблон:**
   - В поиске: "ASP.NET Web Application"
   - Выберите **ASP.NET Web Application (.NET Framework)** ⚠️ НЕ .NET!
   - Нажмите **Next**

4. **Настройки проекта:**
   - Name: `HotelsApp.API`
   - Location: `C:\Users\onigo\Desktop\HotelsApp` (та же папка)
   - Solution: **Add to solution**
   - Нажмите **Create**

5. **Выберите шаблон Web API:**
   - Выберите **Web API** (пустой шаблон)
   - Убедитесь: **.NET Framework 4.8**
   - Нажмите **Create**

6. **Добавьте ссылку на HotelsApp:**
   - Правой кнопкой на проект **HotelsApp.API**
   - **Add → Reference...**
   - Вкладка **Projects**
   - ✅ Поставьте галочку на **HotelsApp**
   - **OK**

7. **Установите NuGet пакеты:**
   - Правой кнопкой на **HotelsApp.API**
   - **Manage NuGet Packages...**
   - Browse → установите:
     - `Microsoft.AspNet.WebApi` (5.2.9)
     - `Microsoft.AspNet.WebApi.Cors` (5.2.9)
     - `EntityFramework` (6.5.1)

8. **Скопируйте файлы из папки API/** (см. ADD_PROJECT_TO_SOLUTION.md)

---

## Визуальная инструкция

### Шаг 1: Добавление проекта
```
Solution Explorer
└── Solution 'HotelsApp' (правой кнопкой)
    └── Add
        └── New Project...
```

### Шаг 2: Выбор шаблона
```
Visual C# → Web → ASP.NET Web Application (.NET Framework)
⚠️ НЕ ASP.NET Core Web Application!
```

### Шаг 3: Добавление Reference
```
HotelsApp.API (правой кнопкой)
└── Add
    └── Reference...
        └── Projects → Solution
            └── ✅ HotelsApp
```

---

## Проверка

После выполнения:
1. В Solution Explorer должны быть оба проекта:
   - ✅ HotelsApp
   - ✅ HotelsApp.API

2. В HotelsApp.API → References должны быть:
   - ✅ HotelsApp (проект, не DLL)

3. Запустите HotelsApp.API (F5)
4. Откройте: `http://localhost:port/api/hotels`

---

## Если что-то не работает

**Ошибка: "Cannot add reference"**
- Убедитесь, что оба проекта в одном solution
- Убедитесь, что HotelsApp.API - это .NET Framework 4.8

**Ошибка: "The type or namespace name 'ProHotelEntities' could not be found"**
- Проверьте, что добавлена ссылка на HotelsApp
- Добавьте `using HotelsApp.Model;` в контроллеры

**Ошибка подключения к БД**
- Скопируйте connection string из `HotelsApp/App.config` в `HotelsApp.API/Web.config`

