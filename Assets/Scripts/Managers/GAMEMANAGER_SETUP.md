# GameManager - Настройка врагов и волн 🎮

## Где хранятся все префабы врагов?

**Ответ: В GameManager.cs** ✅

---

## 📋 Настройка GameManager в Unity:

### **Шаг 1: Найти GameManager в сцене**

Если его нет:
```
Hierarchy → Create Empty → GameManager
Добавить скрипт GameManager
```

### **Шаг 2: Назначить префабы врагов**

В Inspector → **GameManager** → **Enemy Prefabs**:

| Поле | Префаб | Обязательно? |
|------|--------|--------------|
| **Basic Enemy Prefab** | BasicEnemy | ✅ ДА |
| **Shooter Enemy Prefab** | ShooterEnemy | ⚠️ С волны 3 |
| **Fast Enemy Prefab** | FastEnemy | ⚠️ С волны 4 |
| **Dasher Enemy Prefab** | DasherEnemy | ⚠️ С волны 5 |
| **Teleporter Enemy Prefab** | TeleporterEnemy | ⚠️ С волны 6 |
| **Tank Enemy Prefab** | TankEnemy | ⚠️ С волны 7 |

**Просто перетащи префабы из папки Prefabs!**

### **Шаг 3: Настроить параметры волн**

В Inspector → **GameManager** → **Wave Settings**:

| Параметр | Значение | Описание |
|----------|----------|----------|
| **Arena Center** | `null` | Создастся автоматически в (0,0) |
| **Spawn Radius** | `15` | Радиус спавна врагов |
| **Time Between Waves** | `3` | Пауза между волнами (сек) |

### **Шаг 4: Настроить появление врагов**

В Inspector → **GameManager** → **Wave Progression**:

| Параметр | По умолчанию | Что делает |
|----------|--------------|------------|
| **Shooter Unlock Wave** | `3` | С какой волны появляются Shooter |
| **Fast Unlock Wave** | `4` | С какой волны Fast |
| **Dasher Unlock Wave** | `5` | Dasher |
| **Teleporter Unlock Wave** | `6` | Teleporter |
| **Tank Unlock Wave** | `7` | Tank (мини-боссы) |

---

## 🌊 Автоматические волны:

GameManager **сам решает**, каких врагов спавнить на каждой волне!

### **Волна 1-2: Обучение**
```
Волна 1: 4 BasicEnemy
Волна 2: 5 BasicEnemy
```
→ Игрок учится управлению

### **Волна 3-4: Первые стрелки**
```
Волна 3: 2 Basic + 1 Shooter
Волна 4: 2 Basic + 2 Shooter
```
→ Появляется дальний бой

### **Волна 5-6: Хаос и скорость**
```
Волна 5: 1 Basic + 1 Shooter + 2 Fast
Волна 6: 1 Basic + 1 Shooter + 2 Fast + 1 Dasher
```
→ Быстрые враги + врывающиеся

### **Волна 7-8: Мини-боссы**
```
Волна 7: 1 Tank + 2 Basic + 1 Shooter + 1 Fast
Волна 8: 1 Tank + 2 Basic + 1 Shooter + 1 Fast + 1 Dasher + 1 Teleporter
```
→ Появляются танки

### **Волна 9+: Экстрим**
```
Волна 9: 1 Tank + 2 Shooter + 2 Dasher + 2 Teleporter + 3 Fast
Волна 10: 2 Tank + 2 Shooter + 2 Dasher + 2 Teleporter + 3 Fast
Волна 11+: 3 Tank + 2 Shooter + 2 Dasher + 2 Teleporter + 3 Fast
```
→ Максимальная сложность

---

## 🔧 Как работает код:

### **Метод GetWaveComposition()**

```csharp
List<GameObject> GetWaveComposition(int wave) {
    List<GameObject> enemies = new List<GameObject>();
    
    if (wave <= 2) {
        // Только Basic
    }
    else if (wave <= 4) {
        // Basic + Shooter
    }
    // ... и так далее
    
    return enemies;
}
```

Этот метод возвращает **список префабов** для текущей волны.

### **Пример волны 5:**

```csharp
enemies.Add(basicEnemyPrefab);      // 1 Basic
enemies.Add(shooterEnemyPrefab);    // 1 Shooter
enemies.Add(fastEnemyPrefab);       // 1 Fast
enemies.Add(fastEnemyPrefab);       // 1 Fast
```

Итого: 4 врага

---

## 🎯 Как изменить волны?

### **Вариант 1: Изменить в коде (GameManager.cs)**

Найди метод `GetWaveComposition()` и измени:

```csharp
// ВОЛНА 1-2: Только Basic
if (wave <= 2)
{
    // Было: 3 + wave
    // Стало: 5 + wave (больше врагов!)
    for (int i = 0; i < 5 + wave; i++)
    {
        enemies.Add(basicEnemyPrefab);
    }
}
```

### **Вариант 2: Изменить unlock волны в Inspector**

```
Shooter Unlock Wave: 3 → 2 (появляются раньше!)
Tank Unlock Wave: 7 → 5 (танки раньше!)
```

---

## 💡 Идеи для кастомных волн:

### **Волна "Только Fast"** (хаос)
```csharp
else if (wave == 10) // Boss волна
{
    for (int i = 0; i < 10; i++) {
        enemies.Add(fastEnemyPrefab);
    }
}
```

### **Волна "Танковый ад"**
```csharp
else if (wave == 15)
{
    for (int i = 0; i < 5; i++) {
        enemies.Add(tankEnemyPrefab);
    }
}
```

### **Волна "Телепортеры"**
```csharp
else if (wave == 12)
{
    for (int i = 0; i < 8; i++) {
        enemies.Add(teleporterEnemyPrefab);
    }
}
```

---

## 🐛 Troubleshooting:

### Проблема: Враги не спавнятся
**Решение:**
- ✅ Префабы назначены в GameManager?
- ✅ У префабов есть тег "Enemy"?
- ✅ GameManager в сцене?

### Проблема: Спавнятся только Basic
**Решение:**
- Проверь текущую волну (должна быть >= unlock волны)
- Проверь что префабы назначены (Shooter, Fast, etc.)
- Посмотри Console логи: "✅ Создан: ShooterEnemy_Wave3"

### Проблема: Враги спавнятся в странных местах
**Решение:**
- Arena Center должен быть в (0, 0, 0)
- Spawn Radius слишком большой? (попробуй 10-15)

### Проблема: Слишком много/мало врагов
**Решение:**
- Измени метод `GetWaveComposition()` в GameManager.cs
- Или измени логику для конкретной волны

---

## 📊 Статистика волн:

| Волна | Врагов | Типы | Сложность |
|-------|--------|------|-----------|
| 1 | 4 | Basic | ⭐ |
| 2 | 5 | Basic | ⭐ |
| 3 | 3 | Basic, Shooter | ⭐⭐ |
| 4 | 4 | Basic, Shooter | ⭐⭐ |
| 5 | 4 | Basic, Shooter, Fast | ⭐⭐⭐ |
| 6 | 5 | Basic, Shooter, Fast, Dasher | ⭐⭐⭐ |
| 7 | 6 | Tank, Basic, Shooter, Fast | ⭐⭐⭐⭐ |
| 8 | 8 | Tank, Basic, Shooter, Fast, Dasher, Teleporter | ⭐⭐⭐⭐ |
| 9+ | 10+ | ВСЕ ТИПЫ | ⭐⭐⭐⭐⭐ |

---

## ✅ Быстрый чеклист:

- [ ] GameManager в сцене
- [ ] BasicEnemy назначен ✅
- [ ] ShooterEnemy назначен
- [ ] FastEnemy назначен
- [ ] DasherEnemy назначен
- [ ] TeleporterEnemy назначен
- [ ] TankEnemy назначен
- [ ] Все префабы имеют тег "Enemy"
- [ ] Spawn Radius = 15
- [ ] Time Between Waves = 3
- [ ] Запустить игру и проверить волны

---

## 🎨 Расширение:

### **Создать WaveConfig Scriptable Object** (продвинутое)

Если хочешь более гибкую систему:

1. Создать `WaveConfig.cs`:
```csharp
[CreateAssetMenu(fileName = "Wave", menuName = "Game/Wave Config")]
public class WaveConfig : ScriptableObject
{
    public List<GameObject> enemies;
    public float spawnDelay;
    public string waveName;
}
```

2. Создать конфиги для каждой волны в Unity
3. В GameManager хранить массив `WaveConfig[]`

Но для начала **текущий вариант проще и работает отлично!** ✅

---

**Готово! Теперь все префабы врагов в одном месте - GameManager! 🎮**
