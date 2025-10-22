using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DNACard : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI categoryText;
    public TextMeshProUGUI rarityText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI statsText;
    public Button selectButton;
    public Image backgroundImage;

    private DNAStrand dna;
    private Action<DNAStrand> onSelected;
    private bool isSelected;

    public void Setup(DNAStrand dnaStrand, bool selected, Action<DNAStrand> onSelectCallback)
    {
        dna = dnaStrand;
        isSelected = selected;
        onSelected = onSelectCallback;

        if (nameText != null)
        {
            nameText.text = dna.displayName;
        }

        if (categoryText != null)
        {
            categoryText.text = dna.category.ToString();
        }

        if (rarityText != null)
        {
            rarityText.text = dna.rarity.ToString();
            rarityText.color = GetRarityColor(dna.rarity);
        }

        if (descriptionText != null)
        {
            descriptionText.text = dna.description;
        }

        if (statsText != null)
        {
            statsText.text = FormatStatModifiers(dna.statModifiers);
        }

        if (selectButton != null)
        {
            selectButton.onClick.AddListener(OnSelect);
        }

        if (backgroundImage != null)
        {
            backgroundImage.color = isSelected ? Color.green : Color.gray;
        }
    }

    private string FormatStatModifiers(StatModifiers mods)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        if (mods.speedBonus != 0) sb.AppendLine($"Speed: {FormatBonus(mods.speedBonus)}");
        if (mods.defenseBonus != 0) sb.AppendLine($"Defense: {FormatBonus(mods.defenseBonus)}");
        if (mods.attackBonus != 0) sb.AppendLine($"Attack: {FormatBonus(mods.attackBonus)}");
        if (mods.staminaBonus != 0) sb.AppendLine($"Stamina: {FormatBonus(mods.staminaBonus)}");
        if (mods.jumpingBonus != 0) sb.AppendLine($"Jumping: {FormatBonus(mods.jumpingBonus)}");
        if (mods.strengthBonus != 0) sb.AppendLine($"Strength: {FormatBonus(mods.strengthBonus)}");
        if (mods.agilityBonus != 0) sb.AppendLine($"Agility: {FormatBonus(mods.agilityBonus)}");
        if (mods.accuracyBonus != 0) sb.AppendLine($"Accuracy: {FormatBonus(mods.accuracyBonus)}");
        return sb.ToString();
    }

    private string FormatBonus(int bonus)
    {
        return bonus > 0 ? $"+{bonus}" : bonus.ToString();
    }

    private Color GetRarityColor(DNARarity rarity)
    {
        switch (rarity)
        {
            case DNARarity.Common: return Color.gray;
            case DNARarity.Uncommon: return Color.green;
            case DNARarity.Rare: return Color.blue;
            case DNARarity.Epic: return new Color(0.6f, 0f, 1f); // Purple
            case DNARarity.Legendary: return new Color(1f, 0.5f, 0f); // Orange
            default: return Color.white;
        }
    }

    private void OnSelect()
    {
        onSelected?.Invoke(dna);
    }
}

