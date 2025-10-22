using System.Collections.Generic;
using UnityEngine;

public class DNALibrary : MonoBehaviour
{
    public static DNALibrary Instance { get; private set; }

    private List<DNAStrand> unlockedDNA = new List<DNAStrand>();
    private List<DNAStrand> allPossibleDNA = new List<DNAStrand>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAllDNA();
            LoadUnlockedDNA();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAllDNA()
    {
        // ANIMAL DNA
        allPossibleDNA.Add(CreateDNA("dna_gazelle", "Gazelle DNA", DNACategory.Animal, DNARarity.Common, 
            "Speed boost from nature's fastest runners", 15, 0, 0, 5, 0, -5, 10, 0));
        
        allPossibleDNA.Add(CreateDNA("dna_gorilla", "Gorilla DNA", DNACategory.Animal, DNARarity.Common,
            "Massive strength and power", -5, 10, 5, 0, 5, 20, -5, 0));
        
        allPossibleDNA.Add(CreateDNA("dna_owl", "Owl DNA", DNACategory.Animal, DNARarity.Uncommon,
            "Enhanced perception and accuracy", 0, 5, 0, 0, 0, 0, 5, 15));
        
        allPossibleDNA.Add(CreateDNA("dna_cheetah", "Cheetah DNA", DNACategory.Animal, DNARarity.Rare,
            "Explosive speed and agility", 20, 0, 5, -5, 0, 0, 15, 0));
        
        allPossibleDNA.Add(CreateDNA("dna_dolphin", "Dolphin DNA", DNACategory.Animal, DNARarity.Uncommon,
            "Enhanced agility and stamina", 5, 0, 0, 10, 0, 0, 10, 0));

        // LEGENDARY PLAYER DNA
        allPossibleDNA.Add(CreateDNA("dna_10", "Number 10 DNA", DNACategory.LegendaryPlayer, DNARarity.Legendary,
            "The magic of the greatest", 10, 5, 15, 10, 5, 5, 15, 15));
        
        allPossibleDNA.Add(CreateDNA("dna_7", "Number 7 DNA", DNACategory.LegendaryPlayer, DNARarity.Legendary,
            "Speed and precision combined", 15, 0, 20, 10, 10, 10, 10, 15));
        
        allPossibleDNA.Add(CreateDNA("dna_goalgod", "GoalGod DNA", DNACategory.LegendaryPlayer, DNARarity.Epic,
            "Pure attacking instinct", 10, 0, 25, 5, 10, 5, 5, 20));

        // ENVIRONMENT DNA
        allPossibleDNA.Add(CreateDNA("dna_ice", "Ice DNA", DNACategory.Environment, DNARarity.Uncommon,
            "Cool under pressure, steady performance", 0, 10, 0, 15, 0, 5, 5, 10));
        
        allPossibleDNA.Add(CreateDNA("dna_lava", "Lava DNA", DNACategory.Environment, DNARarity.Rare,
            "Explosive power and intensity", 10, -5, 15, 10, 5, 15, 0, 5));
        
        allPossibleDNA.Add(CreateDNA("dna_wind", "Wind DNA", DNACategory.Environment, DNARarity.Common,
            "Swift movement and agility", 10, 0, 0, 5, 0, 0, 15, 0));
        
        allPossibleDNA.Add(CreateDNA("dna_lightning", "Lightning DNA", DNACategory.Environment, DNARarity.Epic,
            "Blinding speed and reflexes", 20, 0, 10, 5, 5, 0, 20, 10));

        // MECHANICAL DNA
        allPossibleDNA.Add(CreateDNA("dna_drone", "Drone DNA", DNACategory.Mechanical, DNARarity.Uncommon,
            "Enhanced aerial ability", 5, 0, 0, 10, 20, 0, 10, 5));
        
        allPossibleDNA.Add(CreateDNA("dna_magnet", "Magnet DNA", DNACategory.Mechanical, DNARarity.Rare,
            "Ball control and attraction", 0, 10, 10, 0, 0, 5, 5, 15));
        
        allPossibleDNA.Add(CreateDNA("dna_jetpack", "Jetpack DNA", DNACategory.Mechanical, DNARarity.Epic,
            "Explosive jumping and speed", 15, 0, 5, 5, 25, 0, 10, 0));
        
        allPossibleDNA.Add(CreateDNA("dna_titanium", "Titanium DNA", DNACategory.Mechanical, DNARarity.Rare,
            "Unbreakable defense", 0, 20, 0, 15, 5, 15, 0, 0));
    }

    private DNAStrand CreateDNA(string id, string name, DNACategory category, DNARarity rarity, 
        string description, int speed, int defense, int attack, int stamina, int jumping, 
        int strength, int agility, int accuracy)
    {
        DNAStrand dna = new DNAStrand(id, name, category, rarity, description);
        dna.statModifiers.speedBonus = speed;
        dna.statModifiers.defenseBonus = defense;
        dna.statModifiers.attackBonus = attack;
        dna.statModifiers.staminaBonus = stamina;
        dna.statModifiers.jumpingBonus = jumping;
        dna.statModifiers.strengthBonus = strength;
        dna.statModifiers.agilityBonus = agility;
        dna.statModifiers.accuracyBonus = accuracy;
        return dna;
    }

    private void LoadUnlockedDNA()
    {
        // Start with some basic DNA unlocked
        if (!PlayerPrefs.HasKey("DNALibrary_Initialized"))
        {
            // Unlock starter DNA
            UnlockDNA("dna_gazelle");
            UnlockDNA("dna_gorilla");
            UnlockDNA("dna_wind");
            UnlockDNA("dna_drone");
            PlayerPrefs.SetInt("DNALibrary_Initialized", 1);
            PlayerPrefs.Save();
        }
        else
        {
            // Load from PlayerPrefs
            string savedDNA = PlayerPrefs.GetString("UnlockedDNA", "");
            if (!string.IsNullOrEmpty(savedDNA))
            {
                string[] dnaIds = savedDNA.Split(',');
                foreach (string id in dnaIds)
                {
                    DNAStrand dna = allPossibleDNA.Find(d => d.id == id);
                    if (dna != null && !unlockedDNA.Contains(dna))
                    {
                        unlockedDNA.Add(dna);
                    }
                }
            }
        }
    }

    public void UnlockDNA(string dnaId)
    {
        DNAStrand dna = allPossibleDNA.Find(d => d.id == dnaId);
        if (dna != null && !unlockedDNA.Contains(dna))
        {
            unlockedDNA.Add(dna);
            SaveUnlockedDNA();
        }
    }

    private void SaveUnlockedDNA()
    {
        List<string> ids = new List<string>();
        foreach (DNAStrand dna in unlockedDNA)
        {
            ids.Add(dna.id);
        }
        PlayerPrefs.SetString("UnlockedDNA", string.Join(",", ids));
        PlayerPrefs.Save();
    }

    public List<DNAStrand> GetUnlockedDNA()
    {
        return new List<DNAStrand>(unlockedDNA);
    }

    public DNAStrand GetRandomUnresearchedDNA(DNACategory category)
    {
        List<DNAStrand> unresearched = allPossibleDNA.FindAll(d => 
            d.category == category && !unlockedDNA.Contains(d));
        
        if (unresearched.Count == 0) return null;

        // Weighted random based on rarity
        float totalWeight = 0f;
        foreach (var dna in unresearched)
        {
            totalWeight += GetRarityWeight(dna.rarity);
        }

        float random = Random.Range(0, totalWeight);
        float currentWeight = 0f;

        foreach (var dna in unresearched)
        {
            currentWeight += GetRarityWeight(dna.rarity);
            if (random <= currentWeight)
            {
                return dna;
            }
        }

        return unresearched[0];
    }

    private float GetRarityWeight(DNARarity rarity)
    {
        switch (rarity)
        {
            case DNARarity.Common: return 50f;
            case DNARarity.Uncommon: return 30f;
            case DNARarity.Rare: return 15f;
            case DNARarity.Epic: return 4f;
            case DNARarity.Legendary: return 1f;
            default: return 10f;
        }
    }

    public void ResetLibrary()
    {
        unlockedDNA.Clear();
        PlayerPrefs.DeleteKey("DNALibrary_Initialized");
        PlayerPrefs.DeleteKey("UnlockedDNA");
        LoadUnlockedDNA();
    }
}

