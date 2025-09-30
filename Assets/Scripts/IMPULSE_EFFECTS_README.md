# Улучшенная система отброса врагов 💥

## Что добавлено:

### ✅ **Визуальные эффекты при импульсе:**

1. **Расширяющееся кольцо волны** (ImpulseWaveEffect)
   - Волна расширяется от игрока
   - Постепенно затухает
   - Яркий голубой цвет

2. **Screen Shake** (CameraShakeEffect)
   - Камера трясётся при импульсе
   - Усиливает ощущение удара

3. **Flash эффект на врагах**
   - Враги мигают белым при попадании
   - Мгновенная обратная связь

4. **Улучшенные логи**
   - Показывает количество попаданий
   - Сообщает о промахах

---

## 📋 Настройка эффектов

### Вариант 1: Простой эффект (Circle Sprite)

1. **Создать ImpulseEffect (старый вариант):**
   - `Hierarchy` → `2D Object` → `Sprite` → `Circle`
   - Назвать: `ImpulseEffect`
   - Добавить скрипт `ImpulseEffect`
   - **Sprite Renderer**: голубой полупрозрачный (`#00FFFF`, Alpha = 0.5)
   - Сделать префаб

### Вариант 2: Кольцо волны (LineRenderer) ⭐ РЕКОМЕНДУЕТСЯ

1. **Создать ImpulseWaveEffect:**
   - `Hierarchy` → `Create Empty`
   - Назвать: `ImpulseWaveEffect`
   - Добавить скрипт `ImpulseWaveEffect`
   - **Настройки скрипта**:
     - Expand Speed: `30`
     - Max Radius: `10`
     - Lifetime: `0.5`
     - Wave Color: голубой (`#00FFFF`)
   - Сделать префаб

### Вариант 3: Оба эффекта одновременно 🔥

**Создать составной префаб:**

1. `Create Empty` → `ImpulseEffectCombo`
2. Дочерний объект: `Circle Sprite` (заливка)
3. Дочерний объект: `Empty` с `ImpulseWaveEffect` (кольцо)
4. Сделать префаб

---

## 🎮 Настройка PlayerController

1. Выбрать **Player** в Hierarchy
2. В компоненте **PlayerController**:
   - **Impulse Prefab**: перетащить `ImpulseWaveEffect` (или любой другой)
   - **Screen Shake Duration**: `0.2`
   - **Screen Shake Intensity**: `0.3`

---

## 🎯 Параметры отброса

### В PlayerController:

| Параметр | Значение | Описание |
|----------|----------|----------|
| **Impulse Radius** | `5` | Радиус действия импульса |
| **Impulse Force** | `20` | Сила отброса врагов |
| **Impulse Cooldown** | `1.5` | Перезарядка в секундах |

### Рекомендуемые настройки:

**Для быстрого геймплея:**
- Radius: `6`
- Force: `25`
- Cooldown: `1.0`

**Для тактического геймплея:**
- Radius: `4`
- Force: `15`
- Cooldown: `2.0`

**Для хаоса:**
- Radius: `8`
- Force: `30`
- Cooldown: `0.5`

---

## 🔧 Настройка врагов для правильного отброса

### В BasicEnemy (префаб):

1. **Rigidbody 2D**:
   - Mass: `1` (обычный)
   - Linear Drag: `1-2` (чтобы останавливались)
   - Angular Drag: `5`
   - ⚠️ **Gravity Scale: `0`**
   - ✅ **Freeze Rotation Z**

2. **Circle Collider 2D**:
   - ✅ **Is Trigger** (для детекта касания игрока)
   - Radius: `0.4`

### Heavy Orb (тяжёлый враг):

В **EnemyAI**:
- **Push Resistance**: `2` (в 2 раза тяжелее отбросить)

Rigidbody 2D автоматически получит Mass = 2

---

## 📊 Как это работает:

### 1. Игрок нажимает Space/ЛКМ
```
PlayerController.TryUseImpulse()
  ↓
Проверка кулдауна
  ↓
UseImpulse()
```

### 2. Эффекты активируются
```
UseImpulse()
  ├── Создать визуальный эффект (ImpulseWaveEffect)
  ├── Screen Shake камеры
  └── Physics2D.OverlapCircleAll()
```

### 3. Отброс врагов
```
Для каждого Collider2D в радиусе:
  ├── Получить Rigidbody2D
  ├── Рассчитать направление (от игрока к врагу)
  ├── AddForce(direction * impulseForce)
  └── Вспышка белым (flash эффект)
```

### 4. Враги летят
```
Rigidbody2D получает импульс
  ↓
Враг отбрасывается в направлении от игрока
  ↓
ArenaBoundary проверяет выход за границу
  ↓
Если вышел → Destroy + OnEnemyDied()
```

---

## 🎨 Дополнительные улучшения

### Particle System вместо Circle

1. `GameObject` → `Effects` → `Particle System`
2. Настройки:
   - **Duration**: `0.5`
   - **Start Lifetime**: `0.3`
   - **Start Speed**: `10`
   - **Start Size**: `0.2`
   - **Start Color**: голубой
   - **Emission** → Rate: `100`
   - **Shape** → Circle, Radius: `0.1`
3. Сделать префаб

### Trail на врагах при отбросе

В **EnemyAI** добавить:
```csharp
void OnImpulseHit() {
    TrailRenderer trail = GetComponent<TrailRenderer>();
    if (trail) trail.emitting = true;
    Invoke("StopTrail", 0.5f);
}
```

### Звук импульса

1. Добавить **Audio Source** к Player
2. Назначить звук "whoosh" или "blast"
3. В `UseImpulse()` вызвать `audioSource.Play()`

---

## 🐛 Troubleshooting

### Проблема: Враги не отбрасываются
**Решение:**
- ✅ У врагов есть Rigidbody 2D?
- ✅ Gravity Scale = 0?
- ✅ Impulse Force достаточно большой? (попробуй 30)

### Проблема: Враги отбрасываются слишком сильно/слабо
**Решение:**
- Увеличь/уменьши **Impulse Force** в PlayerController
- Измени **Mass** врагов в Rigidbody 2D
- Измени **Linear Drag** (больше = быстрее останавливаются)

### Проблема: Screen Shake не работает
**Решение:**
- Камера находится в `(0, 0, -10)`?
- Скрипт `CameraShakeEffect` добавится автоматически при первом импульсе

### Проблема: Эффект волны не виден
**Решение:**
- Проверь **Sorting Layer** = "Effects"
- LineRenderer имеет материал?
- Wave Color имеет Alpha > 0?

### Проблема: Flash эффект не работает
**Решение:**
- У врага есть **SpriteRenderer**?
- У врага установлен тег **"Enemy"**?

---

## ✨ Результат

**При правильной настройке:**
1. 🎯 Игрок нажимает Space
2. 💥 Яркая волна расширяется от игрока
3. 📹 Камера трясётся
4. ⚡ Враги мигают белым
5. 🚀 Враги отбрасываются в стороны
6. 💀 Враги вылетают за границу и уничтожаются
7. 📊 В Console выводится: "💪 Попадание! Врагов отброшено: 3"

---

**Удачи! 🎮✨**
