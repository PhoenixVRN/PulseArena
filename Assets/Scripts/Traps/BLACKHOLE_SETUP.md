# Черные дыры на арене 🕳️

## Что добавлено:

✅ **BlackHole.cs** — скрипт черной дыры с притяжением и уничтожением  
✅ **TrapManager.cs** — менеджер рандомного спавна дыр  
✅ Рандомный размер и позиция  
✅ Притяжение объектов к центру  
✅ Автоматическое пересоздание дыр через интервал  

---

## 📋 Настройка (Пошагово)

### Шаг 1: Создать префаб черной дыры

1. **Hierarchy** → **2D Object** → **Sprite** → **Circle**
2. Назвать: `BlackHole`
3. **Transform**:
   - Position: `(0, 0, 0)`
   - Scale: `(1, 1, 1)` — будет меняться автоматически

4. **Sprite Renderer**:
   - Color: **Черный** (`#000000`)
   - Или **Фиолетовый темный** (`#1a0033`)
   - Sorting Layer: `Game`
   - Order in Layer: `-1` (под игроком и врагами)

5. **Добавить Circle Collider 2D**:
   - ✅ **Is Trigger** = true
   - Radius: `0.5`

6. **Добавить скрипт BlackHole**:
   - Pull Force: `5`
   - Pull Radius: `3`
   - Rotation Speed: `30` (визуальное вращение)

7. **Сделать префаб**:
   - Перетащить в `Assets/Prefabs/BlackHole`
   - Удалить из сцены

---

### Шаг 2: Настроить TrapManager

1. **Hierarchy** → **Create Empty**
2. Назвать: `TrapManager`
3. **Добавить скрипт TrapManager**

**Настройки:**

| Параметр | Значение | Описание |
|----------|----------|----------|
| **Black Hole Prefab** | Префаб BlackHole | Перетащить из Prefabs |
| **Min Black Holes** | `1` | Минимум дыр |
| **Max Black Holes** | `3` | Максимум дыр |
| **Arena Radius** | `18` | Радиус арены (чуть меньше границы) |
| **Min Distance From Center** | `5` | Не спавнить в центре |
| **Min Distance Between Holes** | `4` | Расстояние между дырами |
| **Size Range** | Min: `0.5`, Max: `2` | Диапазон размеров |
| **Respawn Interval** | `15` | Пересоздавать каждые 15 сек (0 = выкл) |
| **Avoid Player Spawn** | ✅ | Не спавнить рядом с игроком |
| **Player Safe Radius** | `8` | Безопасная зона игрока |

---

### Шаг 3: Улучшенный визуал (опционально)

#### Вариант A: Добавить свечение (2D Light)

Если используешь **URP (Universal Render Pipeline)**:

1. Выбрать префаб `BlackHole`
2. Добавить **Light 2D** (Component → Rendering → Light 2D)
3. Настройки:
   - Light Type: `Point`
   - Color: **Фиолетовый** (`#8800FF`)
   - Intensity: `0.5`
   - Outer Radius: `4`

#### Вариант B: Добавить внешнее кольцо

1. Дочерний объект: **Sprite** → **Circle**
2. Назвать: `OuterRing`
3. **Scale**: `(1.5, 1.5, 1)`
4. **Color**: Фиолетовый полупрозрачный (`#8800FF`, Alpha = 0.3)
5. **Sorting Order**: `-2` (под основной дырой)

#### Вариант C: Вращающиеся звезды вокруг

1. Дочерний объект: **Sprite** → **Square**
2. Назвать: `Star`
3. **Scale**: `(0.1, 0.1, 1)`
4. **Color**: Белый
5. Дублировать 3-4 раза по кругу
6. Скрипт `BlackHole` автоматически вращает родителя

---

## 🎮 Как работает система:

### 1. При старте игры (TrapManager.Start())
```
1. Генерируется случайное количество дыр (1-3)
2. Для каждой дыры:
   - Находится случайная валидная позиция
   - Генерируется случайный размер (0.5 - 2.0)
   - Создается объект BlackHole
```

### 2. Валидация позиции
```
✅ Расстояние от центра > 5
✅ Расстояние до края < 18
✅ Расстояние до игрока > 8
✅ Расстояние до других дыр > 4

Если позиция невалидна → пробуем снова (макс 30 попыток)
```

### 3. Притяжение (BlackHole.FixedUpdate())
```
Каждый кадр:
  1. Physics2D.OverlapCircleAll() находит объекты в радиусе
  2. Для каждого объекта:
     - Рассчитывается направление к центру
     - Сила притяжения убывает с расстоянием
     - AddForce() применяет притяжение
```

### 4. Уничтожение (BlackHole.OnTriggerEnter2D())
```
Объект коснулся центра дыры:
  - Враг → Destroy() + OnEnemyDied()
  - Игрок → Game Over (пока только лог)
```

### 5. Пересоздание дыр (опционально)
```
Каждые N секунд:
  1. ClearBlackHoles() — удаляет все дыры
  2. SpawnRandomBlackHoles() — создает новые
  
  → Арена постоянно меняется!
```

---

## 🎯 Стратегии балансировки:

### Легкий режим
```yaml
Min Black Holes: 1
Max Black Holes: 2
Size Range: 0.5 - 1.0
Pull Force: 3
Respawn Interval: 0 (не пересоздавать)
```

### Средний режим (рекомендуется)
```yaml
Min Black Holes: 1
Max Black Holes: 3
Size Range: 0.5 - 2.0
Pull Force: 5
Respawn Interval: 20
```

### Хардкор режим
```yaml
Min Black Holes: 2
Max Black Holes: 5
Size Range: 0.8 - 2.5
Pull Force: 8
Respawn Interval: 10
Player Safe Radius: 5
```

### Хаос режим 💀
```yaml
Min Black Holes: 3
Max Black Holes: 7
Size Range: 1.0 - 3.0
Pull Force: 10
Respawn Interval: 5
Min Distance Between Holes: 2
```

---

## 🔧 Параметры BlackHole:

| Параметр | Описание | Рекомендуемое |
|----------|----------|---------------|
| **Pull Force** | Сила притяжения | `5` |
| **Pull Radius** | Радиус притяжения | `3` (в 3 раза больше коллайдера) |
| **Rotation Speed** | Скорость вращения (визуал) | `30` |

### Как работает притяжение:

```csharp
pullStrength = pullForce * (1 - distance / pullRadius)

Примеры (pullForce = 5, pullRadius = 3):
- Расстояние 0: сила = 5 × 1.0 = 5 (максимум)
- Расстояние 1.5: сила = 5 × 0.5 = 2.5
- Расстояние 3: сила = 5 × 0.0 = 0 (нет притяжения)
```

---

## 🎨 Визуальные улучшения:

### Shader Graph (продвинутое)

Если знаком с Shader Graph (URP):
1. Создать **2D Shader Graph**
2. Добавить эффект искажения пространства
3. Анимация вращения текстуры
4. Gradient Noise для турбулентности

### Particle System

1. Добавить **Particle System** к BlackHole
2. **Shape**: Circle
3. **Emission**: 20 частиц/сек
4. **Start Size**: 0.1-0.2
5. **Start Color**: Фиолетовый с gradient
6. **Velocity over Lifetime**: К центру дыры

---

## 🐛 Troubleshooting:

### Проблема: Дыры не спавнятся
**Решение:**
- ✅ Префаб BlackHole назначен в TrapManager?
- ✅ Randomize On Start включен?
- ✅ Проверь Console на ошибки

### Проблема: Враги не уничтожаются в дыре
**Решение:**
- ✅ У BlackHole есть Circle Collider 2D с **Is Trigger**?
- ✅ У врагов есть тег **"Enemy"**?
- ✅ У врагов есть Rigidbody 2D?

### Проблема: Притяжение не работает
**Решение:**
- ✅ Pull Radius > 0?
- ✅ Pull Force > 0?
- ✅ У объектов есть Rigidbody 2D (не kinematic)?

### Проблема: Дыры спавнятся в странных местах
**Решение:**
- Увеличь **Min Distance Between Holes**
- Уменьши **Max Black Holes**
- Проверь **Arena Radius** (должен быть < 20)

### Проблема: Игрок сразу умирает при старте
**Решение:**
- ✅ **Avoid Player Spawn** = true
- ✅ **Player Safe Radius** = 8+
- У игрока установлен тег **"Player"**?

---

## 💡 Дополнительные фичи (на будущее):

### Разные типы дыр:

**Small Fast Hole** — маленькая, но сильно притягивает
```
Size: 0.5
Pull Force: 10
Pull Radius: 2
```

**Large Slow Hole** — большая, слабо притягивает
```
Size: 2.5
Pull Force: 3
Pull Radius: 5
```

**Pulsing Hole** — пульсирует (меняет силу притяжения)
```csharp
pullForce = basePullForce * (1 + Mathf.Sin(Time.time * 2) * 0.5f);
```

### Временные дыры:
```csharp
public float lifetime = 5f;

void Start() {
    Destroy(gameObject, lifetime);
}
```

### Дыры, которые двигаются:
```csharp
void FixedUpdate() {
    rb.velocity = Vector2.right * moveSpeed;
}
```

---

## ✅ Чеклист настройки:

- [ ] Создан префаб BlackHole (Circle + Collider2D + BlackHole.cs)
- [ ] Создан TrapManager в сцене
- [ ] Префаб назначен в TrapManager
- [ ] Настроены параметры спавна
- [ ] У врагов тег "Enemy"
- [ ] У игрока тег "Player"
- [ ] Gravity Scale = 0 у всех объектов
- [ ] Протестировано: дыры спавнятся рандомно
- [ ] Протестировано: враги уничтожаются в дырах
- [ ] Протестировано: притяжение работает

---

**Готово! Теперь арена стала опаснее! 🕳️💀**
