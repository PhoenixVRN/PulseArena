# Pulse Arena - Инструкция по настройке (2D версия)

## 📋 Шаг 1: Настройка сцены для 2D

### 1.1 Переключить проект в 2D режим
1. **Edit** → **Project Settings** → **Editor**
2. **Default Behavior Mode**: установить **2D**
3. Закрыть Project Settings

### 1.2 Настроить камеру для 2D
1. Выбрать **Main Camera** в **Hierarchy**
2. **Transform**:
   - Position: `(0, 0, -10)` — стандартная позиция для 2D
   - Rotation: `(0, 0, 0)`
3. **Camera**:
   - Projection: **Orthographic** ✅
   - Size: `15` (охватывает арену)
   - Background: черный или космический цвет

---

## 📋 Шаг 2: Создать арену

### 2.1 Создать фон арены
1. **Hierarchy** → **2D Object** → **Sprite** → **Circle**
2. Назвать: `Arena`
3. **Transform**:
   - Position: `(0, 0, 0)`
   - Scale: `(40, 40, 1)` — круглая арена диаметр 40 единиц

### 2.2 Настроить материал арены
1. **Sprite Renderer**:
   - Color: тёмно-синий или серый (`#1a1a2e`)

### 2.3 Создать визуальную границу (опционально)
1. **Hierarchy** → **2D Object** → **Sprite** → **Circle**
2. Назвать: `ArenaBorder`
3. **Transform**:
   - Position: `(0, 0, 0)`
   - Scale: `(40.5, 40.5, 1)` — чуть больше арены
4. **Sprite Renderer**:
   - Color: красный полупрозрачный (`#FF0000` с Alpha = 0.3)
5. Убрать **Collider** если есть

---

## 📋 Шаг 3: Создать игрока

### 3.1 Создать объект игрока
1. **Hierarchy** → **2D Object** → **Sprite** → **Circle**
2. Назвать: `Player`
3. **Transform**:
   - Position: `(0, 0, 0)`
   - Scale: `(1, 1, 1)`

### 3.2 Настроить компоненты
1. Добавить **Rigidbody 2D**:
   - Mass: `1`
   - Linear Drag: `2` (чтобы плавно останавливался)
   - Angular Drag: `5`
   - Gravity Scale: `0` ⚠️ **ВАЖНО! Ноль для top-down игры**
   - ✅ Freeze Rotation → Z (чтобы не вращался)

2. Добавить **Circle Collider 2D** (должен быть по умолчанию):
   - Radius: `0.5`

3. Добавить **скрипт** `PlayerController`:
   - Move Speed: `10`
   - Max Speed: `15`
   - Impulse Radius: `5`
   - Impulse Force: `20`
   - Impulse Cooldown: `1.5`

4. **Tag**: установить `Player`
   - **Inspector** → **Tag** → **Add Tag** → создать тег `Player`
   - Применить к объекту Player

### 3.3 Материал игрока
1. **Sprite Renderer**:
   - Color: **Синий** (`#0080FF`)

---

## 📋 Шаг 4: Создать врага (префаб)

### 4.1 Создать объект врага
1. **Hierarchy** → **2D Object** → **Sprite** → **Circle**
2. Назвать: `BasicEnemy`
3. **Transform**:
   - Position: `(5, 5, 0)` — временно в стороне
   - Scale: `(0.8, 0.8, 1)` — чуть меньше игрока

### 4.2 Настроить компоненты
1. Добавить **Rigidbody 2D**:
   - Mass: `1`
   - Linear Drag: `1`
   - Gravity Scale: `0` ⚠️ **ВАЖНО!**
   - ✅ Freeze Rotation → Z

2. Добавить **Circle Collider 2D**:
   - ✅ **Is Trigger** (чтобы детектить касание с игроком)
   - Radius: `0.4`

3. Добавить **скрипт** `EnemyAI`:
   - Move Speed: `5`
   - Push Resistance: `1` (для Heavy Orb = 2)

4. **Tag**: установить `Enemy`
   - **Inspector** → **Tag** → **Add Tag** → создать тег `Enemy`
   - Применить к BasicEnemy

### 4.3 Материал врага
1. **Sprite Renderer**:
   - Color: **Красный** (`#FF0000`)

### 4.4 Сделать префаб
1. Создать папку: **Assets** → **Create** → **Folder** → `Prefabs`
2. Перетащить `BasicEnemy` из **Hierarchy** в папку `Assets/Prefabs/`
3. Удалить `BasicEnemy` из сцены (он будет спавниться GameManager'ом)

---

## 📋 Шаг 5: Создать эффект импульса (префаб)

### 5.1 Создать объект эффекта
1. **Hierarchy** → **2D Object** → **Sprite** → **Circle**
2. Назвать: `ImpulseEffect`
3. **Transform**:
   - Position: `(0, 0, 0)`
   - Scale: `(0.5, 0.5, 1)`

### 5.2 Настроить компоненты
1. **Убрать** Circle Collider 2D (не нужен)

2. Добавить **скрипт** `ImpulseEffect`:
   - Expand Duration: `0.3`
   - Max Scale: `10`

### 5.3 Материал эффекта
1. **Sprite Renderer**:
   - Color: **Голубой полупрозрачный** (`#00FFFF` с Alpha = 0.5)

### 5.4 Улучшенный визуал (опционально)
Если используешь **Universal Render Pipeline (URP)**:
1. **Assets** → **Create** → **Material** → `ImpulseMaterial_2D`
2. Shader: **Sprites/Default** или **Shader Graph**
3. Добавить **свечение** через 2D Lights (опционально)

### 5.5 Сделать префаб
1. Перетащить `ImpulseEffect` в `Assets/Prefabs/`
2. Удалить из сцены

---

## 📋 Шаг 6: Настроить GameManager

### 6.1 Создать объект GameManager
1. **Hierarchy** → **Create Empty**
2. Назвать: `GameManager`
3. **Transform**: Position `(0, 0, 0)`

### 6.2 Добавить компоненты
1. Добавить **скрипт** `GameManager`:
   - **Enemy Prefab**: перетащить `BasicEnemy` префаб из Prefabs
   - Arena Center: `null` (будет создан автоматически в (0,0))
   - Spawn Radius: `15`
   - Initial Enemy Count: `3`
   - Time Between Waves: `3`
   - Enemy Increase Per Wave: `1`

---

## 📋 Шаг 7: Настроить границы арены

### 7.1 Создать объект границы
1. **Hierarchy** → **Create Empty**
2. Назвать: `ArenaBoundary`

### 7.2 Добавить скрипт
1. Добавить **скрипт** `ArenaBoundary`:
   - Arena Radius: `20` (половина размера арены)
   - Arena Center: `(0, 0)`

---

## 📋 Шаг 8: Связать всё вместе

### 8.1 Связать PlayerController с эффектом
1. Выбрать `Player` в **Hierarchy**
2. В компоненте `PlayerController`:
   - **Impulse Prefab**: перетащить префаб `ImpulseEffect` из Prefabs

---

## 📋 Шаг 9: Настройка слоёв (Sorting Layers)

### 9.1 Создать слои для правильного отображения
1. **Edit** → **Project Settings** → **Tags and Layers**
2. **Sorting Layers** → добавить слои:
   - `Background` (0) — арена
   - `Game` (1) — игрок и враги
   - `Effects` (2) — импульсы и эффекты
   - `UI` (3) — интерфейс

### 9.2 Применить слои к объектам
- **Arena**: Sorting Layer = `Background`
- **Player**: Sorting Layer = `Game`, Order in Layer = `1`
- **BasicEnemy (префаб)**: Sorting Layer = `Game`, Order in Layer = `0`
- **ImpulseEffect (префаб)**: Sorting Layer = `Effects`

---

## 🎮 Шаг 10: Тестирование

### 10.1 Проверить теги
- ✅ Player имеет тег `Player`
- ✅ Префаб врага имеет тег `Enemy`

### 10.2 Проверить Rigidbody 2D
- ✅ **Gravity Scale = 0** у игрока и врагов
- ✅ Freeze Rotation Z включен

### 10.3 Запустить игру
1. Нажать **Play** ▶️
2. Управление:
   - **WASD** или **Стрелки** — движение
   - **Пробел** или **ЛКМ** — импульс

### 10.4 Что должно работать:
- ✅ Игрок двигается в 2D плоскости
- ✅ Враги спавнятся по кругу и идут к игроку
- ✅ Импульс отбрасывает врагов от игрока
- ✅ Эффект импульса расширяется и исчезает
- ✅ Враги исчезают за границей арены
- ✅ Новые волны спавнятся автоматически
- ✅ Счёт увеличивается при убийстве врагов

---

## 🐛 Возможные проблемы

### Проблема: Объекты падают вниз
**Решение**: Установить **Gravity Scale = 0** у всех Rigidbody 2D

### Проблема: Враги не двигаются
**Решение**: 
- Проверить, что у игрока установлен тег `Player`
- Проверить, что Gravity Scale = 0

### Проблема: Импульс не работает
**Решение**: 
- Проверить, что у врагов есть `Rigidbody 2D`
- Проверить радиус импульса (должен быть ~5)
- Проверить, что префаб эффекта назначен в PlayerController

### Проблема: Враги проходят сквозь игрока
**Решение**: 
- У врага Circle Collider 2D должен быть **Is Trigger = true**
- У игрока Circle Collider 2D должен быть обычным (не триггером)

### Проблема: GameManager не спавнит врагов
**Решение**: 
- Проверить, что префаб врага назначен в Inspector
- Проверить Console на ошибки

### Проблема: Объекты не видны
**Решение**:
- Проверить Sorting Layers
- Убедиться что Camera Z = -10
- Проверить что у объектов есть Sprite Renderer

---

## 📝 Следующие шаги (TODO)

После успешного тестирования базового прототипа:

### Phase 2: Core Features
- [ ] Добавить систему здоровья игрока (3 HP)
- [ ] Создать UI (счёт, волна, HP, кулдаун)
- [ ] Добавить урон при касании врага
- [ ] Создать Game Over экран

### Phase 3: Разнообразие
- [ ] Создать Heavy Orb (медленный, устойчивый)
- [ ] Создать Fast Orb (быстрый, слабый)
- [ ] Добавить ловушки (дыры в арене)
- [ ] Добавить визуальные эффекты (частицы, trail)

### Phase 4: Полировка
- [ ] Добавить звуки (импульс, смерть, волна)
- [ ] Улучшить визуал (2D Lights, свечение)
- [ ] Добавить screen shake при импульсе
- [ ] Создать меню и паузу

---

## 🎨 Дополнительные улучшения визуала

### Использование 2D Lights (URP)
1. **Window** → **Rendering** → **Lighting**
2. Добавить **2D Light** к игроку (голубое свечение)
3. Добавить **2D Light** к врагам (красное свечение)

### Particle System для импульса
1. Заменить простой круг на **Particle System**
2. Частицы разлетаются от центра
3. Эффект хроматической аберрации через PostProcessing

### Trail Renderer
1. Добавить **Trail Renderer** к игроку
2. Голубой след при движении

---

## 🎯 Цель прототипа

**Базовый геймплей цикл:**
1. Враги спавнятся → идут к игроку
2. Игрок уворачивается и использует импульс
3. Враги отбрасываются за границу
4. Волна завершена → следующая волна
5. Сложность растёт → больше врагов, быстрее
6. Game Over при касании или выходе за границу

**Если это работает — прототип готов! 🎉**

---

**Удачи! 🎮✨**