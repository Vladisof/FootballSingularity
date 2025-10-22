using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    public GameState currentState = GameState.MainMenu;

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
    }

    private void Start()
    {
        // Initialize all systems
        InitializeGame();
    }

    private void InitializeGame()
    {
        Debug.Log("Football DNA Lab - Game Initialized!");
        
        // Завжди починаємо з головного меню
        currentState = GameState.MainMenu;
        ShowMainMenu();
    }

    private void ShowMainMenu()
    {
        // Показати головне меню, сховати ігрові панелі
        if (UIManager.Instance != null)
        {
            UIManager.Instance.HideAllGamePanels();
        }
        
        if (MainMenuManager.Instance != null)
        {
            MainMenuManager.Instance.ShowMenu();
        }
    }

    public void StartNewGame()
    {
        // Скинути весь прогрес перед новою грою
        ResetGame();
        currentState = GameState.Lab;
        
        // Сховати меню, показати гру
        if (MainMenuManager.Instance != null)
        {
            MainMenuManager.Instance.HideMenu();
        }
        
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowLabPanel();
        }
    }

    public void ContinueGame()
    {
        currentState = GameState.Lab;
        
        // Завантажити збережену гру
        LoadGameProgress();
        
        // Сховати меню, показати гру
        if (MainMenuManager.Instance != null)
        {
            MainMenuManager.Instance.HideMenu();
        }
        
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowLabPanel();
        }
    }

    private void LoadGameProgress()
    {
        // Завантажити збережену гру
        if (SaveSystem.Instance != null)
        {
            bool loaded = SaveSystem.Instance.LoadGame();
            if (loaded)
            {
                Debug.Log("Гру завантажено з збереження!");
            }
            else
            {
                Debug.Log("Розпочато нову гру!");
            }
        }
    }

    public void ReturnToMainMenu()
    {
        currentState = GameState.MainMenu;
        
        // Зберегти перед поверненням в меню
        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.SaveGame();
        }
        
        // Сховати гру, показати меню
        if (UIManager.Instance != null)
        {
            UIManager.Instance.HideAllGamePanels();
        }
        
        if (MainMenuManager.Instance != null)
        {
            MainMenuManager.Instance.ShowMenu();
        }
    }

    public void ResetGame()
    {
        // Reset all progress
        if (MoneyController.Instance != null)
        {
            MoneyController.Instance.ResetToDefault();
        }

        if (ReputationManager.Instance != null)
        {
            ReputationManager.Instance.ResetAllReputations();
        }

        if (LabUpgradeManager.Instance != null)
        {
            LabUpgradeManager.Instance.ResetAllUpgrades();
        }

        if (DNALibrary.Instance != null)
        {
            DNALibrary.Instance.ResetLibrary();
        }

        if (SubjectGenerator.Instance != null)
        {
            SubjectGenerator.Instance.ResetSubjects();
        }

        if (OrderManager.Instance != null)
        {
            OrderManager.Instance.ResetOrders();
        }

        if (CreatedPlayersManager.Instance != null)
        {
            CreatedPlayersManager.Instance.ResetPlayers();
        }

        // Видалити збережену гру
        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.DeleteSaveData();
        }

        Debug.Log("Game progress reset!");
    }

    public void SaveGame()
    {
        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.SaveGame();
        }
        else
        {
            PlayerPrefs.Save();
            Debug.Log("Game saved!");
        }
    }

    public void QuitGame()
    {
        SaveGame();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}

public enum GameState
{
    MainMenu,
    Lab,
    Orders,
    Research,
    Upgrades,
    Mutation
}
