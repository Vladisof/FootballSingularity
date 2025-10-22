using System.Collections.Generic;
using UnityEngine;

public class CreatedPlayersManager : MonoBehaviour
{
    public static CreatedPlayersManager Instance { get; private set; }

    private List<CreatedPlayer> createdPlayers = new List<CreatedPlayer>();

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

    public void AddPlayer(CreatedPlayer player)
    {
        createdPlayers.Add(player);
        Debug.Log($"Player added: {player.playerName} (Overall: {player.stats.GetOverallRating()})");
    }

    public List<CreatedPlayer> GetAllPlayers()
    {
        return new List<CreatedPlayer>(createdPlayers);
    }

    public List<CreatedPlayer> GetAvailablePlayers()
    {
        // Повертає гравців, які не призначені на замовлення
        return createdPlayers.FindAll(p => !p.isAssigned);
    }

    public bool AssignPlayerToOrder(string playerId, string orderId)
    {
        CreatedPlayer player = createdPlayers.Find(p => p.playerId == playerId);
        if (player == null || player.isAssigned)
        {
            return false;
        }

        player.isAssigned = true;
        player.assignedOrderId = orderId;
        return true;
    }

    public void RemovePlayer(string playerId)
    {
        CreatedPlayer player = createdPlayers.Find(p => p.playerId == playerId);
        if (player != null)
        {
            createdPlayers.Remove(player);
        }
    }

    public void ResetPlayers()
    {
        createdPlayers.Clear();
        Debug.Log("All created players cleared!");
    }
}

[System.Serializable]
public class CreatedPlayer
{
    public string playerId;
    public string playerName;
    public PlayerStats stats;
    public List<string> dnaUsed; // Назви використаних ДНК
    public bool isAssigned;
    public string assignedOrderId;

    public CreatedPlayer(PlayerStats playerStats, List<DNAStrand> dna, BaseSubject subject)
    {
        playerId = System.Guid.NewGuid().ToString();
        playerName = GeneratePlayerName(subject);
        stats = playerStats;
        dnaUsed = new List<string>();
        
        foreach (var strand in dna)
        {
            dnaUsed.Add(strand.displayName);
        }
        
        isAssigned = false;
        assignedOrderId = "";
    }

    private string GeneratePlayerName(BaseSubject subject)
    {
        // Генерувати ім'я на основі суб'єкта
        string[] prefixes = { "Alpha", "Beta", "Gamma", "Delta", "Omega", "Sigma", "Prime" };
        string prefix = prefixes[Random.Range(0, prefixes.Length)];
        return $"{prefix}-{subject.subjectName}";
    }
}

