using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject labPanel;
    public GameObject ordersPanel;
    public GameObject researchPanel;
    public GameObject upgradesPanel;
    public GameObject mutationPanel;
    public GameObject pauseMenuPanel;

    [Header("HUD Elements")]
    public TextMeshProUGUI moneyText;
    public Button menuButton;
    public Button saveGameButton;
    public TextMeshProUGUI autoSaveIndicator;

    [Header("Navigation Buttons")]
    public Button labButton;
    public Button ordersButton;
    public Button researchButton;
    public Button upgradesButton;
    public Button mutationsButton;

    [Header("Pause Menu")]
    public Button resumeButton;
    public Button saveButton;
    public Button settingsButton;
    public Button mainMenuButton;

    [Header("Lab Panel Elements")]
    public Transform subjectsContainer;
    public GameObject subjectCardPrefab;
    public Button refreshSubjectsButton;
    public TextMeshProUGUI refreshCostText;

    [Header("Orders Panel Elements")]
    public Transform ordersContainer;
    public GameObject orderCardPrefab;
    public Transform acceptedOrdersContainer;
    public GameObject acceptedOrderCardPrefab;
    public TextMeshProUGUI ordersTabText;

    [Header("Research Panel Elements")]
    public Button researchAnimalButton;
    public Button researchLegendaryButton;
    public Button researchEnvironmentButton;
    public Button researchMechanicalButton;
    public TextMeshProUGUI researchStatusText;

    [Header("Upgrades Panel Elements")]
    public Transform upgradesContainer;
    public GameObject upgradeCardPrefab;

    [Header("Mutation Panel Elements")]
    public TextMeshProUGUI mutationStatusText;
    public Transform dnaSelectionContainer;
    public GameObject dnaCardPrefab;
    public Button startMutationButton;
    public Button cancelMutationButton;

    [Header("Active Mutations Panel Elements")]
    public GameObject activeMutationsPanel;
    public Transform activeMutationsContainer;
    public GameObject mutationCardPrefab;
    public TextMeshProUGUI activeMutationsCountText;

    [Header("Player Selection Panel Elements")]
    public GameObject playerSelectionPanel;
    public Transform playerSelectionContainer;
    public GameObject playerSelectionCardPrefab;
    public TextMeshProUGUI playerSelectionTitleText;
    public Button closePlayerSelectionButton;

    private List<DNAStrand> selectedDNA = new List<DNAStrand>();
    private BaseSubject selectedSubject;
    private string currentOrderId;
    private int currentRequirementIndex;

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
        // НЕ показуємо HUD елементи при старті - вони покажуться при вході в гру
        // ShowHUDElements(); // Видалено
        
        // Ініціалізувати кнопки
        InitializeButtons();
        
        // Не показуємо автоматично LabPanel - чекаємо команди від GameManager
        // ShowLabPanel(); // Викликається з GameManager
    }
    
    private void ShowHUDElements()
    {
        // Показати HUD елементи (вони повинні бути видимі завжди під час гри)
        if (moneyText != null) moneyText.gameObject.SetActive(true);
        if (autoSaveIndicator != null) autoSaveIndicator.gameObject.SetActive(true);
        if (menuButton != null) menuButton.gameObject.SetActive(true);
        if (saveGameButton != null) saveGameButton.gameObject.SetActive(true);
        
        // Показати кнопки навігації
        if (labButton != null) labButton.gameObject.SetActive(true);
        if (ordersButton != null) ordersButton.gameObject.SetActive(true);
        if (researchButton != null) researchButton.gameObject.SetActive(true);
        if (upgradesButton != null) upgradesButton.gameObject.SetActive(true);
        if (mutationsButton != null) mutationsButton.gameObject.SetActive(true);
    }

    private void InitializeButtons()
    {
        // HUD buttons
        if (menuButton != null)
            menuButton.onClick.AddListener(ReturnToMainMenu); // Прямо в головне меню
        
        if (saveGameButton != null)
            saveGameButton.onClick.AddListener(SaveGame);
        
        // Navigation buttons
        if (labButton != null)
            labButton.onClick.AddListener(ShowLabPanel);
        
        if (ordersButton != null)
            ordersButton.onClick.AddListener(ShowOrdersPanel);
        
        if (researchButton != null)
            researchButton.onClick.AddListener(ShowResearchPanel);
        
        if (upgradesButton != null)
            upgradesButton.onClick.AddListener(ShowUpgradesPanel);
        
        if (mutationsButton != null)
            mutationsButton.onClick.AddListener(ShowActiveMutationsPanel);
        
        // Pause menu buttons
        if (resumeButton != null)
            resumeButton.onClick.AddListener(HidePauseMenu);
        
        if (saveButton != null)
            saveButton.onClick.AddListener(SaveGame);
        
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);
        
        // Lab panel buttons
        if (refreshSubjectsButton != null)
            refreshSubjectsButton.onClick.AddListener(OnRefreshSubjects);
        
        // Research buttons
        if (researchAnimalButton != null)
            researchAnimalButton.onClick.AddListener(() => OnResearchCategory(0));
        
        if (researchLegendaryButton != null)
            researchLegendaryButton.onClick.AddListener(() => OnResearchCategory(1));
        
        if (researchEnvironmentButton != null)
            researchEnvironmentButton.onClick.AddListener(() => OnResearchCategory(2));
        
        if (researchMechanicalButton != null)
            researchMechanicalButton.onClick.AddListener(() => OnResearchCategory(3));
        
        // Mutation buttons
        if (startMutationButton != null)
            startMutationButton.onClick.AddListener(OnStartMutation);
        
        if (cancelMutationButton != null)
            cancelMutationButton.onClick.AddListener(OnCancelMutation);
    }

    private void Update()
    {
        // Update HUD
        UpdateHUD();
        
        // Update auto-save indicator
        UpdateAutoSaveIndicator();
        
        // Update research progress if on research panel
        if (researchPanel != null && researchPanel.activeSelf)
        {
            RefreshResearchUI();
        }

        // ESC key to toggle pause menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenuPanel != null && pauseMenuPanel.activeSelf)
            {
                HidePauseMenu();
            }
            else
            {
                ShowPauseMenu();
            }
        }
    }

    private void UpdateHUD()
    {
        if (moneyText != null && MoneyController.Instance != null)
        {
            moneyText.text = $"💰 ${MoneyController.Instance.GetMoney():F0}";
        }
    }

    private void UpdateAutoSaveIndicator()
    {
        if (autoSaveIndicator != null && SaveSystem.Instance != null)
        {
            float timeUntilSave = SaveSystem.Instance.GetTimeUntilNextSave();
            
            if (timeUntilSave > 0)
            {
                int minutes = Mathf.FloorToInt(timeUntilSave / 60f);
                int seconds = Mathf.FloorToInt(timeUntilSave % 60f);
                autoSaveIndicator.text = $"💾 Save after: {minutes}:{seconds:00}";
            }
            else
            {
                autoSaveIndicator.text = "💾 Save";
            }
        }
    }

    public void HideAllGamePanels()
    {
        // Сховати всі ігрові панелі (не меню!)
        if (labPanel != null) labPanel.SetActive(false);
        if (ordersPanel != null) ordersPanel.SetActive(false);
        if (researchPanel != null) researchPanel.SetActive(false);
        if (upgradesPanel != null) upgradesPanel.SetActive(false);
        if (mutationPanel != null) mutationPanel.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        
        // Сховати HUD елементи
        if (moneyText != null) moneyText.gameObject.SetActive(false);
        if (autoSaveIndicator != null) autoSaveIndicator.gameObject.SetActive(false);
        if (menuButton != null) menuButton.gameObject.SetActive(false);
        // saveGameButton видалена
        
        // Сховати кнопки навігації
        if (labButton != null) labButton.gameObject.SetActive(false);
        if (ordersButton != null) ordersButton.gameObject.SetActive(false);
        if (researchButton != null) researchButton.gameObject.SetActive(false);
        if (upgradesButton != null) upgradesButton.gameObject.SetActive(false);
        if (mutationsButton != null) mutationsButton.gameObject.SetActive(false);
    }

    // Panel Navigation
    public void ShowLabPanel()
    {
        // Показати HUD елементи при вході в гру
        ShowHUDElements();
        
        // HUD та кнопки вже показані, просто перемикаємо панель
        HideAllPanels();
        if (labPanel != null) labPanel.SetActive(true);
        
        // Перемістити камеру в позицію гри ПІСЛЯ показу панелі
        if (CameraController.Instance != null)
        {
            CameraController.Instance.MoveToGamePosition();
        }
        
        RefreshLabUI();
    }

    public void ShowOrdersPanel()
    {
        HideAllPanels();
        if (ordersPanel != null) ordersPanel.SetActive(true);
        RefreshOrdersUI();
    }

    public void ShowResearchPanel()
    {
        HideAllPanels();
        if (researchPanel != null) researchPanel.SetActive(true);
        RefreshResearchUI();
    }

    public void ShowUpgradesPanel()
    {
        HideAllPanels();
        if (upgradesPanel != null) upgradesPanel.SetActive(true);
        RefreshUpgradesUI();
    }

    public void ShowMutationPanel(BaseSubject subject)
    {
        HideAllPanels();
        if (mutationPanel != null) mutationPanel.SetActive(true);
        selectedSubject = subject;
        selectedDNA.Clear();
        RefreshMutationUI();
    }

    public void ShowActiveMutationsPanel()
    {
        HideAllPanels();
        if (activeMutationsPanel != null) activeMutationsPanel.SetActive(true);
        RefreshActiveMutationsUI();
    }

    public void ShowPlayerSelectionPanel(string orderId, int requirementIndex)
    {
        currentOrderId = orderId;
        currentRequirementIndex = requirementIndex;
        
        HideAllPanels();
        if (playerSelectionPanel != null) playerSelectionPanel.SetActive(true);
        RefreshPlayerSelectionUI();
    }

    private void HideAllPanels()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (labPanel != null) labPanel.SetActive(false);
        if (ordersPanel != null) ordersPanel.SetActive(false);
        if (researchPanel != null) researchPanel.SetActive(false);
        if (upgradesPanel != null) upgradesPanel.SetActive(false);
        if (mutationPanel != null) mutationPanel.SetActive(false);
        if (activeMutationsPanel != null) activeMutationsPanel.SetActive(false);
        if (playerSelectionPanel != null) playerSelectionPanel.SetActive(false);
        // Don't hide pause menu here
    }

    // Lab UI
    private void RefreshLabUI()
    {
        if (SubjectGenerator.Instance == null)
        {
            Debug.LogWarning("SubjectGenerator.Instance is null in RefreshLabUI");
            return;
        }

        if (subjectsContainer == null)
        {
            Debug.LogWarning("subjectsContainer is null in RefreshLabUI");
            return;
        }

        // Clear existing
        foreach (Transform child in subjectsContainer)
        {
            Destroy(child.gameObject);
        }

        // Display subjects
        List<BaseSubject> subjects = SubjectGenerator.Instance.GetAvailableSubjects();
        Debug.Log($"Refreshing Lab UI with {subjects.Count} subjects");
        
        foreach (BaseSubject subject in subjects)
        {
            GameObject card = Instantiate(subjectCardPrefab, subjectsContainer);
            SubjectCard cardScript = card.GetComponent<SubjectCard>();
            if (cardScript != null)
            {
                cardScript.Setup(subject);
                Debug.Log($"Created card for subject: {subject.subjectName}");
            }
            else
            {
                Debug.LogWarning("SubjectCard component not found on instantiated prefab");
            }
        }

        if (refreshCostText != null)
        {
            refreshCostText.text = "Refresh: $50";
        }
    }

    public void OnRefreshSubjects()
    {
        if (SubjectGenerator.Instance != null)
        {
            SubjectGenerator.Instance.RefreshAllSubjects(50);
            RefreshLabUI();
        }
    }

    // Orders UI
    public void RefreshOrdersUI()
    {
        if (OrderManager.Instance == null) return;

        foreach (Transform child in ordersContainer)
        {
            Destroy(child.gameObject);
        }

        List<TeamOrder> orders = OrderManager.Instance.GetActiveOrders();
        foreach (TeamOrder order in orders)
        {
            GameObject card = Instantiate(orderCardPrefab, ordersContainer);
            OrderCard cardScript = card.GetComponent<OrderCard>();
            if (cardScript != null)
            {
                cardScript.Setup(order);
            }
        }

        // Refresh accepted orders
        RefreshAcceptedOrdersUI();
    }

    public void RefreshAcceptedOrdersUI()
    {
        if (OrderManager.Instance == null) return;

        foreach (Transform child in acceptedOrdersContainer)
        {
            Destroy(child.gameObject);
        }

        List<TeamOrder> acceptedOrders = OrderManager.Instance.GetAcceptedOrders();
        foreach (TeamOrder order in acceptedOrders)
        {
            GameObject card = Instantiate(acceptedOrderCardPrefab, acceptedOrdersContainer);
            AcceptedOrderCard cardScript = card.GetComponent<AcceptedOrderCard>();
            if (cardScript != null)
            {
                cardScript.Setup(order);
            }
        }

        // Update orders tab text
        if (ordersTabText != null)
        {
            int activeCount = OrderManager.Instance.GetActiveOrders().Count;
            int acceptedCount = OrderManager.Instance.GetAcceptedOrders().Count;
            ordersTabText.text = $"Active Orders ({activeCount}) / Accepted Orders ({acceptedCount})";
        }
    }

    // Research UI
    private void RefreshResearchUI()
    {
        if (ResearchSystem.Instance == null) return;

        UpdateResearchButton(researchAnimalButton, DNACategory.Animal);
        UpdateResearchButton(researchLegendaryButton, DNACategory.LegendaryPlayer);
        UpdateResearchButton(researchEnvironmentButton, DNACategory.Environment);
        UpdateResearchButton(researchMechanicalButton, DNACategory.Mechanical);
    }

    private void UpdateResearchButton(Button button, DNACategory category)
    {
        if (button == null) return;

        bool isResearching = ResearchSystem.Instance.IsResearching(category);
        button.interactable = !isResearching;

        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            if (isResearching)
            {
                float progress = ResearchSystem.Instance.GetResearchProgress(category);
                buttonText.text = $"{category}\n{(progress * 100):F0}%";
            }
            else
            {
                int cost = ResearchSystem.Instance.GetResearchCost(category);
                buttonText.text = $"Research {category}\n${cost}";
            }
        }
    }

    public void OnResearchCategory(int categoryIndex)
    {
        DNACategory category = (DNACategory)categoryIndex;
        if (ResearchSystem.Instance != null)
        {
            ResearchSystem.Instance.StartResearch(category, OnResearchComplete);
        }
    }

    private void OnResearchComplete(DNAStrand newDNA)
    {
        if (newDNA != null)
        {
            Debug.Log($"Unlocked: {newDNA.displayName}!");
            // Could show a popup here
        }
        RefreshResearchUI();
    }

    // Upgrades UI
    private void RefreshUpgradesUI()
    {
        if (LabUpgradeManager.Instance == null) return;

        foreach (Transform child in upgradesContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (UpgradeType type in System.Enum.GetValues(typeof(UpgradeType)))
        {
            GameObject card = Instantiate(upgradeCardPrefab, upgradesContainer);
            UpgradeCard cardScript = card.GetComponent<UpgradeCard>();
            if (cardScript != null)
            {
                cardScript.Setup(type);
            }
        }
    }

    // Mutation UI
    private void RefreshMutationUI()
    {
        if (DNALibrary.Instance == null) return;

        // Clear existing DNA cards
        foreach (Transform child in dnaSelectionContainer)
        {
            Destroy(child.gameObject);
        }

        // Display available DNA
        List<DNAStrand> availableDNA = DNALibrary.Instance.GetUnlockedDNA();
        foreach (DNAStrand dna in availableDNA)
        {
            GameObject card = Instantiate(dnaCardPrefab, dnaSelectionContainer);
            DNACard cardScript = card.GetComponent<DNACard>();
            if (cardScript != null)
            {
                bool isSelected = selectedDNA.Contains(dna);
                cardScript.Setup(dna, isSelected, OnDNASelected);
            }
        }

        if (mutationStatusText != null)
        {
            mutationStatusText.text = $"Selected DNA: {selectedDNA.Count}/3\n" +
                                     $"Subject: {selectedSubject?.subjectName ?? "None"}";
        }

        if (startMutationButton != null)
        {
            startMutationButton.interactable = selectedDNA.Count >= 2 && selectedDNA.Count <= 3;
        }
    }

    private void OnDNASelected(DNAStrand dna)
    {
        if (selectedDNA.Contains(dna))
        {
            selectedDNA.Remove(dna);
        }
        else if (selectedDNA.Count < 3)
        {
            selectedDNA.Add(dna);
        }
        RefreshMutationUI();
    }

    public void OnStartMutation()
    {
        if (MutationSystem.Instance != null && selectedSubject != null)
        {
            bool started = MutationSystem.Instance.StartMutation(selectedSubject, selectedDNA, OnMutationComplete);
            if (started)
            {
                if (startMutationButton != null) startMutationButton.interactable = false;
                if (mutationStatusText != null) mutationStatusText.text = "Mutation in progress...";
            }
        }
    }

    private void OnMutationComplete(MutationResult result)
    {
        if (result.success)
        {
            Debug.Log("Mutation successful! Player created with stats:");
            Debug.Log($"Overall Rating: {result.mutatedPlayer.GetOverallRating()}");
            
            // Зберегти створеного гравця
            if (CreatedPlayersManager.Instance != null && selectedSubject != null)
            {
                CreatedPlayer newPlayer = new CreatedPlayer(result.mutatedPlayer, selectedDNA, selectedSubject);
                CreatedPlayersManager.Instance.AddPlayer(newPlayer);
                Debug.Log($"Player {newPlayer.playerName} saved to roster!");
            }
            
            // Використати суб'єкта (видалити його зі списку)
            if (SubjectGenerator.Instance != null && selectedSubject != null)
            {
                SubjectGenerator.Instance.UseSubject(selectedSubject.subjectId);
            }
            
            ShowLabPanel();
        }
        else
        {
            Debug.Log($"Mutation failed: {result.failureMessage}");
            ShowLabPanel();
        }
    }

    public void OnCancelMutation()
    {
        ShowLabPanel();
    }

    // Active Mutations Panel
    private void RefreshActiveMutationsUI()
    {
        if (MutationSystem.Instance == null)
        {
            Debug.LogWarning("MutationSystem.Instance is null in RefreshActiveMutationsUI");
            return;
        }

        if (activeMutationsContainer == null)
        {
            Debug.LogWarning("activeMutationsContainer is null in RefreshActiveMutationsUI");
            return;
        }

        // Clear existing cards
        foreach (Transform child in activeMutationsContainer)
        {
            Destroy(child.gameObject);
        }

        // Get active mutations
        List<MutationProcess> mutations = MutationSystem.Instance.GetActiveMutations();
        
        // Update count text
        if (activeMutationsCountText != null)
        {
            int maxMutations = MutationSystem.Instance.maxSimultaneousMutations;
            activeMutationsCountText.text = $"Active: {mutations.Count}/{maxMutations}";
        }

        // Display each mutation
        if (mutations.Count == 0)
        {
            // Show empty message
            if (activeMutationsCountText != null)
            {
                activeMutationsCountText.text = "None active mutations.";
            }
        }
        else
        {
            foreach (MutationProcess mutation in mutations)
            {
                GameObject card = Instantiate(mutationCardPrefab, activeMutationsContainer);
                MutationCard cardScript = card.GetComponent<MutationCard>();
                if (cardScript != null)
                {
                    cardScript.Setup(mutation);
                }
                else
                {
                    Debug.LogWarning("MutationCard component not found on instantiated prefab");
                }
            }
        }
    }

    // Player Selection UI
    private void RefreshPlayerSelectionUI()
    {
        if (currentOrderId == null || currentRequirementIndex < 0) return;

        // Clear existing player selections
        foreach (Transform child in playerSelectionContainer)
        {
            Destroy(child.gameObject);
        }

        // Get the current order and its requirements
        TeamOrder currentOrder = OrderManager.Instance.GetOrderById(currentOrderId);
        if (currentOrder == null) return;

        if (playerSelectionTitleText != null)
        {
            playerSelectionTitleText.text = $"Select Player for {currentOrder.teamName} Order";
        }

        // Display available players
        if (CreatedPlayersManager.Instance != null)
        {
            List<CreatedPlayer> availablePlayers = CreatedPlayersManager.Instance.GetAvailablePlayers();
            foreach (CreatedPlayer player in availablePlayers)
            {
                GameObject card = Instantiate(playerSelectionCardPrefab, playerSelectionContainer);
                PlayerSelectionCard cardScript = card.GetComponent<PlayerSelectionCard>();
                if (cardScript != null)
                {
                    cardScript.Setup(player, OnPlayerSelected);
                }
            }
        }

        if (closePlayerSelectionButton != null)
        {
            closePlayerSelectionButton.onClick.RemoveAllListeners();
            closePlayerSelectionButton.onClick.AddListener(HidePlayerSelectionPanel);
        }
    }

    private void OnPlayerSelected(CreatedPlayer player)
    {
        if (currentOrderId != null && OrderManager.Instance != null)
        {
            // Submit the player to fulfill the requirement
            bool success = OrderManager.Instance.SubmitPlayer(currentOrderId, currentRequirementIndex, player.stats);
            
            if (success)
            {
                Debug.Log($"Player {player.playerName} submitted successfully!");
                
                // Refresh orders UI
                RefreshOrdersUI();
                RefreshAcceptedOrdersUI();
            }

            // Hide player selection and return to orders panel
            HidePlayerSelectionPanel();
            ShowOrdersPanel();
        }
    }

    public void HidePlayerSelectionPanel()
    {
        if (playerSelectionPanel != null)
        {
            playerSelectionPanel.SetActive(false);
            Time.timeScale = 1f; // Resume game
        }
    }

    // Pause Menu
    public void ShowPauseMenu()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
            Time.timeScale = 0f; // Pause game
        }
    }

    public void HidePauseMenu()
    {
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
            Time.timeScale = 1f; // Resume game
        }
    }

    public void SaveGame()
    {
        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.SaveGame();
            Debug.Log("Game saved!");
        }
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f; // Reset time scale
        
        // Перемістити камеру назад в позицію меню
        if (CameraController.Instance != null)
        {
            CameraController.Instance.MoveToMenuPosition();
        }
        
        // Повернутися в меню через GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReturnToMainMenu();
        }
    }
}
