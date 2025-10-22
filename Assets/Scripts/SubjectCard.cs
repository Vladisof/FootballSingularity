using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SubjectCard : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI statsText;
    public TextMeshProUGUI traitsText;
    public Button selectButton;

    private BaseSubject subject;

    public void Setup(BaseSubject subj)
    {
        subject = subj;

        if (nameText != null)
        {
            nameText.text = subject.subjectName;
        }

        if (statsText != null)
        {
            statsText.text = $"SPD: {subject.baseStats.speed} DEF: {subject.baseStats.defense}\n" +
                           $"ATK: {subject.baseStats.attack} STA: {subject.baseStats.stamina}\n" +
                           $"JMP: {subject.baseStats.jumping} STR: {subject.baseStats.strength}\n" +
                           $"AGI: {subject.baseStats.agility} ACC: {subject.baseStats.accuracy}\n" +
                           $"Overall: {subject.baseStats.GetOverallRating()}";
        }

        if (traitsText != null)
        {
            traitsText.text = "Traits: " + string.Join(", ", subject.naturalTraits);
        }

        if (selectButton != null)
        {
            // Очистити старі слухачі перед додаванням нового
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(OnSelectSubject);
        }
    }

    private void OnSelectSubject()
    {
        Debug.Log($"Subject card clicked: {subject.subjectName}");
        
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowMutationPanel(subject);
        }
        else
        {
            Debug.LogError("UIManager.Instance is null!");
        }
    }
}
