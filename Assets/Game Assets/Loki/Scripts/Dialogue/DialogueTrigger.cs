using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue")]
    [SerializeField] private DialogueData dialogue;
    [SerializeField] private bool playOnlyOnce;

    private bool hasPlayed;

    // Call this from your existing player interaction script.
    public void Interact()
    {
        if (playOnlyOnce && hasPlayed)
            return;

        if (dialogue == null)
        {
            Debug.LogWarning($"No dialogue assigned to {name}.", this);
            return;
        }

        if (DialogueManager.Instance == null)
        {
            Debug.LogError("No DialogueManager exists in the scene.", this);
            return;
        }

        if (DialogueManager.Instance.IsPlaying)
            return;

        DialogueManager.Instance.StartDialogue(dialogue, () =>
        {
            if (playOnlyOnce)
                hasPlayed = true;
        });
    }

    public void ResetTrigger()
    {
        hasPlayed = false;
    }
}
