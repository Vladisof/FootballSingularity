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
        basePayout = CalculateBasePayout(reputation);
        maxTime = 30f; // 30 секунд на прийняття замовлення
        timeRemaining = maxTime;
        isCompleted = false;

        // Generate 3 player requirements
        for (int i = 0; i < 3; i++)
        {
            playerRequirements.Add(new PlayerRequirement(reputation));
        }

        // Generate bonus tag
        GenerateBonusTag();
    }

    private int CalculateBasePayout(float rep)
    {
        return Mathf.RoundToInt(100 + (rep * 5)); // Higher rep = better pay
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
}

[Serializable]
public class PlayerRequirement
{
    public string position;
    public Dictionary<string, StatRequirement> statRequirements;
    public bool isFulfilled;
    public PlayerStats submittedPlayer;

    public PlayerRequirement(float reputation)
    {
        string[] positions = { "Forward", "Midfielder", "Defender", "Goalkeeper" };
        position = positions[UnityEngine.Random.Range(0, positions.Length)];
        statRequirements = new Dictionary<string, StatRequirement>();
        isFulfilled = false;

        // Generate 2-3 stat requirements based on position
        GenerateRequirementsForPosition(position, reputation);
    }

    private void GenerateRequirementsForPosition(string pos, float reputation)
    {
        int minStatValue = Mathf.RoundToInt(40 + (reputation / 5)); // Higher rep = harder requirements
        int maxStatValue = minStatValue + 20;

        switch (pos)
        {
            case "Forward":
                statRequirements["speed"] = new StatRequirement(minStatValue, 99);
                statRequirements["attack"] = new StatRequirement(minStatValue + 5, 99);
                statRequirements["accuracy"] = new StatRequirement(minStatValue - 5, 99);
                break;
            case "Midfielder":
                statRequirements["stamina"] = new StatRequirement(minStatValue, 99);
                statRequirements["agility"] = new StatRequirement(minStatValue, 99);
                statRequirements["speed"] = new StatRequirement(minStatValue - 10, 99);
                break;
            case "Defender":
                statRequirements["defense"] = new StatRequirement(minStatValue + 5, 99);
                statRequirements["strength"] = new StatRequirement(minStatValue, 99);
                statRequirements["jumping"] = new StatRequirement(minStatValue - 5, 99);
                break;
            case "Goalkeeper":
                statRequirements["jumping"] = new StatRequirement(minStatValue + 5, 99);
                statRequirements["agility"] = new StatRequirement(minStatValue, 99);
                statRequirements["defense"] = new StatRequirement(minStatValue, 99);
                break;
        }
    }

    public float CalculateMatchScore()
    {
        if (submittedPlayer == null) return 0f;

        float totalScore = 0f;
        int requirementCount = statRequirements.Count;

        foreach (var requirement in statRequirements)
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
        if (actualValue >= minValue && actualValue <= maxValue)
        {
            // Perfect match
            return 1.0f;
        }
        else if (actualValue < minValue)
        {
            // Below minimum - penalize heavily
            float diff = minValue - actualValue;
            return Mathf.Max(0, 1.0f - (diff / 20f));
        }
        else
        {
            // Above maximum is actually good!
            return 1.0f;
        }
    }
}
