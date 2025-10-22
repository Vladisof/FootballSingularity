using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeCard : MonoBehaviour
{
    public TextMeshProUGUI upgradeNameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI costText;
    public Button upgradeButton;

    private UpgradeType upgradeType;

    public void Setup(UpgradeType type)
    {
        upgradeType = type;

        if (upgradeNameText != null)
        {
            upgradeNameText.text = GetUpgradeName(type);
        }

        if (LabUpgradeManager.Instance != null)
        {
            int currentLevel = LabUpgradeManager.Instance.GetUpgradeLevel(type);
            
            if (levelText != null)
            {
                levelText.text = $"Level: {currentLevel}/10";
            }

            if (descriptionText != null)
            {
                descriptionText.text = GetUpgradeDescription(type, currentLevel);
            }

            if (costText != null && currentLevel < 10)
            {
                int cost = LabUpgradeManager.Instance.GetUpgradeCost(type, currentLevel);
                costText.text = $"Cost: ${cost}";
            }
            else if (costText != null)
            {
                costText.text = "MAX LEVEL";
            }

            if (upgradeButton != null)
            {
                upgradeButton.interactable = currentLevel < 10;
                upgradeButton.onClick.AddListener(OnUpgrade);
            }
        }
    }

    private string GetUpgradeName(UpgradeType type)
    {
        switch (type)
        {
            case UpgradeType.MutationChamber: return "Mutation Chamber";
            case UpgradeType.TraitStabilizer: return "Trait Stabilizer";
            case UpgradeType.ResearchSpeed: return "Research Speed";
            case UpgradeType.DNALibraryCapacity: return "DNA Library";
            case UpgradeType.MutationSpeed: return "Mutation Speed";
            default: return type.ToString();
        }
    }

    private string GetUpgradeDescription(UpgradeType type, int level)
    {
        switch (type)
        {
            case UpgradeType.MutationChamber:
                float failReduction = level * 3f;
                return $"Reduces mutation failure chance by {failReduction}%";
            case UpgradeType.TraitStabilizer:
                float retention = 50f + (level * 5f);
                return $"Trait retention: {retention}%";
            case UpgradeType.ResearchSpeed:
                float speedBonus = level * 15f;
                return $"Research {speedBonus}% faster";
            case UpgradeType.DNALibraryCapacity:
                int capacity = 20 + (level * 5);
                return $"Store up to {capacity} DNA samples";
            case UpgradeType.MutationSpeed:
                float mutSpeed = level * 20f;
                return $"Mutations {mutSpeed}% faster";
            default:
                return "Upgrade your lab!";
        }
    }

    private void OnUpgrade()
    {
        if (LabUpgradeManager.Instance != null)
        {
            bool success = LabUpgradeManager.Instance.PurchaseUpgrade(upgradeType);
            if (success)
            {
                // Refresh the upgrades panel using SendMessage
                GameObject uiManagerObj = GameObject.Find("Canvas");
                if (uiManagerObj != null)
                {
                    uiManagerObj.SendMessage("ShowUpgradesPanel", SendMessageOptions.DontRequireReceiver);
                }
            }
        }
    }
}
