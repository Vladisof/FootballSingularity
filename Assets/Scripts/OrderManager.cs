using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance { get; private set; }

    [Header("Order Spawn Settings")]
    public float minSpawnInterval = 10f;
    public float maxSpawnInterval = 30f;
    public int maxActiveOrders = 5;

    private List<TeamOrder> activeOrders = new List<TeamOrder>();
    private List<TeamOrder> acceptedOrders = new List<TeamOrder>();
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
        timeUntilNextOrder = Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    private void Update()
    {
        // Оновити таймери всіх активних замовлень
        for (int i = activeOrders.Count - 1; i >= 0; i--)
        {
            TeamOrder order = activeOrders[i];
            order.UpdateTimer(Time.deltaTime);
            
            if (order.IsExpired())
            {
                Debug.Log($"Order from {order.teamName} expired!");
                activeOrders.RemoveAt(i);
                
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
        float baseReputation = ReputationManager.Instance.GetReputation(team);
        
        // Додати варіативність ±5 до репутації ордера
        float reputationVariance = Random.Range(-5f, 5f);
        float orderReputation = Mathf.Clamp(baseReputation + reputationVariance, 0f, 100f);

        TeamOrder order = new TeamOrder(team, orderReputation);
        activeOrders.Add(order);

        Debug.Log($"New order from {team}! Base Reputation: {baseReputation:F0}, Order Reputation: {orderReputation:F0}, Requirements: {order.playerRequirements.Count}, Payout: ${order.basePayout}");
        
        if (UIManager.Instance != null)
        {
            UIManager.Instance.RefreshOrdersUI();
        }
    }

    public List<TeamOrder> GetActiveOrders()
    {
        return new List<TeamOrder>(activeOrders);
    }

    public List<TeamOrder> GetAcceptedOrders()
    {
        return new List<TeamOrder>(acceptedOrders);
    }

    public TeamOrder GetOrderById(string orderId)
    {
        TeamOrder order = activeOrders.Find(o => o.orderId == orderId);
        if (order == null)
        {
            order = acceptedOrders.Find(o => o.orderId == orderId);
        }
        return order;
    }

    public bool AcceptOrder(string orderId)
    {
        TeamOrder order = activeOrders.Find(o => o.orderId == orderId);
        if (order == null || order.IsExpired())
        {
            return false;
        }

        acceptedOrders.Add(order);
        activeOrders.Remove(order);

        Debug.Log($"Order from {order.teamName} accepted!");
        return true;
    }

    public bool SubmitPlayer(string orderId, int requirementIndex, PlayerStats player)
    {
        TeamOrder order = acceptedOrders.Find(o => o.orderId == orderId);
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
        // Система оцінювання згідно з ORDER_BALANCE_GUIDE.md
        float totalScore = 0f;
        foreach (var requirement in order.playerRequirements)
        {
            totalScore += requirement.CalculateMatchScore();
        }
        float averageScore = totalScore / order.playerRequirements.Count;

        int payout;
        float reputationChange;

        if (averageScore >= 95f)
        {
            // 95%+ (Ідеально): +30% до винагороди, +8 репутації
            payout = Mathf.RoundToInt(order.basePayout * 1.3f);
            reputationChange = 8f;
            Debug.Log($"Order completed PERFECTLY! Score: {averageScore:F1}% | +${payout} | +{reputationChange} rep");
        }
        else if (averageScore >= 85f)
        {
            // 85-95% (Відмінно): +10% до винагороди, +5 репутації
            payout = Mathf.RoundToInt(order.basePayout * 1.1f);
            reputationChange = 5f;
            Debug.Log($"Order completed EXCELLENTLY! Score: {averageScore:F1}% | +${payout} | +{reputationChange} rep");
        }
        else if (averageScore >= 70f)
        {
            // 70-85% (Добре): 100% винагороди, +3 репутації
            payout = order.basePayout;
            reputationChange = 3f;
            Debug.Log($"Order completed WELL! Score: {averageScore:F1}% | +${payout} | +{reputationChange} rep");
        }
        else if (averageScore >= 50f)
        {
            // 50-70% (Прийнятно): 70% винагороди, +1 репутація
            payout = Mathf.RoundToInt(order.basePayout * 0.7f);
            reputationChange = 1f;
            Debug.Log($"Order completed ACCEPTABLY. Score: {averageScore:F1}% | +${payout} | +{reputationChange} rep");
        }
        else
        {
            // <50% (Погано): 40% винагороди, -1 репутація
            payout = Mathf.RoundToInt(order.basePayout * 0.4f);
            reputationChange = -1f;
            Debug.Log($"Order completed POORLY. Score: {averageScore:F1}% | +${payout} | {reputationChange} rep");
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
        acceptedOrders.Remove(order);
        
        if (UIManager.Instance != null)
        {
            UIManager.Instance.RefreshOrdersUI();
        }
        
        if (NotificationManager.Instance != null)
        {
            NotificationManager.Instance.ShowSuccess($"Order completed! +${payout}");
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
        acceptedOrders.Clear();
        timeUntilNextOrder = Random.Range(minSpawnInterval, maxSpawnInterval);
    }
}
