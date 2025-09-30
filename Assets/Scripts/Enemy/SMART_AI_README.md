# Умный AI врагов - Обход черных дыр 🧠

## ✅ Что добавлено:

1. **Обнаружение дыр на пути**
   - Враг проверяет, есть ли дыра между ним и игроком
   - Использует математику векторов для точной проверки

2. **Режим обхода**
   - Если дыра на пути → враг идёт в случайную точку
   - Длительность обхода: 3 секунды (настраивается)
   - После обхода снова проверяет путь

3. **Безопасное расстояние**
   - Враги держатся подальше от дыр
   - Не подходят слишком близко

---

## 🧠 Как работает AI:

### **Алгоритм поведения:**

```
Каждые 0.5 секунды:
  ├─ Режим обхода активен?
  │   ├─ Да → Идти к случайной точке
  │   └─ Нет → Проверить путь к игроку
  │
  ├─ Найти все черные дыры (тег "BlackHole")
  │
  ├─ Для каждой дыры:
  │   ├─ Проверка 1: Дыра слишком близко?
  │   │   └─ Да → Начать обход
  │   │
  │   └─ Проверка 2: Дыра на пути к игроку?
  │       └─ Да → Начать обход
  │
  └─ Путь свободен → Идти к игроку
```

### **Математика проверки пути:**

```
1. Вектор направления к игроку: directionToPlayer
2. Позиция дыры: holePosition
3. Проекция дыры на линию движения: projectionLength
4. Расстояние от дыры до линии: distanceFromPath

Если distanceFromPath < holeDangerRadius:
   → Дыра на пути! Начать обход
```

---

## 🎮 Настройка в Unity:

### **Шаг 1: Создать тег BlackHole**

1. **Edit → Project Settings → Tags and Layers**
2. **Tags** → нажать **+**
3. Ввести: `BlackHole`
4. Сохранить

### **Шаг 2: Настроить Enemy AI**

В префабе **BasicEnemy** → компонент **EnemyAI**:

| Параметр | Значение | Описание |
|----------|----------|----------|
| **Move Speed** | `5` | Скорость движения |
| **Push Resistance** | `1` | Устойчивость к отбросу |
| **Knockback Recovery Time** | `0.5` | Оглушение после импульса |
| **Black Hole Detection Range** | `6` | Радиус обнаружения дыр |
| **Black Hole Safe Distance** | `2` | Безопасное расстояние от дыры |
| **Random Walk Duration** | `3` | Как долго идти в обход (сек) |
| **Path Check Interval** | `0.5` | Как часто проверять путь (сек) |

### **Шаг 3: Префаб BlackHole**

**ВАЖНО!** Тег устанавливается автоматически через `TrapManager`, но можно задать вручную:

Префаб **BlackHole**:
- **Tag**: `BlackHole` ⚠️ (создать если нет)

---

## 📊 Параметры для разных типов врагов:

### **Basic Orb (обычный)**
```
Move Speed: 5
Black Hole Detection Range: 6
Black Hole Safe Distance: 2
Random Walk Duration: 3
```
→ Средняя скорость, нормальное избегание

### **Smart Orb (умный)**
```
Move Speed: 4
Black Hole Detection Range: 8
Black Hole Safe Distance: 3
Random Walk Duration: 2
```
→ Медленнее, но избегает дыры заранее

### **Dumb Orb (тупой)**
```
Move Speed: 6
Black Hole Detection Range: 3
Black Hole Safe Distance: 1
Random Walk Duration: 4
```
→ Быстрый, но плохо избегает дыры

### **Heavy Orb (тяжелый)**
```
Move Speed: 3
Push Resistance: 2
Black Hole Detection Range: 5
Black Hole Safe Distance: 2
Random Walk Duration: 3
```
→ Медленный, устойчивый к отбросу

---

## 🎯 Визуализация в Scene (Gizmos):

**Зеленая линия** → Путь к игроку (нормальное преследование)  
**Красная линия** → Путь обхода (идёт в случайную точку)  
**Оранжевый круг** → Радиус обнаружения дыр  
**Желтый круг** → Враг оглушён после импульса  

---

## 🔧 Как работает обход:

### **1. Обнаружение дыры:**
```csharp
if (distanceToHole < holeDangerRadius) {
    StartAvoidance(holePosition);
}
```

### **2. Генерация точки обхода:**
```csharp
// Направление ОТ дыры
Vector2 awayFromHole = (позиция_врага - позиция_дыры).normalized;

// Случайный угол отклонения (-90° до +90°)
float randomAngle = Random.Range(-90f, 90f);

// Поворачиваем вектор
Vector2 randomDirection = Rotate(awayFromHole, randomAngle);

// Точка на расстоянии 3-6 единиц
randomTargetPosition = позиция_врага + randomDirection * Random.Range(3, 6);
```

### **3. Движение к точке обхода:**
```csharp
for (3 секунды) {
    Двигаться к randomTargetPosition
}

После 3 секунд:
    isAvoidingBlackHole = false;
    → Снова проверить путь к игроку
```

---

## 📐 Математика проверки пути:

### **Геометрическая визуализация:**

```
          [Игрок]
             ↑
             |
             |  ← Путь
        🕳️  |  ← Дыра БЛИЗКО к пути
             |
             |
          [Враг]

Проекция дыры на линию: ✅
Расстояние от линии: 1 < holeDangerRadius → ОБХОД!
```

```
          [Игрок]
             ↑
             |
             |  ← Путь
             |
             |
          [Враг]
    
    🕳️  ← Дыра ДАЛЕКО от пути

Проекция дыры на линию: ✅
Расстояние от линии: 5 > holeDangerRadius → ПУТЬ СВОБОДЕН!
```

### **Формула:**
```
toHole = holePosition - enemyPosition
projectionLength = dot(toHole, directionToPlayer)

Если projectionLength < 0:
   → Дыра позади, игнорируем

Если projectionLength > distanceToPlayer:
   → Дыра за игроком, игнорируем

closestPoint = enemyPosition + directionToPlayer × projectionLength
distanceFromPath = distance(holePosition, closestPoint)

Если distanceFromPath < holeDangerRadius:
   → Дыра на пути! ОБХОД!
```

---

## 🐛 Troubleshooting:

### Проблема: Враги не обходят дыры
**Решение:**
- ✅ Создан тег `BlackHole`?
- ✅ У префаба BlackHole установлен тег `BlackHole`?
- ✅ `Black Hole Detection Range` достаточно большой? (попробуй 8)
- ✅ Включи Gizmos в Scene view → увидишь радиус обнаружения

### Проблема: Враги застревают
**Решение:**
- Увеличь `Random Walk Duration` до 4-5 секунд
- Увеличь `Black Hole Safe Distance` до 3-4
- Уменьши `Path Check Interval` до 0.3 (чаще проверять)

### Проблема: Враги всё равно падают в дыры
**Решение:**
- Увеличь `Black Hole Safe Distance` до 3+
- Увеличь `Black Hole Detection Range` до 10+
- Проверь, что дыры не слишком большие (size < 2)

### Проблема: Враги слишком умные, игра легкая
**Решение:**
- Уменьши `Black Hole Detection Range` до 3-4
- Уменьши `Black Hole Safe Distance` до 1
- Увеличь `Path Check Interval` до 1-2 (реже проверять)

---

## 💡 Дополнительные улучшения:

### **Вариант 1: Разная сложность AI**

Создай несколько вариантов врагов:

**EasyEnemy** (часто падают в дыры):
- Detection Range: 3
- Safe Distance: 1
- Check Interval: 1.0

**NormalEnemy** (средний AI):
- Detection Range: 6
- Safe Distance: 2
- Check Interval: 0.5

**HardEnemy** (умные):
- Detection Range: 10
- Safe Distance: 3
- Check Interval: 0.3

### **Вариант 2: Группировка врагов**

Враги могут предупреждать друг друга о дырах:
```csharp
void WarnNearbyEnemies(Vector2 holePosition) {
    Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position, 5f);
    foreach (var col in nearby) {
        EnemyAI enemy = col.GetComponent<EnemyAI>();
        if (enemy != null) {
            enemy.StartAvoidance(holePosition);
        }
    }
}
```

### **Вариант 3: Запоминание дыр**

Враг запоминает позиции дыр и избегает их постоянно:
```csharp
List<Vector2> knownBlackHoles = new List<Vector2>();

void RememberBlackHole(Vector2 position) {
    if (!knownBlackHoles.Contains(position)) {
        knownBlackHoles.Add(position);
    }
}
```

---

## ✅ Чеклист настройки:

- [ ] Создан тег `BlackHole` в Project Settings
- [ ] Префаб BlackHole имеет тег `BlackHole`
- [ ] TrapManager автоматически назначает тег (строка 126)
- [ ] У Enemy настроены параметры обхода
- [ ] Включены Gizmos в Scene view для отладки
- [ ] Протестировано: враг обходит дыры ✅
- [ ] Протестировано: после обхода возвращается к игроку ✅

---

## 🎮 Тестирование:

### **Тест 1: Обнаружение дыры**
1. Запусти игру
2. Враг идёт к игроку
3. Дыра появляется на пути
4. Враг останавливается
5. Console: `Enemy_1_1: Дыра на пути к игроку! Начинаю обход.`
6. Враг идёт в сторону (красная линия в Gizmos)

### **Тест 2: Возврат к преследованию**
1. Враг начал обход (красная линия)
2. Через 3 секунды
3. Console: `Enemy_1_1: Закончил обход дыры, возвращаюсь к преследованию`
4. Враг снова идёт к игроку (зеленая линия)

### **Тест 3: Безопасное расстояние**
1. Враг подходит близко к дыре
2. На расстоянии ~2 единицы
3. Враг начинает обход
4. Console: `Enemy_1_1: Слишком близко к дыре! Начинаю обход.`

---

**Готово! Теперь враги умные и не падают в дыры! 🧠✨**
