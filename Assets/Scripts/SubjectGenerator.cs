using System.Collections.Generic;
using UnityEngine;

public class SubjectGenerator : MonoBehaviour
{
    public static SubjectGenerator Instance { get; private set; }

    private List<BaseSubject> availableSubjects = new List<BaseSubject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            GenerateInitialSubjects();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void GenerateInitialSubjects()
    {
        // Generate 3 random subjects
        for (int i = 0; i < 3; i++)
        {
            availableSubjects.Add(BaseSubject.CreateRandom());
        }
    }

    public List<BaseSubject> GetAvailableSubjects()
    {
        return new List<BaseSubject>(availableSubjects);
    }

    public BaseSubject GetSubject(string subjectId)
    {
        return availableSubjects.Find(s => s.subjectId == subjectId);
    }

    public void UseSubject(string subjectId)
    {
        BaseSubject subject = availableSubjects.Find(s => s.subjectId == subjectId);
        if (subject != null)
        {
            availableSubjects.Remove(subject);
            // Generate a new subject to replace it
            availableSubjects.Add(BaseSubject.CreateRandom());
        }
    }

    public void RefreshAllSubjects(int cost)
    {
        if (MoneyController.Instance != null && MoneyController.Instance.SubtractMoney(cost))
        {
            availableSubjects.Clear();
            GenerateInitialSubjects();
            Debug.Log("All subjects refreshed!");
        }
    }

    public void ResetSubjects()
    {
        availableSubjects.Clear();
        GenerateInitialSubjects();
        Debug.Log("Subjects reset for new game!");
    }
}
