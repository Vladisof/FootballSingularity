using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class OrderCard : MonoBehaviour
{
    public TextMeshProUGUI teamNameText;
    public TextMeshProUGUI reputationText;
    public TextMeshProUGUI requirementsText;
    public TextMeshProUGUI bonusTagText;
    public TextMeshProUGUI payoutText;
    public TextMeshProUGUI timerText;
    public Slider timerSlider;
    public Button acceptButton;

    private TeamOrder order;

    public void Setup(TeamOrder ord)
    {
        order = ord;

        if (teamNameText != null)
        {
            teamNameText.text = order.teamName;
        }

        if (reputationText != null)
        {
            reputationText.text = $"Rep: {order.currentReputation:F0}/100";
        }

        if (requirementsText != null)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Requirements:");
            foreach (var req in order.playerRequirements)
            {
                sb.AppendLine($"- {req.position}");
                foreach (var stat in req.statRequirements)
                {
                    sb.AppendLine($"  {stat.Key}: {stat.Value.minValue}+");
                }
            }
            requirementsText.text = sb.ToString();
        }

        if (bonusTagText != null)
        {
            bonusTagText.text = order.bonusTag;
        }

        if (payoutText != null)
        {
            payoutText.text = $"💰 ${order.basePayout}";
        }

        if (acceptButton != null)
        {
            acceptButton.onClick.RemoveAllListeners();
            acceptButton.onClick.AddListener(OnAcceptOrder);
        }
    }

    private void Update()
    {
        if (order == null) return;

        // Оновлювати таймер
        UpdateTimer();
    }

    private void UpdateTimer()
    {
        if (timerText != null)
        {
            int seconds = Mathf.CeilToInt(order.timeRemaining);
            timerText.text = $"⏱ {seconds}s";
            
            // Змінити колір тексту залежно від часу
            if (order.timeRemaining < 10f)
            {
                timerText.color = Color.red;
            }
            else if (order.timeRemaining < 20f)
            {
                timerText.color = Color.yellow;
            }
            else
            {
                timerText.color = Color.green;
            }
        }

        if (timerSlider != null)
        {
            timerSlider.value = order.GetTimeProgress();
        }
    }


    private void OnAcceptOrder()
    {
        if (OrderManager.Instance != null)
        {
            bool accepted = OrderManager.Instance.AcceptOrder(order.orderId);
            if (accepted)
            {
                Debug.Log($"Accepted order from {order.teamName}");
                // Тут можна додати логіку переходу на екран виконання замовлення
            }
        }
    }
}
