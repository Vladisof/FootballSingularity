using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MutationCard : MonoBehaviour
{
    public TextMeshProUGUI subjectNameText;
    public TextMeshProUGUI progressText;
    public TextMeshProUGUI timeRemainingText;
    public Slider progressBar;
    public TextMeshProUGUI dnaListText;

    private MutationProcess mutation;

    public void Setup(MutationProcess mutationProcess)
    {
        mutation = mutationProcess;
        UpdateDisplay();
    }

    private void Update()
    {
        if (mutation != null)
        {
            UpdateDisplay();
        }
    }

    private void UpdateDisplay()
    {
        if (mutation == null) return;

        // Subject name
        if (subjectNameText != null)
        {
            subjectNameText.text = mutation.subject.subjectName;
        }

        // Progress
        float progress = mutation.GetProgress();
        if (progressBar != null)
        {
            progressBar.value = progress;
        }

        if (progressText != null)
        {
            progressText.text = $"{(progress * 100):F0}%";
        }

        // Time remaining
        if (timeRemainingText != null)
        {
            float remaining = mutation.GetRemainingTime();
            int minutes = Mathf.FloorToInt(remaining / 60f);
            int seconds = Mathf.FloorToInt(remaining % 60f);
            timeRemainingText.text = $"⏱ {minutes}:{seconds:00}";
        }

        // DNA list
        if (dnaListText != null && mutation.dnaStrands != null)
        {
            string dnaNames = "";
            for (int i = 0; i < mutation.dnaStrands.Count; i++)
            {
                if (i > 0) dnaNames += ", ";
                dnaNames += mutation.dnaStrands[i].displayName;
            }
            dnaListText.text = $"DNA: {dnaNames}";
        }
    }
}
