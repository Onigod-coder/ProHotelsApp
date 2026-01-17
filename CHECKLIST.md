# Чек-лист: Проверка готовности API проекта

## ✅ Что должно быть сделано:

### 1. Проект создан в solution
- [ ] В Visual Studio в Solution Explorer видны **2 проекта**:
  - ✅ HotelsApp
  - ✅ HotelsApp.API

**Как проверить:**
- Откройте `HotelsApp.sln` в Visual Studio
- В Solution Explorer должны быть оба проекта

**Если нет:**
- Следуйте инструкции в `QUICK_START.md` (шаги 1-5)

---

### 2. Проект HotelsApp.API добавлен в solution
- [ ] В файле `HotelsApp.sln` есть запись о проекте HotelsApp.API

**Как проверить:**
- Откройте `HotelsApp.sln` в текстовом редакторе
- Должна быть строка: `Project(...) = "HotelsApp.API"`

**Если нет:**
- Проект не добавлен в solution. Добавьте его через Visual Studio (Add → Existing Project)

---

### 3. Добавлена ссылка на HotelsApp
- [ ] В проекте HotelsApp.API есть Reference на HotelsApp

**Как проверить:**
- В Visual Studio: HotelsApp.API → References
- Должен быть проект **HotelsApp** (не DLL, а проект!)

**Если нет:**
- Правой кнопкой на HotelsApp.API → Add → Reference → Projects → Solution → ✅ HotelsApp

---

### 4. Установлены NuGet пакеты
- [ ] Microsoft.AspNet.WebApi (5.2.9)
- [ ] Microsoft.AspNet.WebApi.Cors (5.2.9)
- [ ] EntityFramework (6.5.1)
- [ ] Newtonsoft.Json (13.0.1)

**Как проверить:**
- HotelsApp.API → References → должны быть пакеты NuGet
- Или: HotelsApp.API → packages.config

**Если нет:**
- Правой кнопкой на HotelsApp.API → Manage NuGet Packages → Browse → установите пакеты

---

### 5. Скопированы файлы из папки API/

#### 5.1. WebApiConfig.cs
- [ ] Файл `HotelsApp.API/App_Start/WebApiConfig.cs` существует
- [ ] Содержит настройки CORS и маршрутизации

**Как проверить:**
- Откройте файл, должен быть метод `Register` с настройками

**Если нет:**
- Скопируйте содержимое из `API/App_Start/WebApiConfig.cs`

#### 5.2. Global.asax.cs
- [ ] Файл `HotelsApp.API/Global.asax.cs` обновлен
- [ ] Вызывает `WebApiConfig.Register`

**Как проверить:**
- Должна быть строка: `GlobalConfiguration.Configure(App_Start.WebApiConfig.Register);`

**Если нет:**
- Скопируйте содержимое из `API/Global.asax.cs`

#### 5.3. Контроллеры
- [ ] `HotelsApp.API/Controllers/HotelsController.cs`
- [ ] `HotelsApp.API/Controllers/BookingsController.cs`
- [ ] `HotelsApp.API/Controllers/RoomsController.cs`

**Как проверить:**
- В Solution Explorer: HotelsApp.API → Controllers → должны быть все 3 файла

**Если нет:**
- Скопируйте файлы из `API/Controllers/` в `HotelsApp.API/Controllers/`

#### 5.4. DTOs
- [ ] Папка `HotelsApp.API/DTOs/` создана
- [ ] `HotelDTO.cs`, `RoomDTO.cs`, `BookingDTO.cs` скопированы

**Как проверить:**
- В Solution Explorer должна быть папка DTOs с 3 файлами

**Если нет:**
- Создайте папку DTOs
- Скопируйте файлы из `API/DTOs/`

#### 5.5. Filters
- [ ] Папка `HotelsApp.API/Filters/` создана
- [ ] `ExceptionFilter.cs` скопирован

**Как проверить:**
- В Solution Explorer должна быть папка Filters с файлом

**Если нет:**
- Создайте папку Filters
- Скопируйте файл из `API/Filters/`

---

### 6. Настроен Web.config
- [ ] Скопирована секция `<connectionStrings>` из `HotelsApp/App.config`
- [ ] Скопирована секция `<appSettings>` из `HotelsApp/App.config`
- [ ] Добавлена секция `<entityFramework>`

**Как проверить:**
- Откройте `HotelsApp.API/Web.config`
- Должны быть все 3 секции

**Если нет:**
- Скопируйте секции из `HotelsApp/App.config` или из `API/Web.config`

---

### 7. Проект компилируется
- [ ] Build → Build Solution (F6) выполняется без ошибок

**Как проверить:**
- В Visual Studio: Build → Build Solution
- В Output должно быть: "Build succeeded"

**Если есть ошибки:**
- Проверьте using директивы в контроллерах
- Проверьте, что добавлена ссылка на HotelsApp
- Проверьте namespace в файлах

---

### 8. Проект запускается
- [ ] HotelsApp.API можно запустить (F5)
- [ ] Открывается браузер с адресом типа `http://localhost:port/`

**Как проверить:**
- Правой кнопкой на HotelsApp.API → Set as StartUp Project
- Нажмите F5
- Должен открыться браузер

**Если не запускается:**
- Проверьте ошибки компиляции
- Проверьте Web.config

---

### 9. API работает
- [ ] `http://localhost:port/api/hotels` возвращает данные

**Как проверить:**
- После запуска откройте в браузере: `http://localhost:port/api/hotels`
- Должен вернуться JSON с отелями или пустой массив

**Если ошибка:**
- Проверьте connection string в Web.config
- Проверьте, что база данных доступна
- Проверьте логи ошибок

---

## 📋 Быстрая проверка в Visual Studio:

1. **Откройте Solution Explorer**
2. **Проверьте структуру:**
   ```
   Solution 'HotelsApp'
   ├── HotelsApp ✅
   └── HotelsApp.API ✅
       ├── References
       │   └── HotelsApp ✅ (проект)
       ├── App_Start
       │   └── WebApiConfig.cs ✅
       ├── Controllers
       │   ├── HotelsController.cs ✅
       │   ├── BookingsController.cs ✅
       │   └── RoomsController.cs ✅
       ├── DTOs
       │   ├── HotelDTO.cs ✅
       │   ├── RoomDTO.cs ✅
       │   └── BookingDTO.cs ✅
       ├── Filters
       │   └── ExceptionFilter.cs ✅
       ├── Global.asax ✅
       ├── Global.asax.cs ✅
       └── Web.config ✅
   ```

3. **Попробуйте собрать проект:**
   - Build → Build Solution
   - Должно быть: "Build succeeded"

4. **Попробуйте запустить:**
   - Правой кнопкой на HotelsApp.API → Set as StartUp Project
   - F5
   - Должен открыться браузер

---

## ❌ Частые проблемы:

### "The type or namespace name 'ProHotelEntities' could not be found"
- ✅ Проверьте: HotelsApp.API → References → есть ли HotelsApp
- ✅ Добавьте: `using HotelsApp.Model;` в контроллерах

### "Cannot add reference"
- ✅ Убедитесь, что оба проекта в одном solution
- ✅ Убедитесь, что HotelsApp.API - это .NET Framework 4.8 (не .NET 8!)

### "Connection string not found"
- ✅ Скопируйте connection string из `HotelsApp/App.config` в `HotelsApp.API/Web.config`

### Проект не компилируется
- ✅ Проверьте все using директивы
- ✅ Проверьте namespace в файлах (должен быть `HotelsApp.API.Controllers`, не `HotelsApp.Controllers`)

