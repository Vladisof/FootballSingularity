using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TeamOrder
{
    public string orderId;
    public string teamName;
    public float currentReputation;
    public List<PlayerRequirement> playerRequirements;
    public string bonusTag;
    public int basePayout;
    public float timeRemaining; // Час в секундах до зникнення
    public float maxTime; // Максимальний час (30 секунд)
    public bool isCompleted;

    public TeamOrder(string team, float reputation)
    {
        orderId = Guid.NewGuid().ToString();
        teamName = team;
        currentReputation = reputation;
        playerRequirements = new List<PlayerRequirement>();
        
        // Визначити кількість гравців за репутацією
        int playerCount = GetPlayerCountForReputation(reputation);
        
        // Generate player requirements
        for (int i = 0; i < playerCount; i++)
        {
            playerRequirements.Add(new PlayerRequirement(reputation));
        }
        
        // Calculate payout based on requirements
        basePayout = CalculateBasePayout(reputation);
        
        maxTime = 30f; // 30 секунд на прийняття замовлення
        timeRemaining = maxTime;
        isCompleted = false;

        // Generate bonus tag
        GenerateBonusTag();
    }

    private int GetPlayerCountForReputation(float rep)
    {
        if (rep < 20f) return 1; // Початківці: 1 гравець
        else if (rep < 40f) return UnityEngine.Random.Range(1, 3); // Новачки: 1-2 гравці
        else if (rep < 60f) return UnityEngine.Random.Range(1, 3); // Середній: 1-2 гравці
        else return UnityEngine.Random.Range(2, 4); // Досвідчені та Експерти: 2-3 гравці
    }

    private int CalculateBasePayout(float rep)
    {
        // Базова винагорода = 100$ + (мінімальна вимога × 2) + (різниця між оптимальним і мінімумом × 3) × випадковий множник (0.8-1.2)
        float baseReward = 100f;
        
        // Додати винагороду за складність вимог
        foreach (var req in playerRequirements)
        {
            foreach (var statReq in req.statRequirements)
            {
                int minValue = statReq.Value.minValue;
                int optimalValue = statReq.Value.maxValue;
                int difference = optimalValue - minValue;
                
                baseReward += (minValue * 2f) + (difference * 3f);
            }
        }
        
        // Випадковий множник для варіативності
        float randomMultiplier = UnityEngine.Random.Range(0.8f, 1.2f);
        baseReward *= randomMultiplier;
        
        return Mathf.RoundToInt(baseReward);
    }

    private void GenerateBonusTag()
    {
        string[] bonusTags = {
            "Prefers Animal DNA traits",
            "Prefers Mechanical DNA traits",
            "Bonus for high Speed",
            "Bonus for balanced stats",
            "Prefers Legendary Player DNA",
            "Bonus for high Jumping",
            "Prefers Environment DNA traits"
        };
        bonusTag = bonusTags[UnityEngine.Random.Range(0, bonusTags.Length)];
    }

    public void UpdateTimer(float deltaTime)
    {
        if (!isCompleted && timeRemaining > 0)
        {
            timeRemaining -= deltaTime;
        }
    }

    public bool IsExpired()
    {
        return timeRemaining <= 0 && !isCompleted;
    }

    public float GetTimeProgress()
    {
        return Mathf.Clamp01(timeRemaining / maxTime);
    }
    
    // Оцінка виконання ордера та розрахунок фінальної винагороди
    public int CalculateFinalReward(out int reputationChange)
    {
        float totalScore = 0f;
        int fulfilledCount = 0;
        
        foreach (var req in playerRequirements)
        {
            if (req.isFulfilled)
            {
                totalScore += req.CalculateMatchScore();
                fulfilledCount++;
            }
        }
        
        if (fulfilledCount == 0)
        {
            reputationChange = -1;
            return Mathf.RoundToInt(basePayout * 0.4f);
        }
        
        float averageScore = totalScore / fulfilledCount;
        
        // Система оцінювання згідно з гайдом
        float rewardMultiplier;
        if (averageScore >= 95f)
        {
            // Ідеально: +30% до винагороди, +8 репутації
            rewardMultiplier = 1.3f;
            reputationChange = 8;
        }
        else if (averageScore >= 85f)
        {
            // Відмінно: +10% до винагороди, +5 репутації
            rewardMultiplier = 1.1f;
            reputationChange = 5;
        }
        else if (averageScore >= 70f)
        {
            // Добре: 100% винагороди, +3 репутації
            rewardMultiplier = 1.0f;
            reputationChange = 3;
        }
        else if (averageScore >= 50f)
        {
            // Прийнятно: 70% винагороди, +1 репутація
            rewardMultiplier = 0.7f;
            reputationChange = 1;
        }
        else
        {
            // Погано: 40% винагороди, -1 репутація
            rewardMultiplier = 0.4f;
            reputationChange = -1;
        }
        
        return Mathf.RoundToInt(basePayout * rewardMultiplier);
    }
}

[Serializable]
public class PlayerRequirement
{
    public string position;
    public Dictionary<string, StatRequirement> StatRequirements;
    public bool isFulfilled;
    public PlayerStats submittedPlayer;
    
    // Властивість для зворотної сумісності
    public Dictionary<string, StatRequirement> statRequirements 
    { 
        get => StatRequirements; 
        set => StatRequirements = value; 
    }

    public PlayerRequirement(float reputation)
    {
        string[] positions = { "Forward", "Midfielder", "Defender", "Goalkeeper" };
        position = positions[UnityEngine.Random.Range(0, positions.Length)];
        StatRequirements = new Dictionary<string, StatRequirement>();
        isFulfilled = false;

        // Generate stat requirements based on position and reputation
        GenerateRequirementsForPosition(position, reputation);
    }

    private void GenerateRequirementsForPosition(string pos, float reputation)
    {
        // Адаптивна складність згідно з ORDER_BALANCE_GUIDE.md
        int minStatValue, optimalRange;
        
        if (reputation < 20f)
        {
            // Репутація 0-20 (Початківці): 30-45 мінімум, +10-20 до оптимального
            minStatValue = UnityEngine.Random.Range(30, 46);
            optimalRange = UnityEngine.Random.Range(10, 21);
        }
        else if (reputation < 40f)
        {
            // Репутація 20-40 (Новачки): 40-55 мінімум, +10-25 до оптимального
            minStatValue = UnityEngine.Random.Range(40, 56);
            optimalRange = UnityEngine.Random.Range(10, 26);
        }
        else if (reputation < 60f)
        {
            // Репутація 40-60 (Середній рівень): 50-65 мінімум, +15-25 до оптимального
            minStatValue = UnityEngine.Random.Range(50, 66);
            optimalRange = UnityEngine.Random.Range(15, 26);
        }
        else if (reputation < 80f)
        {
            // Репутація 60-80 (Досвідчені): 60-75 мінімум, +15-25 до оптимального
            minStatValue = UnityEngine.Random.Range(60, 76);
            optimalRange = UnityEngine.Random.Range(15, 26);
        }
        else
        {
            // Репутація 80-100 (Експерти): 70-85 мінімум, +10-20 до оптимального
            minStatValue = UnityEngine.Random.Range(70, 86);
            optimalRange = UnityEngine.Random.Range(10, 21);
        }
        
        int optimalValue = Mathf.Min(99, minStatValue + optimalRange);

        // Генерувати вимоги за позицією
        switch (pos)
        {
            case "Forward":
                StatRequirements["speed"] = new StatRequirement(minStatValue, optimalValue);
                StatRequirements["attack"] = new StatRequirement(Mathf.Min(99, minStatValue + 5), Mathf.Min(99, optimalValue + 5));
                StatRequirements["accuracy"] = new StatRequirement(Mathf.Max(30, minStatValue - 5), Mathf.Max(30, optimalValue - 5));
                break;
            case "Midfielder":
                StatRequirements["stamina"] = new StatRequirement(minStatValue, optimalValue);
                StatRequirements["agility"] = new StatRequirement(minStatValue, optimalValue);
                StatRequirements["speed"] = new StatRequirement(Mathf.Max(30, minStatValue - 10), Mathf.Max(30, optimalValue - 10));
                break;
            case "Defender":
                StatRequirements["defense"] = new StatRequirement(Mathf.Min(99, minStatValue + 5), Mathf.Min(99, optimalValue + 5));
                StatRequirements["strength"] = new StatRequirement(minStatValue, optimalValue);
                StatRequirements["jumping"] = new StatRequirement(Mathf.Max(30, minStatValue - 5), Mathf.Max(30, optimalValue - 5));
                break;
            case "Goalkeeper":
                StatRequirements["jumping"] = new StatRequirement(Mathf.Min(99, minStatValue + 5), Mathf.Min(99, optimalValue + 5));
                StatRequirements["agility"] = new StatRequirement(minStatValue, optimalValue);
                StatRequirements["defense"] = new StatRequirement(minStatValue, optimalValue);
                break;
        }
    }

    public float CalculateMatchScore()
    {
        if (submittedPlayer == null) return 0f;

        float totalScore = 0f;
        int requirementCount = StatRequirements.Count;

        foreach (var requirement in StatRequirements)
        {
            int playerStatValue = GetStatValue(submittedPlayer, requirement.Key);
            float score = requirement.Value.CalculateScore(playerStatValue);
            totalScore += score;
        }

        return (totalScore / requirementCount) * 100f;
    }

    private int GetStatValue(PlayerStats stats, string statName)
    {
        switch (statName.ToLower())
        {
            case "speed": return stats.speed;
            case "defense": return stats.defense;
            case "attack": return stats.attack;
            case "stamina": return stats.stamina;
            case "jumping": return stats.jumping;
            case "strength": return stats.strength;
            case "agility": return stats.agility;
            case "accuracy": return stats.accuracy;
            default: return 0;
        }
    }
}

[Serializable]
public class StatRequirement
{
    public int minValue;
    public int maxValue;

    public StatRequirement(int min, int max)
    {
        minValue = min;
        maxValue = max;
    }

    public float CalculateScore(int actualValue)
    {
        if (actualValue >= maxValue)
        {
            // Досягнення оптимального або вище - ідеально!
            return 1.0f;
        }
        else if (actualValue >= minValue)
        {
            // Між мінімумом та оптимумом - лінійна шкала
            float progress = (float)(actualValue - minValue) / (maxValue - minValue);
            return 0.5f + (progress * 0.5f); // Від 0.5 до 1.0
        }
        else
        {
            // Нижче мінімуму - штраф
            float diff = minValue - actualValue;
            return Mathf.Max(0, 0.5f - (diff / 40f)); // Максимальний штраф до 0
        }
    }
}
