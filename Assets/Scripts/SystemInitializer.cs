using UnityEngine;

/// <summary>
/// Simple manager initializer to ensure all systems are loaded at game start
/// Attach this to a GameObject in your first scene
/// </summary>
public class SystemInitializer : MonoBehaviour
{
    [Header("Auto-create managers on Start if missing")]
    public bool autoCreateManagers = true;

    private void Awake()
    {
        if (autoCreateManagers)
        {
            InitializeAllSystems();
        }
    }

    private void InitializeAllSystems()
    {
        // Check and create each manager if it doesn't exist
        EnsureManagerExists<GameManager>("GameManager");
        EnsureManagerExists<MoneyController>("MoneyController");
        EnsureManagerExists<DNALibrary>("DNALibrary");
        EnsureManagerExists<MutationSystem>("MutationSystem");
        EnsureManagerExists<OrderManager>("OrderManager");
        EnsureManagerExists<ReputationManager>("ReputationManager");
        EnsureManagerExists<ResearchSystem>("ResearchSystem");
        EnsureManagerExists<LabUpgradeManager>("LabUpgradeManager");
        EnsureManagerExists<SubjectGenerator>("SubjectGenerator");

        Debug.Log("✓ All game systems initialized!");
    }

    private void EnsureManagerExists<T>(string objectName) where T : Component
    {
        T existing = FindObjectOfType<T>();
        if (existing == null)
        {
            GameObject manager = new GameObject(objectName);
            manager.AddComponent<T>();
            Debug.Log($"Auto-created: {objectName}");
        }
    }

    // Quick access methods for debugging
    [ContextMenu("Print Game Status")]
    public void PrintGameStatus()
    {
        Debug.Log("=== GAME STATUS ===");
        
        if (MoneyController.Instance != null)
            Debug.Log($"Money: ${MoneyController.Instance.GetCurrentMoney()}");
        
        if (DNALibrary.Instance != null)
            Debug.Log($"Unlocked DNA: {DNALibrary.Instance.GetUnlockedDNA().Count}");
        
        if (OrderManager.Instance != null)
            Debug.Log($"Active Orders: {OrderManager.Instance.GetActiveOrders().Count}");
        
        if (SubjectGenerator.Instance != null)
            Debug.Log($"Available Subjects: {SubjectGenerator.Instance.GetAvailableSubjects().Count}");
    }
}
