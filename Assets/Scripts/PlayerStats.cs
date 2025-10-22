using System;
using UnityEngine;

[Serializable]
public class PlayerStats
{
    public int speed;
    public int defense;
    public int attack;
    public int stamina;
    public int jumping;
    public int strength;
    public int agility;
    public int accuracy;

    // Порожній конструктор для серіалізації
    public PlayerStats()
    {
        // Не ініціалізуємо тут Random - це викликає помилку!
    }

    // Конструктор з параметрами
    public PlayerStats(int speed, int defense, int attack, int stamina, int jumping, int strength, int agility, int accuracy)
    {
        this.speed = speed;
        this.defense = defense;
        this.attack = attack;
        this.stamina = stamina;
        this.jumping = jumping;
        this.strength = strength;
        this.agility = agility;
        this.accuracy = accuracy;
    }

    // Статичний метод для створення випадкових статів
    public static PlayerStats CreateRandom(int min = 30, int max = 60)
    {
        return new PlayerStats(
            UnityEngine.Random.Range(min, max + 1),
            UnityEngine.Random.Range(min, max + 1),
            UnityEngine.Random.Range(min, max + 1),
            UnityEngine.Random.Range(min, max + 1),
            UnityEngine.Random.Range(min, max + 1),
            UnityEngine.Random.Range(min, max + 1),
            UnityEngine.Random.Range(min, max + 1),
            UnityEngine.Random.Range(min, max + 1)
        );
    }

    public void Clamp(int min = 0, int max = 99)
    {
        speed = Mathf.Clamp(speed, min, max);
        defense = Mathf.Clamp(defense, min, max);
        attack = Mathf.Clamp(attack, min, max);
        stamina = Mathf.Clamp(stamina, min, max);
        jumping = Mathf.Clamp(jumping, min, max);
        strength = Mathf.Clamp(strength, min, max);
        agility = Mathf.Clamp(agility, min, max);
        accuracy = Mathf.Clamp(accuracy, min, max);
    }

    public int GetOverallRating()
    {
        return (speed + defense + attack + stamina + jumping + strength + agility + accuracy) / 8;
    }

    public PlayerStats Clone()
    {
        PlayerStats clone = new PlayerStats();
        clone.speed = this.speed;
        clone.defense = this.defense;
        clone.attack = this.attack;
        clone.stamina = this.stamina;
        clone.jumping = this.jumping;
        clone.strength = this.strength;
        clone.agility = this.agility;
        clone.accuracy = this.accuracy;
        return clone;
    }
}
