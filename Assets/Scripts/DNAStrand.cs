using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DNAStrand
{
    public string id;
    public string displayName;
    public DNACategory category;
    public DNARarity rarity;
    public string description;
    public List<TraitEffect> traits;
    public StatModifiers statModifiers;
    public Sprite icon;

    public DNAStrand(string id, string name, DNACategory cat, DNARarity rar, string desc)
    {
        this.id = id;
        this.displayName = name;
        this.category = cat;
        this.rarity = rar;
        this.description = desc;
        this.traits = new List<TraitEffect>();
        this.statModifiers = new StatModifiers();
    }
}

[Serializable]
public class StatModifiers
{
    public int speedBonus;
    public int defenseBonus;
    public int attackBonus;
    public int staminaBonus;
    public int jumpingBonus;
    public int strengthBonus;
    public int agilityBonus;
    public int accuracyBonus;

    public StatModifiers()
    {
        speedBonus = 0;
        defenseBonus = 0;
        attackBonus = 0;
        staminaBonus = 0;
        jumpingBonus = 0;
        strengthBonus = 0;
        agilityBonus = 0;
        accuracyBonus = 0;
    }

    public void ApplyTo(PlayerStats stats)
    {
        stats.speed += speedBonus;
        stats.defense += defenseBonus;
        stats.attack += attackBonus;
        stats.stamina += staminaBonus;
        stats.jumping += jumpingBonus;
        stats.strength += strengthBonus;
        stats.agility += agilityBonus;
        stats.accuracy += accuracyBonus;
    }
}

[Serializable]
public class TraitEffect
{
    public string traitName;
    public string description;
}

public enum DNACategory
{
    Animal,
    LegendaryPlayer,
    Environment,
    Mechanical
}

public enum DNARarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

