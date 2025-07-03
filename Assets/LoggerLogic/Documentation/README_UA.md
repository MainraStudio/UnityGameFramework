# 📘 Logger — Легкий та гнучкий інструмент логування для Unity

Простий, елегантний і потужний логер, що допомагає тримати розробку в чистоті та порядку.

---

## ✨ Особливості

- ✅ Статичний API для зручного логування  
- 🎨 Кольорові категорії логів з емодзі/іконками  
- 🔍 Інтерфейс у редакторі для фільтрації логів  
- ⚙️ Увімкнення/вимкнення логів глобально або по категоріях  
- 🧩 Легкий, без залежностей  
- 🎯 Працює в редакторі та в білді

---

## 🛠️ Встановлення

1. Скопіюй папку `LoggerLogic/` у свій Unity-проєкт.
2. Відкрий конфігураційний ассет:
   ```
   Assets/LoggerLogic/Config/LoggerConfig.asset
   ```
3. Налаштуй категорії, кольори, емодзі та глобальні параметри у інспекторі.

---

## ⚙️ Налаштування

Керуй логами через `LoggerConfig`:
- **Enable All Logs** — Вмикає/вимикає всі категорії
- **Editor Only** — Логи лише у редакторі
- **Category Configuration** — Додай власні категорії з кольорами та емодзі

Можна також керувати логами в коді:
```csharp
Logger.Enabled = true;
```

---

## 🧑‍💻 Використання

```csharp
CustomLogger.Log("Системний лог", CustomLogger.LogCategory.System);
CustomLogger.LogWarning("Попередження по UI", CustomLogger.LogCategory.UI);
CustomLogger.LogError("Помилка в ігровому процесі", CustomLogger.LogCategory.Gameplay);
```

---

## 🧪 Приклад виводу

```
⚙ [System] Системний лог  
LoggerExample.Awake:13
```

---

## 📂 Структура проєкту

```
LoggerLogic/
├── Config/
│   └── LoggerConfig.asset
├── Editor/
├── Example/
├── Scenes/
├── CustomLogger.cs
├── LogCategory.cs
```

---

## 📦 Інтеграція

- ✅ Без зовнішніх залежностей
- 🧩 Працює з Odin Inspector, Zenject, UniTask та ін.
- 🛠️ Ідеальний для внутрішніх тулів або runtime дебагу
- 💡 Гнучкий і швидкий для будь-якого проєкту Unity

---

## 🔚 Ліцензія

MIT License — дозволено вільне використання, зміну та розповсюдження.  
Деталі у файлі [LICENSE](./LICENSE).

---

## 📬 Зв’язок

Ідеї, питання чи помилки — пишіть!  
**GitHub**: https://github.com/AndriiSviatenko/Logger-Lightweight-Flexible-Debugging-Tool-for-Unity 
**Email**: your.email@example.com
