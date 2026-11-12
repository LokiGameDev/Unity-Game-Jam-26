using System;
using UnityEngine;

[Serializable]
public class HintStep
{
    [Tooltip("Dialogue shown for this hint level.")]
    public DialogueData dialogue;

    [Header("Automatic Triggers")]
    public bool triggerOnTime = true;
    [Min(0f)] public float secondsWithoutProgress = 60f;

    public bool triggerOnFailedAttempts = true;
    [Min(0)] public int failedAttempts = 3;
}

[CreateAssetMenu(fileName = "Hints_", menuName = "The Ninth Hour/Narrative/Hint Set")]
public class HintData : ScriptableObject
{
    [Tooltip("Unique ID for the puzzle or activity this hint set belongs to.")]
    public string hintSetID;

    public HintStep[] hints;

    [Tooltip("Minimum time between automatically displayed hints.")]
    [Min(0f)] public float automaticHintCooldown = 20f;
}
