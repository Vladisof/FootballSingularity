using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BaseSubject
{
    public string subjectId;
    public string subjectName;
    public PlayerStats baseStats;
    public List<string> naturalTraits;

    // Порожній конструктор для серіалізації Unity
    public BaseSubject()
    {
        subjectId = "";
        subjectName = "";
        baseStats = new PlayerStats();
        naturalTraits = new List<string>();
    }

    // Статичний метод для створення випадкового суб'єкта
    public static BaseSubject CreateRandom()
    {
        BaseSubject subject = new BaseSubject();
        subject.subjectId = Guid.NewGuid().ToString();
        subject.subjectName = GenerateRandomName();
        subject.baseStats = PlayerStats.CreateRandom();
        subject.naturalTraits = new List<string>();
        
        // Add 1-2 random natural traits
        int traitCount = UnityEngine.Random.Range(1, 3);
        string[] possibleTraits = { "Athletic", "Resilient", "Quick Reflexes", "Natural Talent", "Hard Worker" };
        for (int i = 0; i < traitCount; i++)
        {
            string trait = possibleTraits[UnityEngine.Random.Range(0, possibleTraits.Length)];
            if (!subject.naturalTraits.Contains(trait))
            {
                subject.naturalTraits.Add(trait);
            }
        }
        
        return subject;
    }

    private static string GenerateRandomName()
    {
        string[] firstNames = { "Alex", "Jordan", "Taylor", "Morgan", "Casey", "Jamie", "Riley", "Avery", "Quinn", "Blake" };
        string[] lastNames = { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Martinez", "Lopez" };
        return firstNames[UnityEngine.Random.Range(0, firstNames.Length)] + " " + lastNames[UnityEngine.Random.Range(0, lastNames.Length)];
    }
}
