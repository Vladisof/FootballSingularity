using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class GameSaveData
{
    public float money;
    public Dictionary<string, int> reputations;
    public Dictionary<string, int> upgradeLevels;
    public List<string> unlockedDNA;
    public string saveDate;
}

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }

    [Header("Auto Save Settings")]
    public bool enableAutoSave = true;
    public float autoSaveInterval = 180f; // 3 хвилини
    
    private const string SAVE_KEY = "GameSaveData";
    private float timeSinceLastSave;
    private bool isDirty; // Чи є незбережені зміни

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

    private void Update()
    {
        if (enableAutoSave && isDirty)
        {
            timeSinceLastSave += Time.deltaTime;
            
            if (timeSinceLastSave >= autoSaveInterval)
            {
                SaveGame();
            }
        }
    }

    private void OnApplicationQuit()
    {
        // Зберегти гру при виході
        if (isDirty)
        {
            SaveGame();
            Debug.Log("Гру автоматично збережено при виході!");
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        // Зберегти гру при згортанні (мобільні пристрої)
        if (pauseStatus && isDirty)
        {
            SaveGame();
            Debug.Log("Гру автоматично збережено при паузі!");
        }
    }

    public void MarkDirty()
    {
        isDirty = true;
    }

    public void SaveGame()
    {
        GameSaveData saveData = new GameSaveData();

        // Save money
        if (MoneyController.Instance != null)
        {
            saveData.money = MoneyController.Instance.GetMoney();
        }

        // Save reputations
        if (ReputationManager.Instance != null)
        {
            saveData.reputations = new Dictionary<string, int>();
            // This would need to be implemented in ReputationManager
        }

        // Save upgrade levels
        if (LabUpgradeManager.Instance != null)
        {
            saveData.upgradeLevels = new Dictionary<string, int>();
            foreach (UpgradeType upgradeType in System.Enum.GetValues(typeof(UpgradeType)))
            {
                int level = LabUpgradeManager.Instance.GetUpgradeLevel(upgradeType);
                saveData.upgradeLevels[upgradeType.ToString()] = level;
            }
        }

        // Save unlocked DNA
        if (DNALibrary.Instance != null)
        {
            saveData.unlockedDNA = new List<string>();
            List<DNAStrand> unlockedDNA = DNALibrary.Instance.GetUnlockedDNA();
            foreach (DNAStrand dna in unlockedDNA)
            {
                saveData.unlockedDNA.Add(dna.displayName);
            }
        }

        saveData.saveDate = System.DateTime.Now.ToString();

        // Convert to JSON and save
        string jsonData = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString(SAVE_KEY, jsonData);
        PlayerPrefs.SetInt("HasSaveData", 1);
        PlayerPrefs.Save();

        // Reset auto-save timer
        timeSinceLastSave = 0f;
        isDirty = false;

        Debug.Log($"Гру автоматично збережено! ({System.DateTime.Now:HH:mm:ss})");
        
        // Показати сповіщення
        if (NotificationManager.Instance != null)
        {
            NotificationManager.Instance.ShowInfo("💾 Гру збережено");
        }
    }

    public bool LoadGame()
    {
        if (!PlayerPrefs.HasKey(SAVE_KEY))
        {
            Debug.Log("No save data found.");
            return false;
        }

        string jsonData = PlayerPrefs.GetString(SAVE_KEY);
        GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(jsonData);

        // Load money
        if (MoneyController.Instance != null && saveData.money > 0)
        {
            MoneyController.Instance.AddMoney(saveData.money - MoneyController.Instance.GetMoney());
        }

        // Load upgrade levels
        if (LabUpgradeManager.Instance != null && saveData.upgradeLevels != null)
        {
            foreach (var kvp in saveData.upgradeLevels)
            {
                if (System.Enum.TryParse(kvp.Key, out UpgradeType upgradeType))
                {
                    // This would need to be implemented in LabUpgradeManager
                    // LabUpgradeManager.Instance.SetUpgradeLevel(upgradeType, kvp.Value);
                }
            }
        }

        Debug.Log($"Game loaded! Last saved: {saveData.saveDate}");
        return true;
    }

    public void DeleteSaveData()
    {
        PlayerPrefs.DeleteKey(SAVE_KEY);
        PlayerPrefs.DeleteKey("HasSaveData");
        PlayerPrefs.Save();
        isDirty = false;
        timeSinceLastSave = 0f;
        Debug.Log("Save data deleted.");
    }

    public float GetTimeSinceLastSave()
    {
        return timeSinceLastSave;
    }

    public float GetTimeUntilNextSave()
    {
        return Mathf.Max(0, autoSaveInterval - timeSinceLastSave);
    }
}
