using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen Instance { get; private set; }

    [Header("UI Elements")]
    public GameObject loadingPanel;
    public Image loadingBar;
    public TextMeshProUGUI loadingText;
    public TextMeshProUGUI tipText;

    [Header("Settings")]
    public float minLoadingTime = 2f;
    
    private string[] loadingTips = new string[]
    {
        "💡 Комбінуйте ДНК стратегічно для кращих результатів!",
        "🧬 Кожна риса впливає на певні характеристики гравця",
        "⚡ Покращуйте лабораторію для швидших мутацій",
        "📈 Висока репутація відкриває кращі замовлення",
        "🔬 Досліджуйте нові типи ДНК регулярно",
        "💰 Виконуйте замовлення з точністю >90% для максимальної оплати",
        "🎯 Зверніть увагу на бонусні теги у замовленнях",
        "⏱️ Мутації можуть провалитися - покращте камеру мутації!",
        "🦁 Тваринна ДНК чудово підходить для фізичних характеристик",
        "⭐ Легендарна ДНК дає унікальні комбінації рис",
        "🌍 Природна ДНК додає спеціальні здібності",
        "🤖 Механічна ДНК покращує технічні навички",
        "💾 Не забувайте зберігати прогрес!",
        "🔄 Оновлення суб'єктів коштує $50",
        "📊 Перевіряйте статистику перед відправкою гравця"
    };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }
    }

    public void ShowLoading(string message = "Завантаження...")
    {
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
        }

        if (loadingText != null)
        {
            loadingText.text = message;
        }

        if (tipText != null)
        {
            tipText.text = GetRandomTip();
        }

        if (loadingBar != null)
        {
            loadingBar.fillAmount = 0f;
        }
    }

    public void HideLoading()
    {
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }
    }

    public IEnumerator LoadSceneAsync(string sceneName)
    {
        ShowLoading($"Завантаження {sceneName}...");

        float startTime = Time.time;
        
        // Start async load
        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        // Update progress bar
        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            
            if (loadingBar != null)
            {
                loadingBar.fillAmount = progress;
            }

            if (loadingText != null)
            {
                loadingText.text = $"Завантаження... {(progress * 100):F0}%";
            }

            // Check if loading is complete
            if (asyncLoad.progress >= 0.9f)
            {
                // Wait for minimum loading time
                float elapsedTime = Time.time - startTime;
                if (elapsedTime < minLoadingTime)
                {
                    yield return new WaitForSeconds(minLoadingTime - elapsedTime);
                }

                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }

        HideLoading();
    }

    private string GetRandomTip()
    {
        if (loadingTips.Length == 0)
            return "";

        return loadingTips[Random.Range(0, loadingTips.Length)];
    }
}

