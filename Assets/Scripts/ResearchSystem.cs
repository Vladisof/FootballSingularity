using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResearchSystem : MonoBehaviour
{
    public static ResearchSystem Instance { get; private set; }

    [Header("Research Settings")]
    public float baseResearchTime = 180f; // 3 minutes in seconds
    public int baseResearchCost = 100;

    private Dictionary<DNACategory, ResearchProgress> activeResearch = new Dictionary<DNACategory, ResearchProgress>();

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

    public bool StartResearch(DNACategory category, Action<DNAStrand> onComplete)
    {
        if (activeResearch.ContainsKey(category))
        {
            Debug.LogWarning($"Already researching {category}!");
            return false;
        }

        int cost = GetResearchCost(category);
        if (MoneyController.Instance != null && !MoneyController.Instance.SubtractMoney(cost))
        {
            Debug.LogWarning("Not enough money for research!");
            return false;
        }

        ResearchProgress progress = new ResearchProgress
        {
            category = category,
            onComplete = onComplete,
            startTime = DateTime.Now
        };

        activeResearch[category] = progress;
        StartCoroutine(ProcessResearch(category));

        Debug.Log($"Started researching {category} DNA for ${cost}");
        return true;
    }

    private IEnumerator ProcessResearch(DNACategory category)
    {
        if (!activeResearch.ContainsKey(category))
            yield break;

        float researchSpeed = LabUpgradeManager.Instance != null ? 
            LabUpgradeManager.Instance.GetResearchSpeedMultiplier() : 1f;

        float researchTime = baseResearchTime / researchSpeed;
        float elapsed = 0f;

        while (elapsed < researchTime)
        {
            elapsed += Time.deltaTime;
            // Could update UI progress here
            yield return null;
        }

        // Research complete!
        DNAStrand newDNA = DNALibrary.Instance != null ? 
            DNALibrary.Instance.GetRandomUnresearchedDNA(category) : null;

        if (newDNA != null)
        {
            DNALibrary.Instance.UnlockDNA(newDNA.id);
            Debug.Log($"Research complete! Unlocked: {newDNA.displayName}");
            
            ResearchProgress progress = activeResearch[category];
            progress.onComplete?.Invoke(newDNA);
            
            // Позначити, що гра потребує збереження після завершення дослідження
            if (SaveSystem.Instance != null)
            {
                SaveSystem.Instance.MarkDirty();
            }
        }
        else
        {
            Debug.Log($"All {category} DNA already unlocked!");
        }

        activeResearch.Remove(category);
    }

    public bool IsResearching(DNACategory category)
    {
        return activeResearch.ContainsKey(category);
    }

    public float GetResearchProgress(DNACategory category)
    {
        if (!activeResearch.ContainsKey(category))
            return 0f;

        ResearchProgress progress = activeResearch[category];
        float researchSpeed = LabUpgradeManager.Instance != null ? 
            LabUpgradeManager.Instance.GetResearchSpeedMultiplier() : 1f;
        float researchTime = baseResearchTime / researchSpeed;

        float elapsed = (float)(DateTime.Now - progress.startTime).TotalSeconds;
        return Mathf.Clamp01(elapsed / researchTime);
    }

    public int GetResearchCost(DNACategory category)
    {
        // Cost increases based on how much DNA in that category is already unlocked
        return baseResearchCost;
    }

    private class ResearchProgress
    {
        public DNACategory category;
        public Action<DNAStrand> onComplete;
        public DateTime startTime;
    }
}
