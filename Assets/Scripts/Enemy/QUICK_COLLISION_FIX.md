# БЫСТРОЕ ИСПРАВЛЕНИЕ: Враги не толкают игрока ⚡

## Проблема:
Враги проходят сквозь игрока, не толкают его

## Решение за 3 шага:

---

### **Шаг 1: Исправить Player**

**Player → Inspector:**
```
Circle Collider 2D:
  ❌ Is Trigger = FALSE  ← ИЗМЕНИТЬ!

Rigidbody 2D:
  Gravity Scale: 0
  Collision Detection: Continuous
  Freeze Rotation: Z ✅
```

---

### **Шаг 2: Исправить BasicEnemy (и все другие префабы)**

**BasicEnemy → Inspector:**
```
Circle Collider 2D:
  ❌ Is Trigger = FALSE  ← ИЗМЕНИТЬ!
  Radius: 0.4

Rigidbody 2D:
  Gravity Scale: 0
  Collision Detection: Continuous
  Freeze Rotation: Z ✅

Добавить компонент:
  Add Component → EnemyPusher
    Push Force: 5
    Continuous Push: ✅
```

**Повторить для:**
- ShooterEnemy
- FastEnemy
- DasherEnemy
- TeleporterEnemy
- TankEnemy

---

### **Шаг 3: Протестировать**

```
1. Play
2. Подойти к врагу
3. Враг должен толкать игрока!
4. Console: "⚠️ Enemy_1_1 толкает игрока!"
```

---

## ⚠️ ВАЖНО:

**Player коллайдер:** Is Trigger = **FALSE**  
**Enemy коллайдер:** Is Trigger = **FALSE**  
**Оба:** Gravity Scale = **0**  

---

## 🎯 Результат:

✅ Враги толкают игрока  
✅ Толпа может вытолкнуть за границу  
✅ Игрок может быть вытолкнут в дыру  
✅ Game Over работает  

---

**Готово! 💥**
