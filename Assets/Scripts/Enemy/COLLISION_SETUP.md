# Настройка коллизий врагов с игроком 💥

## Проблема:
Враги не толкают игрока → игрок не может быть вытолкнут за арену или в дыру

## Решение:
Правильная настройка коллайдеров + компонент **EnemyPusher**

---

## 📋 Пошаговая настройка:

### **Шаг 1: Настроить Player (ВАЖНО!)**

**Player → Circle Collider 2D:**
```
❌ Is Trigger = FALSE  ← ОБЯЗАТЕЛЬНО!
Radius: 0.5
```

**Player → Rigidbody 2D:**
```
Mass: 1
Linear Drag: 2
Angular Drag: 5
Gravity Scale: 0
Constraints:
  ✅ Freeze Rotation Z
```

**Collision Detection:**
```
Collision Detection: Continuous
```

---

### **Шаг 2: Настроить BasicEnemy (и другие типы)**

#### **Вариант A: Два коллайдера** ⭐ РЕКОМЕНДУЕТСЯ

**Collider 1 - Физический (для толкания):**
```
Circle Collider 2D:
  ❌ Is Trigger = FALSE
  Radius: 0.4
```

**Collider 2 - Триггер (для детекта урона):**
```
Circle Collider 2D #2:
  ✅ Is Trigger = TRUE
  Radius: 0.5 (чуть больше)
```

**Rigidbody 2D:**
```
Mass: 1
Linear Drag: 1
Gravity Scale: 0
Collision Detection: Continuous
Constraints:
  ✅ Freeze Rotation Z
```

**Добавить EnemyPusher:**
```
Add Component → EnemyPusher
  Push Force: 5
  Continuous Push: ✅
```

#### **Вариант B: Один физический коллайдер** (проще)

**Circle Collider 2D:**
```
❌ Is Trigger = FALSE
Radius: 0.4
```

**Rigidbody 2D:**
```
Mass: 1
Linear Drag: 1
Gravity Scale: 0
Collision Detection: Continuous
```

**Добавить EnemyPusher:**
```
Add Component → EnemyPusher
  Push Force: 5
  Continuous Push: ✅
```

**EnemyAI изменить:**
```csharp
// Вместо OnTriggerEnter2D использовать OnCollisionEnter2D
void OnCollisionEnter2D(Collision2D collision)
{
    if (collision.gameObject.CompareTag("Player"))
    {
        Debug.Log("⚠️ Враг коснулся игрока!");
        // TODO: Система урона
    }
}
```

---

### **Шаг 3: Повторить для всех типов врагов**

**Для каждого префаба:**
- BasicEnemy ✅
- ShooterEnemy ✅
- FastEnemy ✅
- DasherEnemy ✅
- TeleporterEnemy ✅
- TankEnemy ✅

**Добавить:**
1. Circle Collider 2D (Is Trigger = FALSE)
2. Rigidbody 2D (настроен правильно)
3. **EnemyPusher** компонент

---

## 🎯 Как работает выталкивание:

```
1. Враг касается игрока (OnCollisionEnter2D)
   ↓
2. EnemyPusher вычисляет направление от врага к игроку
   ↓
3. AddForce толкает игрока в этом направлении
   ↓
4. Игрок отлетает от врага
   ↓
5. Если толпа врагов → игрок может быть вытолкнут за арену!
   ↓
6. ArenaBoundary → Game Over
```

### **Сила выталкивания:**

| Параметр | Слабо | Средне | Сильно |
|----------|-------|--------|--------|
| **Push Force** | 3 | 5 | 10 |
| **Continuous Push** | ❌ | ✅ | ✅ |

**Continuous Push = true** → враг постоянно толкает пока касается (опаснее!)

---

## 🔧 Параметры для разных врагов:

### **BasicEnemy:**
```
Push Force: 5
Continuous Push: ✅
```

### **FastEnemy:**
```
Push Force: 3 (легче, быстрее)
Continuous Push: ✅
```

### **TankEnemy:**
```
Push Force: 10 (СИЛЬНО толкает!)
Continuous Push: ✅
```

### **DasherEnemy:**
```
Push Force: 8 (толкает при рывке)
Continuous Push: ❌ (только при ударе)
```

---

## 💡 Тактика толпы:

### **Как враги могут убить игрока:**

**Сценарий 1: Окружение**
```
  Enemy   Enemy
     \   /
      Player  ← Враги со всех сторон
     /   \
  Enemy   Enemy

→ Каждый враг толкает игрока
→ Силы суммируются
→ Игрок выталкивается в случайном направлении
→ Может попасть в дыру или за границу!
```

**Сценарий 2: Загон к краю**
```
Enemy → Player → [ГРАНИЦА АРЕНЫ]

→ Враги толкают к краю
→ Игрок не может отойти (край близко)
→ Игрок выходит за границу
→ Game Over!
```

**Сценарий 3: Загон к черной дыре**
```
Enemy → Player → 🕳️

→ Враги толкают к дыре
→ Игрок падает в дыру
→ Game Over!
```

---

## 🐛 Troubleshooting:

### Проблема: Враги всё равно не толкают игрока
**Решение:**
- ✅ У Player Circle Collider 2D **Is Trigger = FALSE**?
- ✅ У Enemy Circle Collider 2D **Is Trigger = FALSE**?
- ✅ У обоих Rigidbody 2D?
- ✅ Gravity Scale = 0 у обоих?
- ✅ EnemyPusher компонент добавлен?

### Проблема: Враги толкают слишком сильно
**Решение:**
- Уменьшить Push Force до 2-3
- Отключить Continuous Push
- Увеличить Linear Drag у Player до 3-4

### Проблема: Враги толкают слишком слабо
**Решение:**
- Увеличить Push Force до 8-10
- Включить Continuous Push
- Уменьшить Linear Drag у Player до 1

### Проблема: Игрок пролетает сквозь врагов
**Решение:**
- Collision Detection = Continuous (и у Player, и у Enemy)
- Проверить что оба коллайдера НЕ триггеры

### Проблема: Враги застревают друг в друге
**Решение:**
- Добавить Physics Material 2D:
  ```
  Friction: 0.1
  Bounciness: 0.2
  ```
- Назначить всем врагам и игроку

### Проблема: EnemyAI не работает (OnTriggerEnter2D)
**Решение:**

Если используешь **Вариант B** (один коллайдер), замени в EnemyAI.cs:

```csharp
// БЫЛО:
void OnTriggerEnter2D(Collider2D other) {
    if (other.CompareTag("Player")) {
        Debug.Log("⚠️ Враг коснулся игрока!");
    }
}

// СТАЛО:
void OnCollisionEnter2D(Collision2D collision) {
    if (collision.gameObject.CompareTag("Player")) {
        Debug.Log("⚠️ Враг коснулся игрока!");
    }
}
```

---

## ✅ Быстрый чеклист:

### **Player:**
- [ ] Circle Collider 2D: Is Trigger = **FALSE** ✅
- [ ] Rigidbody 2D: Gravity Scale = **0** ✅
- [ ] Collision Detection = **Continuous** ✅

### **Enemy (каждый префаб):**
- [ ] Circle Collider 2D: Is Trigger = **FALSE** ✅
- [ ] Rigidbody 2D: Gravity Scale = **0** ✅
- [ ] Collision Detection = **Continuous** ✅
- [ ] **EnemyPusher** компонент добавлен ✅
- [ ] Push Force = **5** ✅

### **Тест:**
- [ ] Запустить игру
- [ ] Подойти к врагу
- [ ] Враг толкает игрока ✅
- [ ] Console: "⚠️ Enemy_1_1 толкает игрока!" ✅

---

## 🎮 Игровой цикл:

```
1. Враги спавнятся
   ↓
2. EnemyAI двигает их к игроку
   ↓
3. Враг касается игрока
   ↓
4. OnCollisionEnter2D → EnemyPusher.PushPlayer()
   ↓
5. AddForce толкает игрока
   ↓
6. Игрок отлетает
   ↓
7. Если толпа → игрок может быть вытолкнут за границу
   ↓
8. ArenaBoundary → Game Over
```

---

## 💪 Дополнительные улучшения:

### **Урон при контакте:**

```csharp
void OnCollisionEnter2D(Collision2D collision)
{
    if (collision.gameObject.CompareTag("Player"))
    {
        PlayerHealth health = collision.gameObject.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.TakeDamage(1);
        }
    }
}
```

### **Отбрасывание врагов друг от друга:**

```csharp
void OnCollisionEnter2D(Collision2D collision)
{
    if (collision.gameObject.CompareTag("Enemy"))
    {
        // Враги отталкиваются друг от друга
        Rigidbody2D otherRb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (otherRb != null)
        {
            Vector2 pushDir = (rb.position - otherRb.position).normalized;
            rb.AddForce(pushDir * 2f, ForceMode2D.Impulse);
        }
    }
}
```

---

**Готово! Теперь враги физически толкают игрока! 💥**
