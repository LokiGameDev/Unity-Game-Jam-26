using UnityEngine;

public class HintManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DialogueManager dialogueManager;

    private HintData currentHintData;
    private int nextHintIndex;
    private int failedAttemptCount;
    private float timeSinceProgress;
    private float automaticHintCooldownTimer;
    private bool hintContextActive;
    private bool isShowingHint;

    private void Awake()
    {
        if (dialogueManager == null)
            dialogueManager = DialogueManager.Instance;
    }

    private void Update()
    {
        if (!hintContextActive || currentHintData == null)
            return;

        timeSinceProgress += Time.deltaTime;

        if (automaticHintCooldownTimer > 0f)
            automaticHintCooldownTimer -= Time.deltaTime;

        TryShowAutomaticHint();
    }

    public void BeginHintContext(HintData hintData)
    {
        currentHintData = hintData;
        nextHintIndex = 0;
        failedAttemptCount = 0;
        timeSinceProgress = 0f;
        automaticHintCooldownTimer = 0f;
        hintContextActive = hintData != null;
        isShowingHint = false;
    }

    public void EndHintContext()
    {
        hintContextActive = false;
        currentHintData = null;
        isShowingHint = false;
    }

    // Call when the player makes meaningful progress on the puzzle.
    public void ReportProgress()
    {
        timeSinceProgress = 0f;
        failedAttemptCount = 0;
    }

    // Call when the player makes an incorrect attempt.
    public void ReportFailedAttempt()
    {
        failedAttemptCount++;
        TryShowAutomaticHint();
    }

    // Connect this to a Hint button if you want manual hints.
    public void RequestHint()
    {
        if (!hintContextActive || isShowingHint)
            return;

        ShowNextHint();
    }

    private void TryShowAutomaticHint()
    {
        if (!hintContextActive || isShowingHint)
            return;

        if (automaticHintCooldownTimer > 0f)
            return;

        if (dialogueManager != null && dialogueManager.IsPlaying)
            return;

        if (currentHintData.hints == null ||
            nextHintIndex >= currentHintData.hints.Length)
            return;

        HintStep step = currentHintData.hints[nextHintIndex];

        bool timeReached =
            step.triggerOnTime &&
            timeSinceProgress >= step.secondsWithoutProgress;

        bool attemptsReached =
            step.triggerOnFailedAttempts &&
            step.failedAttempts > 0 &&
            failedAttemptCount >= step.failedAttempts;

        if (timeReached || attemptsReached)
            ShowNextHint();
    }

    private void ShowNextHint()
    {
        if (currentHintData == null ||
            currentHintData.hints == null ||
            nextHintIndex >= currentHintData.hints.Length)
            return;

        if (dialogueManager == null)
            dialogueManager = DialogueManager.Instance;

        // Do not consume a hint if another dialogue is playing.
        if (dialogueManager == null || dialogueManager.IsPlaying)
            return;

        HintStep step = currentHintData.hints[nextHintIndex];
        nextHintIndex++;

        if (step.dialogue == null)
        {
            Debug.LogWarning(
                $"Hint step {nextHintIndex} in '{currentHintData.name}' has no dialogue.",
                currentHintData
            );
            return;
        }

        isShowingHint = true;
        automaticHintCooldownTimer =
            currentHintData.automaticHintCooldown;

        dialogueManager.StartDialogue(step.dialogue, () =>
        {
            isShowingHint = false;
            timeSinceProgress = 0f;
            failedAttemptCount = 0;
        });
    }
}
