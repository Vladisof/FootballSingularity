using System.Collections.Generic;
using UnityEngine;

public class LabUpgradeManager : MonoBehaviour
{
    public static LabUpgradeManager Instance { get; private set; }

    private Dictionary<UpgradeType, int> upgradeLevels = new Dictionary<UpgradeType, int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadUpgrades();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadUpgrades()
    {
        foreach (UpgradeType type in System.Enum.GetValues(typeof(UpgradeType)))
        {
            string key = "Upgrade_" + type.ToString();
            int level = PlayerPrefs.GetInt(key, 0);
            upgradeLevels[type] = level;
        }
    }

    public bool PurchaseUpgrade(UpgradeType type)
    {
        int currentLevel = GetUpgradeLevel(type);
        if (currentLevel >= 10) // Max level 10
        {
            Debug.LogWarning($"{type} is already at max level!");
            return false;
        }

        int cost = GetUpgradeCost(type, currentLevel);
        if (MoneyController.Instance != null && MoneyController.Instance.SubtractMoney(cost))
        {
            upgradeLevels[type] = currentLevel + 1;
            SaveUpgrade(type);
            Debug.Log($"Upgraded {type} to level {upgradeLevels[type]} for ${cost}");

            // Позначити, що гра потребує збереження
            if (SaveSystem.Instance != null)
            {
                SaveSystem.Instance.MarkDirty();
            }

            return true;
        }

        Debug.LogWarning("Not enough money for upgrade!");
        return false;
    }

    public int GetUpgradeCost(UpgradeType type, int currentLevel)
    {
        // Cost increases with each level
        int baseCost = GetBaseUpgradeCost(type);
        return Mathf.RoundToInt(baseCost * Mathf.Pow(1.5f, currentLevel));
    }

    private int GetBaseUpgradeCost(UpgradeType type)
    {
        switch (type)
        {
            case UpgradeType.MutationChamber: return 200;
            case UpgradeType.TraitStabilizer: return 250;
            case UpgradeType.ResearchSpeed: return 300;
            case UpgradeType.DNALibraryCapacity: return 150;
            case UpgradeType.MutationSpeed: return 200;
            default: return 100;
        }
    }

    public int GetUpgradeLevel(UpgradeType type)
    {
        if (!upgradeLevels.ContainsKey(type))
        {
            upgradeLevels[type] = 0;
        }
        return upgradeLevels[type];
    }

    private void SaveUpgrade(UpgradeType type)
    {
        string key = "Upgrade_" + type.ToString();
        PlayerPrefs.SetInt(key, upgradeLevels[type]);
        PlayerPrefs.Save();
    }

    // Getter methods for upgrade effects
    public float GetFailureReduction()
    {
        int level = GetUpgradeLevel(UpgradeType.MutationChamber);
        return level * 0.03f; // 3% reduction per level
    }

    public float GetTraitRetention()
    {
        int level = GetUpgradeLevel(UpgradeType.TraitStabilizer);
        return 0.5f + (level * 0.05f); // 50% base + 5% per level
    }

    public float GetResearchSpeedMultiplier()
    {
        int level = GetUpgradeLevel(UpgradeType.ResearchSpeed);
        return 1.0f + (level * 0.15f); // 15% faster per level
    }

    public int GetDNALibraryCapacity()
    {
        int level = GetUpgradeLevel(UpgradeType.DNALibraryCapacity);
        return 20 + (level * 5); // Base 20 + 5 per level
    }

    public float GetMutationSpeedMultiplier()
    {
        int level = GetUpgradeLevel(UpgradeType.MutationSpeed);
        return 1.0f + (level * 0.2f); // 20% faster per level
    }

    public void ResetAllUpgrades()
    {
        foreach (UpgradeType type in System.Enum.GetValues(typeof(UpgradeType)))
        {
            upgradeLevels[type] = 0;
            SaveUpgrade(type);
        }
    }
}

public enum UpgradeType
{
    MutationChamber,      // Reduces failure chance
    TraitStabilizer,      // Improves trait retention
    ResearchSpeed,        // Shortens research times
    DNALibraryCapacity,   // Increases DNA storage
    MutationSpeed         // Speeds up mutation process
}
