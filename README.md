# 🧬 Football DNA Lab

Futuristic lab simulation where you mutate DNA to create custom football players for teams and nations.

## 🎮 Game Features

- **DNA Mutation System** - Combine DNA strands to create custom players
- **Research System** - Unlock new DNA types (Animal, Legendary, Environment, Mechanical)
- **Laboratory Upgrades** - Improve mutation success rates and speed
- **Order Management** - Fulfill team requests for reputation and rewards
- **Auto-Save System** - Game saves automatically every 3 minutes and on exit

## 🚀 Setup Instructions

### Unity Scene Setup

**ВАЖЛИВО:** Гра повинна починатися з головного меню!

1. **Створіть сцену MainMenu:**
   - File → New Scene → створіть сцену з назвою "MainMenu"
   - Збережіть її в `Assets/Scenes/MainMenu.unity`

2. **Build Settings:**
   - File → Build Settings
   - Додайте сцени в такому порядку:
     - **Scene 0:** MainMenu (головне меню - стартова сцена)
     - **Scene 1:** SampleScene (ігрова сцена)

3. **GameObject Setup в MainMenu сцені:**
   - Створіть порожній GameObject з назвою "GameManager"
   - Додайте компонент `GameManager.cs`
   - Встановіть параметри:
     - Main Menu Scene Name: "MainMenu"
     - Game Scene Name: "SampleScene"

4. **Менеджери (DontDestroyOnLoad):**
   Додайте в MainMenu сцену наступні GameObject:
   - GameManager (GameManager.cs)
   - SaveSystem (SaveSystem.cs)
   - AudioManager (AudioManager.cs)
   - LoadingScreen (LoadingScreen.cs)

5. **UI в MainMenu:**
   - MainMenuPanel з кнопками:
     - New Game → викликає `MainMenuManager.OnNewGame()`
     - Continue → викликає `MainMenuManager.OnContinueGame()`
     - How To Play → відкриває інструкції
     - Settings → відкриває налаштування
     - Credits → відкриває титри
     - Quit → викликає `MainMenuManager.OnQuitGame()`

## 🎯 Як працює запуск гри

1. **Запуск Unity:** Гра завжди починається з MainMenu сцени
2. **New Game:** Видаляє всі збереження → завантажує ігрову сцену
3. **Continue:** Завантажує ігрову сцену → автоматично завантажує збережений прогрес
4. **Return to Menu:** Зберігає прогрес → повертається до MainMenu

## 💾 Auto-Save System

Гра зберігається автоматично:
- ✅ Кожні 3 хвилини (якщо є незбережені зміни)
- ✅ При покупці покращень
- ✅ При завершенні мутації
- ✅ При завершенні дослідження
- ✅ При виході з гри
- ✅ При поверненні в головне меню

## 🔧 Scripts Overview

### Core Systems
- `GameManager.cs` - Управління станом гри та сценами
- `SaveSystem.cs` - Автоматичне збереження/завантаження
- `MainMenuManager.cs` - Управління головним меню

### Game Systems
- `MutationSystem.cs` - Система мутації гравців
- `ResearchSystem.cs` - Дослідження нової ДНК
- `OrderManager.cs` - Система замовлень від команд
- `LabUpgradeManager.cs` - Покращення лабораторії
- `DNALibrary.cs` - Бібліотека ДНК

### UI Systems
- `UIManager.cs` - Управління ігровим UI
- `NotificationManager.cs` - Система сповіщень
- `LoadingScreen.cs` - Екран завантаження з порадами

### Data Classes
- `PlayerStats.cs` - Статистика гравців
- `BaseSubject.cs` - Базові суб'єкти для мутації
- `DNAStrand.cs` - Характеристики ДНК
- `TeamOrder.cs` - Замовлення від команд

## 📝 Important Notes

- **Серіалізація Unity:** Не використовуйте `Random.Range` в конструкторах!
  - ✅ Використовуйте: `PlayerStats.CreateRandom()`
  - ✅ Використовуйте: `BaseSubject.CreateRandom()`
  - ❌ Не використовуйте: `new PlayerStats()` або `new BaseSubject()` для випадкових даних

- **Scene Names:** Переконайтеся, що назви сцен в GameManager співпадають з реальними назвами

## 🎨 UI Structure

```
MainMenu Scene:
├── Canvas
│   ├── MainMenuPanel
│   │   ├── Title
│   │   ├── NewGameButton
│   │   ├── ContinueButton
│   │   ├── HowToPlayButton
│   │   ├── SettingsButton
│   │   ├── CreditsButton
│   │   └── QuitButton
│   ├── SettingsPanel
│   ├── HowToPlayPanel
│   ├── CreditsPanel
│   └── ConfirmationDialog
└── Managers (DontDestroyOnLoad)
    ├── GameManager
    ├── SaveSystem
    ├── AudioManager
    └── LoadingScreen

Game Scene (SampleScene):
├── Canvas
│   ├── HUD
│   │   ├── MoneyText
│   │   ├── AutoSaveIndicator
│   │   └── MenuButton
│   ├── LabPanel
│   ├── OrdersPanel
│   ├── ResearchPanel
│   ├── UpgradesPanel
│   ├── MutationPanel
│   └── PauseMenuPanel
└── Game Managers
    ├── UIManager
    ├── SubjectGenerator
    ├── OrderManager
    ├── ResearchSystem
    ├── MutationSystem
    └── DNALibrary
```

## 🎮 Game Flow

```
Unity Start
    ↓
MainMenu Scene загружається
    ↓
GameManager.InitializeGame()
    ↓
Гравець вибирає:
    ├─→ New Game → Скидає прогрес → SampleScene
    └─→ Continue → Завантажує прогрес → SampleScene
```

## © 2025 Football DNA Lab
Futuristic Sports Simulation Game
