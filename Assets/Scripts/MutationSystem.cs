using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MutationSystem : MonoBehaviour
{
    public static MutationSystem Instance { get; private set; }

    [Header("Mutation Settings")]
    public float baseMutationTime = 30f; // seconds
    public float baseFailureChance = 0.25f; // 25%
    public int maxSimultaneousMutations = 3;

    private List<MutationProcess> activeMutations = new List<MutationProcess>();

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
        }
    }

    public bool StartMutation(BaseSubject subject, List<DNAStrand> dnaStrands, Action<MutationResult> onComplete)
    {
        if (activeMutations.Count >= maxSimultaneousMutations)
        {
            Debug.LogWarning($"Cannot start mutation - maximum {maxSimultaneousMutations} mutations in progress!");
            return false;
        }

        if (dnaStrands.Count < 2 || dnaStrands.Count > 3)
        {
            Debug.LogWarning("Must select 2 or 3 DNA strands!");
            return false;
        }

        float mutationSpeed = LabUpgradeManager.Instance != null ? 
            LabUpgradeManager.Instance.GetMutationSpeedMultiplier() : 1f;

        MutationProcess mutation = new MutationProcess
        {
            subject = subject,
            dnaStrands = dnaStrands,
            onComplete = onComplete,
            startTime = Time.time,
            totalTime = baseMutationTime / mutationSpeed
        };

        activeMutations.Add(mutation);
        StartCoroutine(ProcessMutation(mutation));
        
        Debug.Log($"Mutation started for {subject.subjectName}. Time: {mutation.totalTime}s");
        return true;
    }

    private IEnumerator ProcessMutation(MutationProcess mutation)
    {
        // Get upgrades
        float failureReduction = LabUpgradeManager.Instance != null ? 
            LabUpgradeManager.Instance.GetFailureReduction() : 0f;

        float failureChance = Mathf.Max(0.05f, baseFailureChance - failureReduction);

        // Wait for mutation time
        float elapsed = 0f;
        while (elapsed < mutation.totalTime)
        {
            elapsed += Time.deltaTime;
            mutation.elapsedTime = elapsed;
            yield return null;
        }

        // Calculate result
        MutationResult result = CalculateMutationResult(mutation, failureChance);
        
        mutation.onComplete?.Invoke(result);
        activeMutations.Remove(mutation);
        
        Debug.Log($"Mutation completed for {mutation.subject.subjectName}. Success: {result.success}");
    }

    private MutationResult CalculateMutationResult(MutationProcess mutation, float failureChance)
    {
        MutationResult result = new MutationResult();
        
        // Check if mutation fails
        float roll = UnityEngine.Random.Range(0f, 1f);
        if (roll < failureChance)
        {
            result.success = false;
            result.failureMessage = GetRandomFailureMessage();
            return result;
        }

        // Success! Create mutated player
        result.success = true;
        result.mutatedPlayer = new PlayerStats();

        // Start with base subject stats
        PlayerStats finalStats = mutation.subject.baseStats.Clone();

        // Apply each DNA strand
        foreach (DNAStrand dna in mutation.dnaStrands)
        {
            dna.statModifiers.ApplyTo(finalStats);
        }

        // Add slight randomization (-5 to +5 on each stat)
        finalStats.speed += UnityEngine.Random.Range(-5, 6);
        finalStats.defense += UnityEngine.Random.Range(-5, 6);
        finalStats.attack += UnityEngine.Random.Range(-5, 6);
        finalStats.stamina += UnityEngine.Random.Range(-5, 6);
        finalStats.jumping += UnityEngine.Random.Range(-5, 6);
        finalStats.strength += UnityEngine.Random.Range(-5, 6);
        finalStats.agility += UnityEngine.Random.Range(-5, 6);
        finalStats.accuracy += UnityEngine.Random.Range(-5, 6);

        // Clamp stats to 0-99 range
        finalStats.Clamp(0, 99);

        result.mutatedPlayer = finalStats;
        result.appliedDNA = new List<DNAStrand>(mutation.dnaStrands);

        // Позначити, що гра потребує збереження після мутації
        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.MarkDirty();
        }

        return result;
    }

    private string GetRandomFailureMessage()
    {
        string[] messages = {
            "Mutation failed - DNA strands rejected the subject.",
            "Critical failure - Subject cannot sustain mutations.",
            "Mutation unstable - Process aborted.",
            "DNA incompatibility detected - Mutation failed.",
            "Subject's immune system rejected the mutation."
        };
        return messages[UnityEngine.Random.Range(0, messages.Length)];
    }

    public List<MutationProcess> GetActiveMutations()
    {
        return new List<MutationProcess>(activeMutations);
    }

    public bool IsMutating()
    {
        return activeMutations.Count > 0;
    }

    public int GetActiveMutationCount()
    {
        return activeMutations.Count;
    }
}
