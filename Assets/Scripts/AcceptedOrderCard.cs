using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using System.Linq;

public class AcceptedOrderCard : MonoBehaviour
{
    public TextMeshProUGUI teamNameText;
    public TextMeshProUGUI requirementsText;
    public TextMeshProUGUI payoutText;
    public TextMeshProUGUI progressText;
    public Button[] submitPlayerButtons; // Кнопки для подання гравців
    public GameObject[] fulfilledIndicators; // Індикатори виконання

    private TeamOrder _order;

    public void Setup(TeamOrder ord)
    {
        _order = ord;

        if (teamNameText != null)
        {
            teamNameText.text = _order.teamName;
        }

        if (requirementsText != null)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Requirements:");
            for (int i = 0; i < _order.playerRequirements.Count; i++)
            {
                var req = _order.playerRequirements[i];
                sb.AppendLine($"{i + 1}. {req.position}:");
                
                // Display all stat requirements
                foreach (var statReq in req.statRequirements)
                {
                    string statName = char.ToUpper(statReq.Key[0]) + statReq.Key.Substring(1);
                    sb.AppendLine($"   {statName}: {statReq.Value.minValue}+ (optimal: {statReq.Value.maxValue})");
                }
                
                if (req.isFulfilled && req.submittedPlayer != null)
                {
                    float score = req.CalculateMatchScore();
                    sb.AppendLine($"   ✅ Player submitted - {score:F0}% match");
                }
                else
                {
                    sb.AppendLine("   ⏳ Pending...");
                }
            }
            requirementsText.text = sb.ToString();
        }

        if (payoutText != null)
        {
            payoutText.text = $"Reward: ${_order.basePayout}";
        }

        // Налаштувати кнопки подання гравців
        if (submitPlayerButtons != null)
        {
            for (int i = 0; i < submitPlayerButtons.Length && i < _order.playerRequirements.Count; i++)
            {
                int index = i; // Замикання для правильної роботи
                Button btn = submitPlayerButtons[i];
                
                if (btn != null)
                {
                    btn.gameObject.SetActive(true);
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(() => OnSubmitPlayer(index));
                    
                    // Вимкнути кнопку якщо вже виконано
                    btn.interactable = !_order.playerRequirements[index].isFulfilled;
                    
                    // Оновити текст кнопки
                    TextMeshProUGUI btnText = btn.GetComponentInChildren<TextMeshProUGUI>();
                    if (btnText != null)
                    {
                        if (_order.playerRequirements[index].isFulfilled)
                        {
                            btnText.text = "✅ Submitted";
                        }
                        else
                        {
                            btnText.text = $"Submit Player {index + 1}";
                        }
                    }
                }
            }
            
            // Сховати зайві кнопки
            for (int i = _order.playerRequirements.Count; i < submitPlayerButtons.Length; i++)
            {
                if (submitPlayerButtons[i] != null)
                {
                    submitPlayerButtons[i].gameObject.SetActive(false);
                }
            }
        }

        // Оновити індикатори
        UpdateFulfilledIndicators();
        UpdateProgress();
    }

    private void UpdateFulfilledIndicators()
    {
        if (fulfilledIndicators == null || _order == null) return;

        for (int i = 0; i < fulfilledIndicators.Length && i < _order.playerRequirements.Count; i++)
        {
            if (fulfilledIndicators[i] != null)
            {
                fulfilledIndicators[i].SetActive(_order.playerRequirements[i].isFulfilled);
            }
        }
    }

    private void UpdateProgress()
    {
        if (progressText == null || _order == null) return;

        int fulfilled = 0;
        foreach (var req in _order.playerRequirements)
        {
            if (req.isFulfilled) fulfilled++;
        }

        progressText.text = $"Progress: {fulfilled}/{_order.playerRequirements.Count}";
    }

    private void OnSubmitPlayer(int requirementIndex)
    {
        Debug.Log($"Opening player selection for requirement {requirementIndex + 1}");
        
        // Відкрити панель вибору гравця через UIManager
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowPlayerSelectionPanel(_order.orderId, requirementIndex);
        }
    }
}
