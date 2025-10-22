using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerSelectionCard : MonoBehaviour
{
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI statsText;
    public TextMeshProUGUI dnaText;
    public TextMeshProUGUI overallRatingText;
    public Button selectButton;

    private CreatedPlayer player;
    private System.Action<CreatedPlayer> onPlayerSelected;

    public void Setup(CreatedPlayer createdPlayer, System.Action<CreatedPlayer> callback)
    {
        player = createdPlayer;
        onPlayerSelected = callback;

        if (playerNameText != null)
        {
            playerNameText.text = player.playerName;
        }

        if (statsText != null)
        {
            statsText.text = $"SPD: {player.stats.speed} DEF: {player.stats.defense}\n" +
                           $"ATK: {player.stats.attack} STA: {player.stats.stamina}\n" +
                           $"JMP: {player.stats.jumping} STR: {player.stats.strength}\n" +
                           $"AGI: {player.stats.agility} ACC: {player.stats.accuracy}";
        }

        if (dnaText != null)
        {
            dnaText.text = "DNA: " + string.Join(", ", player.dnaUsed);
        }

        if (overallRatingText != null)
        {
            int overall = player.stats.GetOverallRating();
            overallRatingText.text = $"Overall: {overall}";
            
            // Колір залежно від рейтингу
            if (overall >= 80)
                overallRatingText.color = Color.green;
            else if (overall >= 60)
                overallRatingText.color = Color.yellow;
            else
                overallRatingText.color = Color.red;
        }

        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(OnSelectPlayer);
            selectButton.interactable = !player.isAssigned;
            
            if (player.isAssigned)
            {
                TextMeshProUGUI btnText = selectButton.GetComponentInChildren<TextMeshProUGUI>();
                if (btnText != null)
                    btnText.text = "Assigned";
            }
        }
    }

    private void OnSelectPlayer()
    {
        onPlayerSelected?.Invoke(player);
    }
}

