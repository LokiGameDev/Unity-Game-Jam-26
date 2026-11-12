using System;
using UnityEngine;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private DialogueUI dialogueUI;

    [Header("Events")]
    public UnityEvent onDialogueStarted;
    public UnityEvent onDialogueFinished;

    public bool IsPlaying { get; private set; }

    private DialogueData currentDialogue;
    private int currentLineIndex;
    private Action completionCallback;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("More than one DialogueManager exists. Destroying duplicate.", this);
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (dialogueUI != null)
            dialogueUI.SetContinueCallback(ContinueDialogue);
        else
            Debug.LogError("DialogueUI reference is missing.", this);
    }

    public void StartDialogue(DialogueData dialogue, Action onComplete = null)
    {
        if (dialogue == null)
        {
            Debug.LogWarning("Cannot start a null dialogue.", this);
            onComplete?.Invoke();
            return;
        }

        if (dialogue.lines == null || dialogue.lines.Length == 0)
        {
            Debug.LogWarning($"Dialogue '{dialogue.name}' has no lines.", dialogue);
            onComplete?.Invoke();
            return;
        }

        if (IsPlaying)
        {
            Debug.LogWarning("A dialogue is already playing. The new dialogue was ignored.", this);
            return;
        }

        currentDialogue = dialogue;
        currentLineIndex = 0;
        completionCallback = onComplete;
        IsPlaying = true;

        onDialogueStarted?.Invoke();
        DisplayCurrentLine();
    }

    public void ContinueDialogue()
    {
        if (!IsPlaying)
            return;

        currentLineIndex++;

        if (currentLineIndex >= currentDialogue.lines.Length)
        {
            FinishDialogue();
            return;
        }

        DisplayCurrentLine();
    }

    public void StopDialogue()
    {
        if (!IsPlaying)
            return;

        FinishDialogue();
    }

    private void DisplayCurrentLine()
    {
        dialogueUI.ShowLine(currentDialogue.lines[currentLineIndex]);
    }

    private void FinishDialogue()
    {
        IsPlaying = false;

        if (dialogueUI != null)
            dialogueUI.Hide();

        currentDialogue = null;
        currentLineIndex = 0;

        Action callback = completionCallback;
        completionCallback = null;

        onDialogueFinished?.Invoke();
        callback?.Invoke();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
