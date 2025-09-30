# Враг-стрелок с импульсом 🔴💥

## Что это?

**EnemyShooter** — враг, который может использовать импульс как игрок!

### Особенности:
✅ Стреляет импульсом в радиусе  
✅ **Визуальное предупреждение** перед выстрелом (мигает красным)  
✅ Избегает friendly fire (не стреляет по союзникам)  
✅ Настраиваемая дальность и частота стрельбы  
✅ Работает вместе с EnemyAI (движение + стрельба)  

---

## 🎮 Быстрая настройка:

### **Шаг 1: Создать префаб ShooterEnemy**

1. **Дублировать префаб BasicEnemy**
2. Назвать: `ShooterEnemy`
3. **Sprite Renderer**: изменить цвет на **темно-красный** (`#660000`)
4. **Добавить компонент EnemyShooter**

### **Шаг 2: Создать красную волну импульса**

1. **Дублировать префаб ImpulseWaveEffect** (или ImpulseEffect)
2. Назвать: `EnemyImpulseEffect`
3. Изменить цвет волны на **красный** (`#FF0000`)
4. Если используешь `ImpulseWaveEffect.cs`:
   - Wave Color: красный `#FF0000`

### **Шаг 3: Настроить EnemyShooter**

В префабе **ShooterEnemy** → компонент **EnemyShooter**:

| Параметр | Значение | Описание |
|----------|----------|----------|
| **Impulse Radius** | `4` | Радиус импульса (меньше чем у игрока) |
| **Impulse Force** | `15` | Сила отброса (слабее чем у игрока) |
| **Impulse Cooldown** | `3` | Перезарядка (дольше чем у игрока) |
| **Shoot Range** | `8` | Дистанция обнаружения цели |
| **Min Shoot Distance** | `3` | Не стреляет вплотную |
| **Max Shoot Distance** | `10` | Максимальная дальность |
| **Shoot Chance** | `0.7` | Шанс выстрела (0-1) |
| **Enemy Impulse Prefab** | `EnemyImpulseEffect` | Префаб красной волны |
| **Charge Up Time** | `0.5` | Время подготовки перед выстрелом |
| **Charge Color** | Красный | Цвет мигания |
| **Shoot At Player** | ✅ | Стреляет в игрока |
| **Avoid Friendly Fire** | ✅ | Избегает попадания по союзникам |

### **Шаг 4: Настроить GameManager**

В **GameManager** добавить возможность спавна ShooterEnemy:

```csharp
[SerializeField] private GameObject shooterEnemyPrefab;
[SerializeField] private int shooterSpawnWave = 3; // С какой волны появляются

void SpawnWave() {
    if (currentWave >= shooterSpawnWave) {
        // Спавним 1 ShooterEnemy
        SpawnEnemy(shooterEnemyPrefab);
    }
}
```

---

## 📊 Баланс параметров:

### **Weak Shooter (слабый стрелок)**
```
Impulse Radius: 3
Impulse Force: 10
Impulse Cooldown: 4
Shoot Chance: 0.5
```
→ Редко стреляет, слабый импульс

### **Normal Shooter (обычный)** ⭐ Рекомендуется
```
Impulse Radius: 4
Impulse Force: 15
Impulse Cooldown: 3
Shoot Chance: 0.7
```
→ Сбалансированный враг

### **Strong Shooter (сильный)**
```
Impulse Radius: 5
Impulse Force: 20
Impulse Cooldown: 2.5
Shoot Chance: 0.8
```
→ Часто стреляет, опасный

### **Sniper (снайпер)**
```
Impulse Radius: 6
Impulse Force: 25
Impulse Cooldown: 5
Min Shoot Distance: 6
Max Shoot Distance: 15
Shoot Chance: 1.0
```
→ Стреляет издалека, мощно, но редко

---

## 🧠 Как работает AI:

### **Поведение:**

```
1. EnemyAI двигает врага к игроку
   ↓
2. EnemyShooter проверяет каждый кадр:
   ├─ Кулдаун готов?
   ├─ Игрок в диапазоне (3-10 единиц)?
   ├─ Нет союзников в радиусе импульса?
   └─ Случайный шанс (70%)?
   ↓
3. Если всё ✅ → Начать подготовку
   ↓
4. Мигание красным 0.5 секунды
   ↓
5. ВЫСТРЕЛ! 💥
   ↓
6. Кулдаун 3 секунды
   ↓
7. Повторить
```

### **Логика friendly fire:**

```csharp
bool WouldHitAllies() {
    Collider2D[] nearby = OverlapCircleAll(position, impulseRadius);
    
    foreach (col in nearby) {
        if (col.tag == "Enemy" && col != self) {
            return true; // Союзник в зоне поражения!
        }
    }
    
    return false;
}
```

Если в радиусе есть другие враги → **не стреляет**

---

## 🎯 Тактика игрока:

### **Против ShooterEnemy:**

1. **Заметить предупреждение** (мигает красным)
2. **Уклониться или отбежать** (0.5 сек на реакцию)
3. **Контратаковать своим импульсом**
4. **Использовать других врагов как щит** (friendly fire защищает)

### **Комбо с обычными врагами:**

- Толпа Basic врагов → ShooterEnemy сзади
- Игрок окружён → ShooterEnemy стреляет из-за спины
- Создаёт давление, вынуждает двигаться

---

## 📐 Визуализация (Gizmos):

В **Scene View** при выборе ShooterEnemy:

- 🔴 **Красный круг** → Радиус импульса (4 единицы)
- 🟡 **Внутренний желтый круг** → Min Shoot Distance (3)
- 🟡 **Внешний желтый круг** → Max Shoot Distance (10)

**Зона стрельбы** = между двумя желтыми кругами

---

## 🎨 Визуальные улучшения:

### **Вариант 1: Простое мигание** ✅ (уже реализовано)
```
Мигает красным 0.5 сек → Выстрел
```

### **Вариант 2: Накопление энергии**

Добавить дочерний объект **Sprite → Circle**:
- Назвать: `ChargeIndicator`
- Scale: `(0, 0, 1)` → за 0.5 сек растёт до `(1.5, 1.5, 1)`
- Color: красный полупрозрачный
- Исчезает после выстрела

```csharp
IEnumerator ChargeVisual() {
    chargeIndicator.SetActive(true);
    float t = 0;
    while (t < chargeUpTime) {
        t += Time.deltaTime;
        float scale = Mathf.Lerp(0, 1.5f, t / chargeUpTime);
        chargeIndicator.transform.localScale = Vector3.one * scale;
        yield return null;
    }
    chargeIndicator.SetActive(false);
}
```

### **Вариант 3: Частицы**

Добавить **Particle System**:
- **Emission**: возрастает перед выстрелом
- **Color**: красный
- **Shape**: Circle вокруг врага

---

## 🔧 Расширенные настройки:

### **Предсказуемый стрелок:**

```csharp
[SerializeField] private bool predictableShots = true;
[SerializeField] private float fixedShootInterval = 5f;

void Update() {
    if (predictableShots) {
        if (Time.time % fixedShootInterval < 0.1f) {
            ForceShoot();
        }
    }
}
```

Стреляет каждые 5 секунд точно → игрок может предсказать

### **Стрельба очередями:**

```csharp
IEnumerator BurstShoot(int shots) {
    for (int i = 0; i < shots; i++) {
        Shoot();
        yield return new WaitForSeconds(0.3f);
    }
}
```

Выстрел → 0.3 сек → Выстрел → 0.3 сек → Выстрел

### **Прицеливание в игрока:**

```csharp
void Update() {
    if (player != null) {
        // Поворачиваемся к игроку перед выстрелом
        Vector2 direction = player.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
```

Враг поворачивается в сторону игрока

---

## 🎮 Варианты врагов-стрелков:

### **1. Shotgun Enemy** (дробовик)
```
Impulse Radius: 6
Impulse Force: 10
Impulse Cooldown: 4
Min Distance: 1
Max Distance: 5
```
→ Стреляет вплотную, широкий радиус, слабая сила

### **2. Cannon Enemy** (пушка)
```
Impulse Radius: 3
Impulse Force: 30
Impulse Cooldown: 6
Min Distance: 5
Max Distance: 12
```
→ Издалека, узкий радиус, огромная сила

### **3. Machine Gun Enemy** (пулемёт)
```
Impulse Radius: 2
Impulse Force: 8
Impulse Cooldown: 1
Shoot Chance: 1.0
```
→ Часто стреляет, слабо, маленький радиус

### **4. Boss Shooter** (босс)
```
Impulse Radius: 8
Impulse Force: 25
Impulse Cooldown: 4
Charge Up Time: 1.5
Avoid Friendly Fire: false
```
→ Мощный, большой радиус, долгая подготовка, не боится союзников

---

## 🐛 Troubleshooting:

### Проблема: Враг не стреляет
**Решение:**
- ✅ Prefab `EnemyImpulseEffect` назначен?
- ✅ Игрок в диапазоне 3-10 единиц?
- ✅ Shoot Chance > 0?
- ✅ Проверь Console на логи

### Проблема: Враг стреляет слишком часто/редко
**Решение:**
- Измени **Shoot Chance** (0.5 = реже, 1.0 = чаще)
- Измени **Impulse Cooldown** (больше = реже)

### Проблема: Враг стреляет по союзникам
**Решение:**
- ✅ **Avoid Friendly Fire** = true
- Проверь, что у всех врагов тег `Enemy`

### Проблема: Не вижу предупреждения
**Решение:**
- Увеличь **Charge Up Time** до 1 секунды
- Измени **Charge Color** на более контрастный

### Проблема: Импульс врага слишком слабый/сильный
**Решение:**
- Измени **Impulse Force** (10-30)
- Измени **Impulse Radius** (3-6)

---

## ✅ Чеклист настройки:

- [ ] Создан префаб `ShooterEnemy` (BasicEnemy + EnemyShooter)
- [ ] Создан префаб `EnemyImpulseEffect` (красная волна)
- [ ] EnemyShooter настроен (radius, force, cooldown)
- [ ] Enemy Impulse Prefab назначен
- [ ] Изменён цвет врага (красный/темный)
- [ ] Протестировано: враг мигает перед выстрелом ✅
- [ ] Протестировано: враг стреляет импульсом ✅
- [ ] Протестировано: friendly fire работает ✅
- [ ] GameManager спавнит ShooterEnemy (опционально)

---

## 🎯 Тестирование:

### **Тест 1: Базовая стрельба**
```
1. Создать ShooterEnemy в сцене
2. Запустить игру
3. Подойти к врагу на расстояние 5-7 единиц
4. Враг мигает красным
5. Через 0.5 сек → красная волна
6. Игрока отбрасывает
```

### **Тест 2: Friendly Fire**
```
1. Создать ShooterEnemy + 2 BasicEnemy рядом
2. Подойти так, чтобы BasicEnemy были между тобой и ShooterEnemy
3. ShooterEnemy НЕ должен стрелять (союзники в радиусе)
4. Отойди в сторону
5. ShooterEnemy стреляет
```

### **Тест 3: Дистанция**
```
1. Подойти вплотную (< 3 единиц)
   → ShooterEnemy НЕ стреляет
2. Отойти на 5 единиц
   → ShooterEnemy стреляет
3. Отойти на 15 единиц (> 10)
   → ShooterEnemy НЕ стреляет
```

---

**Готово! Теперь у тебя есть опасные враги-стрелки! 🔴💥**

**Следующие улучшения:**
- Разные типы стрелков (Shotgun, Cannon, Sniper)
- Boss с мега-импульсом
- Комбинации Basic + Shooter + Heavy в одной волне
- Визуальные эффекты накопления энергии

Что хочешь добавить? 🎮✨
