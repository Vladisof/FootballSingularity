using System;
using System.Collections.Generic;
using UnityEngine;

public class MutationProcess
{
    public BaseSubject subject;
    public List<DNAStrand> dnaStrands;
    public Action<MutationResult> onComplete;
    public float startTime;
    public float totalTime;
    public float elapsedTime;

    public float GetProgress()
    {
        return Mathf.Clamp01(elapsedTime / totalTime);
    }

    public float GetRemainingTime()
    {
        return Mathf.Max(0f, totalTime - elapsedTime);
    }
}

public class MutationResult
{
    public bool success;
    public PlayerStats mutatedPlayer;
    public List<DNAStrand> appliedDNA;
    public string failureMessage;
}

