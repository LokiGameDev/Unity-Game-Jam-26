using System;
using UnityEngine;
using UnityEngine.Events;

public class HotelManager : MonoBehaviour, IInteractable
{
    [SerializeField] private StoryEventData storyEventData;
    [SerializeField] private DialogueData dialogueData;
    [SerializeField] private TaskData taskData;

    [field: SerializeField] public Transform interactingPoint { get ; set ; }

    private bool isStoryStarted = false;

    public UnityEvent interactedEvent;

    public void Interact(GameObject interactor)
    {
        interactedEvent?.Invoke();

        if(!isStoryStarted)
        {
            isStoryStarted = true;
            StartStory();
        }
        else if(!LuckyNumberManager.Instance.HasLuckyNumber)
        {
            StartHint();
        }
    }

    public void StartStory()
    {
        StoryManager.Instance.StartStoryEvent(storyEventData);
    }

    public void StartHint()
    {
        DialogueManager.Instance.StartDialogue(dialogueData);
    }
}
