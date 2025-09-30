# Враг-стрелок - Быстрая настройка ⚡

## 3 шага за 5 минут:

### **Шаг 1: Создать префаб ShooterEnemy**

```
1. Дублировать BasicEnemy → ShooterEnemy
2. Sprite Renderer: Color = темно-красный (#660000)
3. Добавить компонент EnemyShooter
```

**Настройки EnemyShooter:**
- Impulse Radius: `4`
- Impulse Force: `15`
- Impulse Cooldown: `3`
- Min/Max Shoot Distance: `3` / `10`
- Shoot Chance: `0.7`
- Charge Up Time: `0.5`
- Charge Color: Red
- ✅ Avoid Friendly Fire

---

### **Шаг 2: Создать красную волну**

```
1. Дублировать ImpulseWaveEffect → EnemyImpulseEffect
2. Изменить Wave Color на красный (#FF0000)
```

Или создать простой круг:
```
1. 2D Object → Sprite → Circle
2. Color: красный полупрозрачный (#FF0000, Alpha=0.5)
3. Добавить ImpulseEffect скрипт
4. Сделать префаб
```

---

### **Шаг 3: Связать и протестировать**

```
1. ShooterEnemy → Enemy Impulse Prefab = EnemyImpulseEffect
2. Поместить ShooterEnemy в сцену
3. Play!
4. Подойти на 5-7 единиц
5. Враг мигает красным → стреляет!
```

---

## 📊 Сравнение с игроком:

| Параметр | Игрок | ShooterEnemy |
|----------|-------|--------------|
| Radius | 5 | 4 ⬇️ |
| Force | 20 | 15 ⬇️ |
| Cooldown | 1.5 сек | 3 сек ⬇️ |
| Цвет | Синий 🔵 | Красный 🔴 |
| Предупреждение | Нет | Да ✅ |

**Враги слабее, но опаснее толпой!**

---

## 🎮 Тест:

### ✅ Работает если:
1. Враг мигает красным перед выстрелом
2. Красная волна расширяется
3. Игрока отбрасывает
4. Не стреляет если рядом союзники
5. Не стреляет вплотную (< 3) или далеко (> 10)

---

## 🔧 Быстрый баланс:

**Слишком сильный?**
- ⬇️ Impulse Force: 10
- ⬆️ Impulse Cooldown: 5
- ⬇️ Shoot Chance: 0.5

**Слишком слабый?**
- ⬆️ Impulse Force: 20
- ⬇️ Impulse Cooldown: 2
- ⬆️ Shoot Chance: 1.0

**Friendly fire мешает?**
- ❌ Avoid Friendly Fire = false

---

## 💡 Идеи:

### **Волны с ShooterEnemy:**
```
Волна 3: 2 Basic + 1 Shooter
Волна 5: 3 Basic + 2 Shooter
Волна 7: 1 Heavy + 3 Shooter
Волна 10: Boss Shooter (огромный радиус)
```

### **Комбо-атаки:**
- Basic окружают → Shooter стреляет из-за спины
- Heavy блокирует путь → Shooter бьёт издалека
- Shooter толкает игрока → в черную дыру! 🕳️

---

**Готово! Враг-стрелок работает! 🔴💥**
