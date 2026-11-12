using System;
using UnityEngine;
using UnityEngine.Events;

public class StoryManager : MonoBehaviour
{
    public static StoryManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private TaskManager taskManager;

    [Header("Events")]
    public UnityEvent<string> onStoryEventStarted;
    public UnityEvent<string> onStoryEventCompleted;

    public StoryEventData CurrentEvent { get; private set; }
    public bool IsRunningEvent { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("More than one StoryManager exists. Destroying duplicate.", this);
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (dialogueManager == null)
            dialogueManager = DialogueManager.Instance;

        if (taskManager == null)
            taskManager = TaskManager.Instance;
    }

    public void StartStoryEvent(StoryEventData storyEvent)
    {
        if (storyEvent == null)
        {
            Debug.LogWarning("Cannot start a null story event.", this);
            return;
        }

        if (IsRunningEvent)
        {
            Debug.LogWarning(
                $"Story event '{CurrentEvent?.eventID}' is still running. " +
                $"'{storyEvent.eventID}' was not started.",
                this
            );
            return;
        }

        if (storyEvent.dialogue != null)
        {
            if (dialogueManager == null)
                dialogueManager = DialogueManager.Instance;

            if (dialogueManager != null && dialogueManager.IsPlaying)
            {
                Debug.LogWarning(
                    "Cannot start this story event because another dialogue is already playing.",
                    this
                );
                return;
            }
        }

        CurrentEvent = storyEvent;
        IsRunningEvent = true;

        onStoryEventStarted?.Invoke(storyEvent.eventID);

        if (!string.IsNullOrWhiteSpace(storyEvent.objectiveToCompleteID))
            taskManager?.CompleteObjective(storyEvent.objectiveToCompleteID);

        if (storyEvent.dialogue != null)
        {
            if (dialogueManager == null)
                dialogueManager = DialogueManager.Instance;

            if (dialogueManager == null)
            {
                Debug.LogError("StoryManager needs a DialogueManager.", this);
                FinishCurrentEvent();
                return;
            }

            dialogueManager.StartDialogue(
                storyEvent.dialogue,
                FinishCurrentEvent
            );
        }
        else
        {
            FinishCurrentEvent();
        }
    }

    private void FinishCurrentEvent()
    {
        if (!IsRunningEvent || CurrentEvent == null)
            return;

        StoryEventData finishedEvent = CurrentEvent;

        if (finishedEvent.objectiveToSet != null)
        {
            if (taskManager == null)
                taskManager = TaskManager.Instance;

            taskManager?.SetObjective(finishedEvent.objectiveToSet);
        }

        IsRunningEvent = false;
        CurrentEvent = null;

        onStoryEventCompleted?.Invoke(finishedEvent.eventID);

        if (finishedEvent.autoStartNextEvent && finishedEvent.nextEvent != null)
            StartStoryEvent(finishedEvent.nextEvent);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
