using System.Collections.Generic;
using UnityEngine;

public class ReputationManager : MonoBehaviour
{
    public static ReputationManager Instance { get; private set; }

    private Dictionary<string, float> teamReputations = new Dictionary<string, float>();
    private List<string> availableTeams = new List<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeTeams();
            LoadReputations();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeTeams()
    {
        // Major clubs
        availableTeams.Add("Manchester United");
        availableTeams.Add("Barcelona");
        availableTeams.Add("Real Madrid");
        availableTeams.Add("Bayern Munich");
        availableTeams.Add("Liverpool");
        availableTeams.Add("Paris Saint-Germain");
        availableTeams.Add("Juventus");
        availableTeams.Add("AC Milan");
        availableTeams.Add("Chelsea");
        availableTeams.Add("Arsenal");

        // National teams
        availableTeams.Add("Brazil National Team");
        availableTeams.Add("Argentina National Team");
        availableTeams.Add("Germany National Team");
        availableTeams.Add("France National Team");
        availableTeams.Add("England National Team");
        availableTeams.Add("Spain National Team");
    }

    private void LoadReputations()
    {
        foreach (string team in availableTeams)
        {
            string key = "Reputation_" + team;
            // Початкова репутація 0-20 для нових гравців (рівень "Початківці")
            float defaultRep = Random.Range(0f, 20f);
            float rep = PlayerPrefs.GetFloat(key, defaultRep);
            teamReputations[team] = rep;
        }
    }

    public void AddReputation(string teamName, float amount)
    {
        if (!teamReputations.ContainsKey(teamName))
        {
            teamReputations[teamName] = 50f;
        }

        teamReputations[teamName] = Mathf.Clamp(teamReputations[teamName] + amount, 0f, 100f);
        SaveReputation(teamName);
    }

    public void SubtractReputation(string teamName, float amount)
    {
        AddReputation(teamName, -amount);
    }

    public void ChangeReputation(string teamName, float amount)
    {
        AddReputation(teamName, amount);
    }

    public float GetReputation(string teamName)
    {
        if (!teamReputations.ContainsKey(teamName))
        {
            return 50f;
        }
        return teamReputations[teamName];
    }

    private void SaveReputation(string teamName)
    {
        string key = "Reputation_" + teamName;
        PlayerPrefs.SetFloat(key, teamReputations[teamName]);
        PlayerPrefs.Save();
    }

    public string GetRandomTeam()
    {
        return availableTeams[Random.Range(0, availableTeams.Count)];
    }

    public List<string> GetAllTeams()
    {
        return new List<string>(availableTeams);
    }

    public void ResetAllReputations()
    {
        foreach (string team in availableTeams)
        {
            teamReputations[team] = 50f;
            SaveReputation(team);
        }
    }

    // Decay reputation over time (called periodically)
    public void ApplyReputationDecay()
    {
        foreach (string team in availableTeams)
        {
            if (teamReputations[team] > 50f)
            {
                // Decay higher reputations slowly back toward 50
                AddReputation(team, -0.5f);
            }
        }
    }
}
