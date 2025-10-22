using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance { get; private set; }

    [Header("Order Spawn Settings")]
    public float minSpawnInterval = 10f; // Мінімум 10 секунд між замовленнями
    public float maxSpawnInterval = 30f; // Максимум 30 секунд між замовленнями
    public int maxActiveOrders = 5; // Максимум активних замовлень одночасно

    private List<TeamOrder> activeOrders = new List<TeamOrder>();
    private List<TeamOrder> completedOrders = new List<TeamOrder>();
    private float timeUntilNextOrder;

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

    private void Start()
    {
        // Встановити час до першого замовлення
        timeUntilNextOrder = Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    private void Update()
    {
        // Оновити таймери всіх активних замовлень
        for (int i = activeOrders.Count - 1; i >= 0; i--)
        {
            TeamOrder order = activeOrders[i];
            order.UpdateTimer(Time.deltaTime);
            
            // Видалити прострочені замовлення
            if (order.IsExpired())
            {
                Debug.Log($"Order from {order.teamName} expired!");
                activeOrders.RemoveAt(i);
                
                // Оповістити UIManager про оновлення
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.RefreshOrdersUI();
                }
            }
        }

        // Таймер для генерації нових замовлень
        if (activeOrders.Count < maxActiveOrders)
        {
            timeUntilNextOrder -= Time.deltaTime;
            
            if (timeUntilNextOrder <= 0)
            {
                GenerateNewOrder();
                timeUntilNextOrder = Random.Range(minSpawnInterval, maxSpawnInterval);
            }
        }
    }

    public void GenerateNewOrder()
    {
        if (ReputationManager.Instance == null) return;
        if (activeOrders.Count >= maxActiveOrders) return;

        string team = ReputationManager.Instance.GetRandomTeam();
        float reputation = ReputationManager.Instance.GetReputation(team);

        TeamOrder order = new TeamOrder(team, reputation);
        activeOrders.Add(order);

        Debug.Log($"New order from {team}! Time remaining: 30s");
        
        // Оповістити UIManager про нове замовлення
        if (UIManager.Instance != null)
        {
            UIManager.Instance.RefreshOrdersUI();
        }
    }

    public List<TeamOrder> GetActiveOrders()
    {
        return new List<TeamOrder>(activeOrders);
    }

    public bool AcceptOrder(string orderId)
    {
        TeamOrder order = activeOrders.Find(o => o.orderId == orderId);
        if (order == null || order.IsExpired())
        {
            return false;
        }

        // Тут можна додати логіку прийняття замовлення
        // Наприклад, перемістити замовлення в "прийняті" список
        Debug.Log($"Order from {order.teamName} accepted!");
        return true;
    }

    public bool SubmitPlayer(string orderId, int requirementIndex, PlayerStats player)
    {
        TeamOrder order = activeOrders.Find(o => o.orderId == orderId);
        if (order == null || requirementIndex >= order.playerRequirements.Count)
        {
            return false;
        }

        PlayerRequirement requirement = order.playerRequirements[requirementIndex];
        requirement.submittedPlayer = player;
        requirement.isFulfilled = true;

        // Check if all requirements fulfilled
        bool allFulfilled = true;
        foreach (var req in order.playerRequirements)
        {
            if (!req.isFulfilled)
            {
                allFulfilled = false;
                break;
            }
        }

        if (allFulfilled)
        {
            CompleteOrder(order);
        }

        return true;
    }

    private void CompleteOrder(TeamOrder order)
    {
        // Calculate overall match score
        float totalScore = 0f;
        foreach (var requirement in order.playerRequirements)
        {
            totalScore += requirement.CalculateMatchScore();
        }
        float averageScore = totalScore / order.playerRequirements.Count;

        // Determine success level
        int payout = 0;
        float reputationChange = 0f;

        if (averageScore >= 90f)
        {
            // Success!
            payout = order.basePayout;
            reputationChange = 5f;
            Debug.Log($"Order completed successfully! +${payout}, +{reputationChange} rep");
        }
        else if (averageScore >= 70f)
        {
            // Acceptable
            payout = Mathf.RoundToInt(order.basePayout * 0.7f);
            reputationChange = 2f;
            Debug.Log($"Order completed acceptably. +${payout}, +{reputationChange} rep");
        }
        else
        {
            // Poor performance
            payout = Mathf.RoundToInt(order.basePayout * 0.3f);
            reputationChange = -3f;
            Debug.Log($"Order completed poorly. +${payout}, {reputationChange} rep");
        }

        // Apply rewards/penalties
        if (MoneyController.Instance != null)
        {
            MoneyController.Instance.AddMoney(payout);
        }

        if (ReputationManager.Instance != null)
        {
            ReputationManager.Instance.ChangeReputation(order.teamName, reputationChange);
        }

        // Move to completed orders
        order.isCompleted = true;
        completedOrders.Add(order);
        activeOrders.Remove(order);
        
        // Оповістити UIManager
        if (UIManager.Instance != null)
        {
            UIManager.Instance.RefreshOrdersUI();
        }
    }

    public float GetTimeUntilNextOrder()
    {
        return timeUntilNextOrder;
    }

    public void ResetOrders()
    {
        activeOrders.Clear();
        completedOrders.Clear();
        timeUntilNextOrder = Random.Range(minSpawnInterval, maxSpawnInterval);
    }
}
