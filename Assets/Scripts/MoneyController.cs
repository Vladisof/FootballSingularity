using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoneyController : MonoBehaviour
{
  public static MoneyController Instance { get; private set; }

  public TextMeshProUGUI moneyText;
  private float money;

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
      return;
    }
  }

  private void Start()
  {
    if (!PlayerPrefs.HasKey("Money"))
    {
        money = 50;
        SaveMoney();
    }
    else
    {
        money = PlayerPrefs.GetFloat("Money", 50);
    }
    UpdateMoneyText();
  }

  public void AddMoney(float amount)
  {
    money += amount;
    SaveMoney();
    UpdateMoneyText();
    
    // Позначити, що гра потребує збереження
    if (SaveSystem.Instance != null)
    {
      SaveSystem.Instance.MarkDirty();
    }
  }

  public bool SubtractMoney(int amount)
  {
    if (money >= amount)
    {
      money -= amount;
      SaveMoney();
      UpdateMoneyText();
      
      // Позначити, що гра потребує збереження
      if (SaveSystem.Instance != null)
      {
        SaveSystem.Instance.MarkDirty();
      }
      
      return true;
    }
    else
    {
      Debug.LogWarning("Недостаточно монет для выполнения операции.");
      return false;
    }
  }

  private void UpdateMoneyText()
  {
    moneyText.text = "" + money.ToString("F0");
    if (money <= 10)
    {
      money = 10;
      moneyText.text = "" + money.ToString("F0");
    }
  }

  private void SaveMoney()
  {
    PlayerPrefs.SetFloat("Money", money);
    PlayerPrefs.Save();
  }

  private void OnApplicationQuit()
  {
    SaveMoney();
  }
  
  // Method to get current money amount
  public float GetMoney()
  {
    return money;
  }

  public bool HasEnoughMoney(int amount)
  {
    return money >= amount;
  }

  public float GetCurrentMoney()
  {
    return money;
  }

  // Add method to reset money to default for progress reset
  public void ResetToDefault()
  {
    money = 50; // Reset to default starting money
    SaveMoney();
    UpdateMoneyText();
  }
}
