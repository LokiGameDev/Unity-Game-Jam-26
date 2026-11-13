using System;
using UnityEngine;
using UnityEngine.Events;

public class ClockMaker : MonoBehaviour, IInteractable
{
    [SerializeField] private StoryEventData storyEventData;
    [SerializeField] private DialogueData dialogueData;
    [SerializeField] private TaskData taskData;

    [field: SerializeField] public Transform interactingPoint { get ; set ; }

    private bool isStoryStarted = false;

    public UnityEvent interactedEvent;

    public void Interact(GameObject interactor)
    {
        if(!isStoryStarted)
        {
            isStoryStarted = true;
            interactedEvent?.Invoke();
            StartStory();
        }
    }

    public void StartStory()
    {
        StoryManager.Instance.StartStoryEvent(storyEventData);
    }
}
