using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HowToPlayUI : MonoBehaviour
{
    [Header("Navigation")]
    public Button nextButton;
    public Button prevButton;
    public TextMeshProUGUI pageIndicator;

    [Header("Content")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public Image illustrationImage;

    private int currentPage = 0;
    private HowToPlayPage[] pages;

    [System.Serializable]
    private class HowToPlayPage
    {
        public string title;
        [TextArea(5, 10)]
        public string description;
        public Sprite illustration;
    }

    private void Start()
    {
        InitializePages();
        
        if (nextButton != null)
            nextButton.onClick.AddListener(NextPage);
        
        if (prevButton != null)
            prevButton.onClick.AddListener(PreviousPage);
        
        ShowPage(0);
    }

    private void InitializePages()
    {
        pages = new HowToPlayPage[]
        {
            new HowToPlayPage
            {
                title = "🧬 Ласкаво просимо до Football DNA Lab!",
                description = "Ви керуєте футуристичною лабораторією, де створюєте футболістів через мутацію ДНК.\n\n" +
                             "Ваша мета: виконувати замовлення від футбольних команд та збірних, створюючи гравців з потрібними характеристиками."
            },
            new HowToPlayPage
            {
                title = "📋 Замовлення",
                description = "Команди надсилають замовлення з вимогами:\n\n" +
                             "• Назва команди/збірної\n" +
                             "• 3 слоти для гравців\n" +
                             "• Необхідні характеристики (Швидкість, Захист, Стрибки тощо)\n" +
                             "• Бонусні теги для додаткових балів\n\n" +
                             "Виконуйте замовлення якісно, щоб підвищити репутацію!"
            },
            new HowToPlayPage
            {
                title = "🧪 Процес мутації",
                description = "1. Виберіть базового суб'єкта з 3 доступних\n" +
                             "2. Оберіть 2-3 нитки ДНК для злиття\n" +
                             "3. Запустіть процес мутації\n" +
                             "4. Зачекайте завершення (є шанс невдачі!)\n" +
                             "5. Відправте гравця замовнику\n\n" +
                             "Оцінка >90% = успіх і повна оплата!"
            },
            new HowToPlayPage
            {
                title = "🧬 Типи ДНК",
                description = "• 🦁 ТВАРИННА: Газель (швидкість), Горила (сила)\n" +
                             "• ⭐ ЛЕГЕНДАРНА: ДНК відомих футболістів\n" +
                             "• 🌍 ПРИРОДНА: Лід, Лава, Вітер\n" +
                             "• 🤖 МЕХАНІЧНА: Дрон, Магніт, Реактивний ранець\n\n" +
                             "Кожна ДНК має унікальні риси та ефекти!"
            },
            new HowToPlayPage
            {
                title = "🔬 Дослідження",
                description = "Відкривайте нові нитки ДНК через дослідження:\n\n" +
                             "• Виберіть категорію для дослідження\n" +
                             "• Заплатіть гроші\n" +
                             "• Зачекайте 1-3 години\n" +
                             "• Отримайте випадкову ДНК обраної категорії\n\n" +
                             "Покращуйте лабораторію для швидших досліджень!"
            },
            new HowToPlayPage
            {
                title = "⚙️ Покращення лаборatorії",
                description = "Витрачайте гроші на покращення:\n\n" +
                             "• 🧪 Камера мутації - менше шансів невдачі\n" +
                             "• 🔬 Стабілізатор рис - краще збереження рис\n" +
                             "• ⚡ Швидкість досліджень\n" +
                             "• 📚 Бібліотека ДНК - більше місця\n" +
                             "• 🚀 Швидкість мутації"
            },
            new HowToPlayPage
            {
                title = "📈 Репутація",
                description = "Кожна команда має свою репутацію (0-100):\n\n" +
                             "✅ Успішні замовлення підвищують репутацію\n" +
                             "❌ Невдачі знижують репутацію\n\n" +
                             "Висока репутація відкриває:\n" +
                             "• Краще оплачувані замовлення\n" +
                             "• Ексклюзивну ДНК від команд\n" +
                             "• Спеціальні скіни лабораторії"
            },
            new HowToPlayPage
            {
                title = "💡 Поради",
                description = "• Завжди перевіряйте вимоги замовлення\n" +
                             "• Комбінуйте ДНК стратегічно\n" +
                             "• Покращуйте лабораторію регулярно\n" +
                             "• Стежте за репутацією команд\n" +
                             "• Зберігайте гру часто!\n\n" +
                             "Удачі у створенні суперфутболістів! ⚽🧬"
            }
        };
    }

    private void ShowPage(int pageIndex)
    {
        if (pages == null || pages.Length == 0) return;
        
        currentPage = Mathf.Clamp(pageIndex, 0, pages.Length - 1);
        
        HowToPlayPage page = pages[currentPage];
        
        if (titleText != null)
            titleText.text = page.title;
        
        if (descriptionText != null)
            descriptionText.text = page.description;
        
        illustrationImage.gameObject.SetActive(true);
        
        if (pageIndicator != null)
            pageIndicator.text = $"{currentPage + 1} / {pages.Length}";
        
        if (prevButton != null)
            prevButton.interactable = currentPage > 0;
        
        if (nextButton != null)
            nextButton.interactable = currentPage < pages.Length - 1;
    }

    public void NextPage()
    {
        ShowPage(currentPage + 1);
    }

    public void PreviousPage()
    {
        ShowPage(currentPage - 1);
    }
}

