# 🎮 UI Setup Guide для Football DNA Lab

## 📋 Структура UI в одній сцені

### 1. Canvas Structure

```
Canvas (Screen Space - Overlay)
├── MainMenuPanel (MainMenuManager GameObject тут)
│   ├── Title Text
│   ├── New Game Button
│   ├── Continue Button
│   ├── How To Play Button
│   ├── Settings Button
│   ├── Credits Button
│   └── Quit Button
│
├── GameUI (UIManager GameObject тут)
│   ├── HUD (постійно видима панель зверху)
│   │   ├── MoneyText (TextMeshPro)
│   │   ├── AutoSaveIndicator (TextMeshPro)
│   │   ├── MenuButton (кнопка паузи)
│   │   └── SaveGameButton (ручне збереження)
│   │
│   ├── NavigationPanel (панель навігації між розділами)
│   │   ├── LabButton → викликає UIManager.ShowLabPanel()
│   │   ├── OrdersButton → викликає UIManager.ShowOrdersPanel()
│   │   ├── ResearchButton → викликає UIManager.ShowResearchPanel()
│   │   └── UpgradesButton → викликає UIManager.ShowUpgradesPanel()
│   │
│   ├── LabPanel
│   │   ├── SubjectsContainer (тут з'являються картки суб'єктів)
│   │   ├── RefreshSubjectsButton
│   │   └── RefreshCostText
│   │
│   ├── OrdersPanel
│   │   └── OrdersContainer (тут з'являються картки замовлень)
│   │
│   ├── ResearchPanel
│   │   ├── ResearchAnimalButton
│   │   ├── ResearchLegendaryButton
│   │   ├── ResearchEnvironmentButton
│   │   ├── ResearchMechanicalButton
│   │   └── ResearchStatusText
│   │
│   ├── UpgradesPanel
│   │   └── UpgradesContainer (тут з'являються картки покращень)
│   │
│   ├── MutationPanel
│   │   ├── DNASelectionContainer
│   │   ├── MutationStatusText
│   │   ├── StartMutationButton
│   │   └── CancelMutationButton
│   │
│   └── PauseMenuPanel
│       ├── ResumeButton
│       ├── SaveButton
│       ├── SettingsButton
│       └── MainMenuButton (повернутися в головне меню)
│
└── Managers (порожні GameObject)
    ├── GameManager (компонент GameManager.cs)
    ├── UIManager (компонент UIManager.cs)
    ├── MainMenuManager (компонент MainMenuManager.cs)
    ├── SaveSystem (компонент SaveSystem.cs)
    ├── AudioManager (компонент AudioManager.cs)
    └── LoadingScreen (компонент LoadingScreen.cs)
```

## 🔧 Налаштування UIManager в Unity Inspector

### 1. Створіть GameObject "UIManager" в сцені
- Add Component → UIManager.cs

### 2. Прив'яжіть всі панелі в Inspector:

**Panels:**
- Main Menu Panel → перетягніть MainMenuPanel GameObject
- Lab Panel → перетягніть LabPanel GameObject
- Orders Panel → перетягніть OrdersPanel GameObject
- Research Panel → перетягніть ResearchPanel GameObject
- Upgrades Panel → перетягніть UpgradesPanel GameObject
- Mutation Panel → перетягніть MutationPanel GameObject
- Pause Menu Panel → перетягніть PauseMenuPanel GameObject

**HUD Elements:**
- Money Text → TextMeshPro компонент для відображення грошей
- Menu Button → кнопка для відкриття паузи
- Save Game Button → кнопка ручного збереження
- Auto Save Indicator → TextMeshPro для таймера автозбереження

**Navigation Buttons (ВАЖЛИВО!):**
- Lab Button → кнопка "Лабораторія"
- Orders Button → кнопка "Замовлення"
- Research Button → кнопка "Дослідження"
- Upgrades Button → кнопка "Покращення"

**Lab Panel Elements:**
- Subjects Container → порожній GameObject з Vertical/Horizontal Layout Group
- Subject Card Prefab → префаб картки суб'єкта (з Assets/Prefabs/)
- Refresh Subjects Button → кнопка оновлення суб'єктів
- Refresh Cost Text → текст "Refresh: $50"

**Orders Panel Elements:**
- Orders Container → порожній GameObject з Layout Group
- Order Card Prefab → префаб картки замовлення

**Research Panel Elements:**
- Research Animal Button → кнопка дослідження тваринної ДНК
- Research Legendary Button → кнопка дослідження легендарної ДНК
- Research Environment Button → кнопка дослідження природної ДНК
- Research Mechanical Button → кнопка дослідження механічної ДНК
- Research Status Text → текст статусу

**Upgrades Panel Elements:**
- Upgrades Container → порожній GameObject з Layout Group
- Upgrade Card Prefab → префаб картки покращення

**Mutation Panel Elements:**
- DNA Selection Container → порожній GameObject з Layout Group
- DNA Card Prefab → префаб картки ДНК
- Mutation Status Text → текст статусу мутації
- Start Mutation Button → кнопка запуску мутації
- Cancel Mutation Button → кнопка скасування

## 🎯 Як працюють кнопки навігації

Коли гравець натискає на кнопку навігації:

```
LabButton.onClick → UIManager.ShowLabPanel()
    ↓
Ховає всі панелі (Orders, Research, Upgrades)
    ↓
Показує LabPanel
    ↓
Викликає RefreshLabUI() - оновлює список суб'єктів
```

## 📝 Важливі моменти

### 1. Початковий стан панелей
Встановіть в Inspector:
- MainMenuPanel → Active ✅
- Всі інші панелі (Lab, Orders, Research, Upgrades, Mutation, Pause) → Inactive ❌
- HUD елементи (Money, AutoSave, кнопки) → Inactive ❌

### 2. Navigation Panel завжди видима
Створіть окрему панель NavigationPanel, яка буде завжди видима під час гри:
- Position: Bottom або Side
- Містить 4 кнопки: Lab, Orders, Research, Upgrades

### 3. Прив'язка кнопок до методів

**Автоматично (через код):**
- Navigation buttons → автоматично прив'язуються в InitializeButtons()
- Research buttons → автоматично прив'язуються
- Mutation buttons → автоматично прив'язуються

**Вручну (якщо потрібно):**
Ви можете також прив'язати в Inspector:
- Button → OnClick() → UIManager → ShowLabPanel()

## 🎨 Приклад Layout для NavigationPanel

```
NavigationPanel (Horizontal Layout Group)
├── Background Image
├── LabButton (з іконкою 🧪)
├── OrdersButton (з іконкою 📋)
├── ResearchButton (з іконкою 🔬)
└── UpgradesButton (з іконкою ⚙️)
```

**Settings для Horizontal Layout Group:**
- Spacing: 10
- Child Alignment: Middle Center
- Child Force Expand: Width ✅, Height ✅

## 🐛 Troubleshooting

### Проблема: Кнопки не працюють
**Рішення:**
1. Перевірте, чи UIManager.Instance не null
2. Перевірте, чи кнопки прив'язані в Inspector
3. Перевірте Console на помилки
4. Переконайтеся, що на кнопках є компонент Button

### Проблема: Панелі не перемикаються
**Рішення:**
1. Перевірте, чи всі панелі прив'язані в UIManager Inspector
2. Перевірте, чи GameObject панелей активні (але самі панелі можуть бути неактивними)
3. Подивіться в Console - UIManager виводить Debug.Log при помилках

### Проблема: Після повернення з меню кнопки не працюють
**Рішення:**
1. Переконайтеся, що UIManager НЕ знищується при поверненні в меню
2. Перевірте, що GameManager правильно викликає HideMenu() та ShowLabPanel()

## ✅ Checklist налаштування

- [ ] Створено Canvas з усіма панелями
- [ ] Створено GameObject "UIManager" з компонентом UIManager.cs
- [ ] Прив'язано всі панелі в Inspector
- [ ] Прив'язано HUD елементи
- [ ] Прив'язано Navigation buttons (Lab, Orders, Research, Upgrades)
- [ ] Створено NavigationPanel з 4 кнопками
- [ ] Всі кнопки мають компонент Button
- [ ] Початковий стан: MainMenu active, решта inactive
- [ ] Протестовано перемикання між панелями
- [ ] Протестовано кнопки Refresh, Research, Mutation

## 🎮 Тестування

1. **Запустіть гру** → має з'явитися MainMenu
2. **Натисніть "New Game"** → має з'явитися LabPanel з суб'єктами
3. **Натисніть кнопку "Orders"** → має переключитися на OrdersPanel
4. **Натисніть кнопку "Research"** → має переключитися на ResearchPanel
5. **Натисніть кнопку "Upgrades"** → має переключитися на UpgradesPanel
6. **Натисніть кнопку "Lab"** → має повернутися до LabPanel
7. **Натисніть ESC** → має з'явитися PauseMenu
8. **Натисніть "Return to Menu"** → має повернутися в MainMenu

Якщо все працює - налаштування завершено! ✅

