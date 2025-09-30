# Руководство по типам врагов 🎭

## Обзор всех типов:

| Тип | Механика | Сложность | Особенность |
|-----|----------|-----------|-------------|
| **BasicEnemy** | Преследует игрока | ⭐ | Базовый враг |
| **ShooterEnemy** | Стреляет импульсом | ⭐⭐ | Дальний бой |
| **FastEnemy** | Зигзаг + рывки | ⭐⭐ | Непредсказуемый |
| **DasherEnemy** | Врывается к игроку | ⭐⭐⭐ | Опасный рывок |
| **TeleporterEnemy** | Телепортируется | ⭐⭐⭐ | Внезапные атаки |
| **TankEnemy** | Требует много ударов | ⭐⭐⭐⭐ | Босс-враг |

---

## 1. BasicEnemy (Базовый) ⚪

**Файл:** `EnemyAI.cs`

### Характеристики:
- HP: 1 удар
- Скорость: средняя (5)
- Размер: обычный (1.0)
- Цвет: **Красный** `#FF0000`

### Поведение:
- Преследует игрока
- Обходит черные дыры
- Умирает от 1 импульса

### Настройки:
```
Move Speed: 5
Push Resistance: 1
Black Hole Detection Range: 6
```

### Когда использовать:
- Волны 1-3 (начало игры)
- Заполнение пространства
- Комбо с другими типами

---

## 2. ShooterEnemy (Стрелок) 🔴

**Файл:** `EnemyShooter.cs`

### Характеристики:
- HP: 1 удар
- Скорость: средняя (5)
- Размер: обычный (1.0)
- Цвет: **Темно-красный** `#660000`
- Дальность стрельбы: 3-10 единиц

### Поведение:
- Преследует игрока
- Мигает красным 0.5 сек → стреляет импульсом
- Избегает friendly fire
- Кулдаун: 3 секунды

### Настройки:
```
Impulse Radius: 4
Impulse Force: 15
Impulse Cooldown: 3
Shoot Chance: 0.7
Avoid Friendly Fire: ✅
```

### Когда использовать:
- Волна 3+ (когда игрок привык к Basic)
- Комбо: Basic окружают → Shooter стреляет сзади
- Boss-волны (3-5 Shooters)

---

## 3. FastEnemy (Быстрый) 🔵

**Файл:** `FastEnemy.cs`

### Характеристики:
- HP: 1 удар
- Скорость: **высокая** (7-8)
- Размер: **маленький** (0.7)
- Масса: **легкая** (0.5)
- Цвет: **Голубой** `#00FFFF`

### Поведение:
- Зигзаг-движение (непредсказуемо)
- Рывки каждые 2 сек (×2 скорость)
- Легко отбрасывается импульсом

### Настройки:
```
Move Speed: 8
Zigzag Frequency: 2
Zigzag Amplitude: 2
Burst Speed Multiplier: 2
Burst Cooldown: 2
```

### Когда использовать:
- Волна 4+ (добавляет хаос)
- Комбо: Tank + Fast (танк блокирует, Fast атакует)
- Массовые атаки (5-7 Fast врагов)

---

## 4. DasherEnemy (Врывающийся) 🟡

**Файл:** `DasherEnemy.cs`

### Характеристики:
- HP: 1 удар
- Скорость: **рывок** (20)
- Размер: обычный (1.0)
- Цвет: **Желтый** `#FFFF00`
- Дальность рывка: 8 единиц

### Поведение:
- Стоит на месте
- Мигает желтым 0.8 сек → рывок к игроку
- Trail эффект при рывке
- После рывка → резкое торможение

### Настройки:
```
Dash Speed: 20
Dash Distance: 8
Dash Cooldown: 3
Charge Up Time: 0.8
```

### Когда использовать:
- Волна 5+ (опасные атаки)
- Комбо: Shooter отбрасывает → Dasher врывается
- Узкие пространства (трудно уклониться)

---

## 5. TeleporterEnemy (Телепортер) 🟣

**Файл:** `TeleporterEnemy.cs`

### Характеристики:
- HP: 1 удар
- Скорость: средняя (5)
- Размер: обычный (1.0)
- Цвет: **Фиолетовый** `#8800FF`
- Дальность телепорта: 3-8 единиц

### Поведение:
- Телепортируется вокруг игрока каждые 4 сек
- Fade out → телепорт → fade in (0.3 сек анимация)
- Появляется в случайной точке вокруг игрока

### Настройки:
```
Teleport Cooldown: 4
Min/Max Teleport Distance: 3 / 8
Fade Time: 0.3
```

### Когда использовать:
- Волна 6+ (сложные враги)
- Комбо: Teleporter окружает → другие атакуют
- Арены с дырами (телепортируется рядом)

---

## 6. TankEnemy (Танк) ⚫

**Файл:** `TankEnemy.cs`

### Характеристики:
- HP: **5 ударов** импульсом!
- Скорость: **медленная** (3)
- Размер: **большой** (1.5)
- Масса: **тяжелая** (3.0)
- Цвет: **Серый/Черный** `#333333`
- Health bar: зеленый → красный

### Поведение:
- Медленно преследует
- Требует 5 импульсов для уничтожения
- Мигает белым при ударе
- Health bar показывает состояние
- Уменьшает отброс на 50%

### Настройки:
```
Hits To Destroy: 5
Armor Reduction: 0.5
Move Speed: 3 (в EnemyAI)
```

### Когда использовать:
- Волна 7+ (мини-боссы)
- Boss-волны (1 Tank + 5 Basic)
- Финальные волны (2-3 Tank'а)

---

## 🎯 Стратегии комбинирования:

### **Волна 1-2: Обучение**
```
3-4 BasicEnemy
```
→ Игрок учится управлению

### **Волна 3-4: Первые вызовы**
```
2 BasicEnemy + 1 ShooterEnemy
```
→ Добавляем дальний бой

### **Волна 5-6: Хаос**
```
1 BasicEnemy + 2 FastEnemy + 1 ShooterEnemy
```
→ Быстрые враги + обстрел

### **Волна 7-8: Тактика**
```
1 TankEnemy + 3 BasicEnemy + 1 ShooterEnemy
```
→ Танк блокирует, Basic окружают, Shooter стреляет

### **Волна 9-10: Экстрим**
```
1 DasherEnemy + 2 TeleporterEnemy + 2 FastEnemy
```
→ Непредсказуемость максимум

### **Boss Волна (10+):**
```
2 TankEnemy + 3 ShooterEnemy + 2 DasherEnemy
```
→ Танки впереди, стрелки сзади, врывающиеся с флангов

---

## 📋 Таблица создания префабов:

| Префаб | Базовый | Компоненты | Цвет | Scale |
|--------|---------|------------|------|-------|
| **BasicEnemy** | ✅ | EnemyAI | #FF0000 | 1.0 |
| **ShooterEnemy** | BasicEnemy | +EnemyShooter | #660000 | 1.0 |
| **FastEnemy** | BasicEnemy | +FastEnemy | #00FFFF | 0.7 |
| **DasherEnemy** | BasicEnemy | +DasherEnemy | #FFFF00 | 1.0 |
| **TeleporterEnemy** | BasicEnemy | +TeleporterEnemy | #8800FF | 1.0 |
| **TankEnemy** | BasicEnemy | +TankEnemy | #333333 | 1.5 |

---

## 🔧 Настройка EnemyAI для каждого типа:

### BasicEnemy:
```
Move Speed: 5
Push Resistance: 1
```

### ShooterEnemy:
```
Move Speed: 5
Push Resistance: 1
```

### FastEnemy:
```
Move Speed: 8
Push Resistance: 0.5
```

### DasherEnemy:
```
Move Speed: 4 (медленно ходит)
Push Resistance: 1
```

### TeleporterEnemy:
```
Move Speed: 5
Push Resistance: 1
```

### TankEnemy:
```
Move Speed: 3
Push Resistance: 2
```

---

## 🎨 Визуальные улучшения:

### FastEnemy:
- Добавить **Trail Renderer** (голубой след)
- **Particle System** при рывке

### DasherEnemy:
- **Trail Renderer** уже есть ✅
- Добавить звук "whoosh" при рывке

### TeleporterEnemy:
- **Teleport Effect** префаб (частицы)
- Звук телепортации

### TankEnemy:
- **Health Bar** уже есть ✅
- Добавить звук удара "clang"

---

## 🐛 Troubleshooting:

### Проблема: FastEnemy слишком хаотичный
**Решение:**
- Уменьшить Zigzag Amplitude до 1
- Увеличить Burst Cooldown до 3

### Проблема: DasherEnemy застревает
**Решение:**
- Проверить что Collision Detection = Continuous
- Добавить Physics Material без трения

### Проблема: TeleporterEnemy телепортируется в дыры
**Решение:**
- В GetRandomTeleportPosition() добавить проверку:
```csharp
// Проверка на черные дыры
Collider2D[] holes = Physics2D.OverlapCircleAll(targetPosition, 2f);
foreach (var hole in holes) {
    if (hole.CompareTag("BlackHole")) {
        return GetRandomTeleportPosition(); // Рекурсия
    }
}
```

### Проблема: TankEnemy умирает от 1 удара
**Решение:**
- Проверить что TankEnemy компонент добавлен
- Проверить что PlayerController вызывает TakeHit()

---

## ✅ Чеклист создания всех врагов:

- [ ] BasicEnemy (уже должен быть)
- [ ] ShooterEnemy + красная волна
- [ ] FastEnemy (маленький, быстрый)
- [ ] DasherEnemy (желтый с trail)
- [ ] TeleporterEnemy (фиолетовый)
- [ ] TankEnemy (большой, серый, с HP bar)
- [ ] Создать тег BlackHole
- [ ] Протестировать каждый тип
- [ ] Настроить GameManager для спавна разных типов

---

**Готово! Теперь у тебя 6 уникальных типов врагов! 🎭✨**
