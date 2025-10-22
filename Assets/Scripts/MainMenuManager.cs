using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance { get; private set; }

    [Header("Menu Panels")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    public GameObject creditsPanel;
    public GameObject howToPlayPanel;

    [Header("Main Menu Buttons")]
    public Button newGameButton;
    public Button continueButton;
    public Button howToPlayButton;
    public Button settingsButton;
    public Button creditsButton;
    public Button quitButton;

    [Header("Settings Panel")]
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Toggle fullscreenToggle;
    public Button backFromSettingsButton;

    [Header("How To Play Panel")]
    public Button backFromHowToPlayButton;

    [Header("Credits Panel")]
    public Button backFromCreditsButton;

    [Header("Confirmation Dialog")]
    public GameObject confirmationDialog;
    public TextMeshProUGUI confirmationText;
    public Button confirmYesButton;
    public Button confirmNoButton;

    private System.Action currentConfirmAction;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeMenu();
        LoadSettings();
        CheckSaveData();
        
        // Play main menu music
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMainMenuMusic();
        }
    }

    private void InitializeMenu()
    {
        // Show main menu by default
        ShowMainMenu();

        // Setup button listeners
        if (newGameButton != null)
            newGameButton.onClick.AddListener(OnNewGame);
        
        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinueGame);
        
        if (howToPlayButton != null)
            howToPlayButton.onClick.AddListener(ShowHowToPlay);
        
        if (settingsButton != null)
            settingsButton.onClick.AddListener(ShowSettings);
        
        if (creditsButton != null)
            creditsButton.onClick.AddListener(ShowCredits);
        
        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitGame);

        // Back buttons
        if (backFromSettingsButton != null)
            backFromSettingsButton.onClick.AddListener(ShowMainMenu);
        
        if (backFromHowToPlayButton != null)
            backFromHowToPlayButton.onClick.AddListener(ShowMainMenu);
        
        if (backFromCreditsButton != null)
            backFromCreditsButton.onClick.AddListener(ShowMainMenu);

        // Settings listeners
        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        
        if (fullscreenToggle != null)
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);

        // Confirmation dialog
        if (confirmYesButton != null)
            confirmYesButton.onClick.AddListener(OnConfirmYes);
        
        if (confirmNoButton != null)
            confirmNoButton.onClick.AddListener(OnConfirmNo);

        if (confirmationDialog != null)
            confirmationDialog.SetActive(false);
    }

    private void CheckSaveData()
    {
        // Check if save data exists
        bool hasSaveData = PlayerPrefs.HasKey("HasSaveData");
        
        if (continueButton != null)
            continueButton.interactable = hasSaveData;
    }

    // Panel Navigation
    private void ShowMainMenu()
    {
        HideAllPanels();
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
    }

    private void ShowSettings()
    {
        HideAllPanels();
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    private void ShowHowToPlay()
    {
        HideAllPanels();
        if (howToPlayPanel != null)
            howToPlayPanel.SetActive(true);
    }

    private void ShowCredits()
    {
        HideAllPanels();
        if (creditsPanel != null)
            creditsPanel.SetActive(true);
    }

    private void HideAllPanels()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (howToPlayPanel != null) howToPlayPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);
    }

    public void ShowMenu()
    {
        // Показати головне меню
        ShowMainMenu();
        gameObject.SetActive(true);
    }

    public void HideMenu()
    {
        // Сховати все меню
        HideAllPanels();
        gameObject.SetActive(false);
    }

    // Button Actions
    public void OnNewGame()
    {
        // Check if save data exists
        if (PlayerPrefs.HasKey("HasSaveData"))
        {
            ShowConfirmation("Почати нову гру? Поточний прогрес буде втрачено!", StartNewGame);
        }
        else
        {
            StartNewGame();
        }
    }

    private void StartNewGame()
    {
        // Clear save data
        PlayerPrefs.DeleteKey("HasSaveData");
        PlayerPrefs.Save();
        
        // Запустити гру через GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartNewGame();
        }
    }

    public void OnContinueGame()
    {
        // Продовжити гру через GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ContinueGame();
        }
    }

    private void LoadGameScene()
    {
        // Load the main game scene with loading screen
        if (LoadingScreen.Instance != null)
        {
            StartCoroutine(LoadingScreen.Instance.LoadSceneAsync("SampleScene"));
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
        }
    }

    public void OnQuitGame()
    {
        ShowConfirmation("Вийти з гри?", QuitGame);
    }

    private void QuitGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.QuitGame();
        }
        else
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }

    // Settings
    private void LoadSettings()
    {
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.8f);
        bool fullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;

        if (musicVolumeSlider != null)
            musicVolumeSlider.value = musicVolume;
        
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.value = sfxVolume;
        
        if (fullscreenToggle != null)
            fullscreenToggle.isOn = fullscreen;

        // Apply settings
        Screen.fullScreen = fullscreen;
    }

    private void OnMusicVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
        
        // Apply to audio system
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(value);
        }
    }

    private void OnSFXVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();
        
        // Apply to SFX audio source
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSfxVolume(value);
        }
    }

    private void OnFullscreenChanged(bool value)
    {
        PlayerPrefs.SetInt("Fullscreen", value ? 1 : 0);
        PlayerPrefs.Save();
        
        Screen.fullScreen = value;
    }

    // Confirmation Dialog
    private void ShowConfirmation(string message, System.Action onConfirm)
    {
        if (confirmationDialog != null)
        {
            confirmationDialog.SetActive(true);
            
            if (confirmationText != null)
                confirmationText.text = message;
            
            currentConfirmAction = onConfirm;
        }
    }

    private void OnConfirmYes()
    {
        if (confirmationDialog != null)
            confirmationDialog.SetActive(false);
        
        currentConfirmAction?.Invoke();
        currentConfirmAction = null;
    }

    private void OnConfirmNo()
    {
        if (confirmationDialog != null)
            confirmationDialog.SetActive(false);
        
        currentConfirmAction = null;
    }
}
