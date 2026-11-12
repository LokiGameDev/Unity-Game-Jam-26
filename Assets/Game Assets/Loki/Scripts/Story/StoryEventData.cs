using UnityEngine;

[CreateAssetMenu(fileName = "StoryEvent_", menuName = "The Ninth Hour/Narrative/Story Event")]
public class StoryEventData : ScriptableObject
{
    [Tooltip("Unique ID for debugging and identifying this story event.")]
    public string eventID;

    [Header("Event Flow")]
    [Tooltip("Optional objective to complete when this event starts. Leave empty if not needed.")]
    public string objectiveToCompleteID;

    [Tooltip("Dialogue played during this event. Can be empty.")]
    public DialogueData dialogue;

    [Tooltip("Objective assigned after this event's dialogue finishes.")]
    public TaskData objectiveToSet;

    [Tooltip("Optional event to start automatically after this event finishes.")]
    public StoryEventData nextEvent;

    public bool autoStartNextEvent;
}
