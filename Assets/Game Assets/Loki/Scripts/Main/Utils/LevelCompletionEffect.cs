
using System.Collections;
using UnityEngine;

public class LevelCompletionEffect : MonoBehaviour
{
    public enum PlayMode
    {
        OnEnable,
        Manual
    }

    [Header("Play Settings")]
    [SerializeField] private PlayMode playMode = PlayMode.Manual;

    [Header("UI References")]
    [SerializeField] private CanvasGroup backgroundCanvas;
    [SerializeField] private CanvasGroup completionCanvas;
    [SerializeField] private RectTransform completionPanel;

    [Header("Background Fade")]
    [SerializeField] private float backgroundFadeIn = 0.2f;
    [SerializeField] private float backgroundFadeOut = 0.35f;

    [Header("Flip & Pop Animation")]
    [SerializeField] private float animationDuration = 0.45f;
    [SerializeField] private float flipAngle = 90f;
    [SerializeField] private AnimationCurve flipCurve =
        AnimationCurve.EaseInOut(0, 0, 1, 1);

    [SerializeField] private AnimationCurve scaleCurve =
        new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(0.3f, 1.3f),
            new Keyframe(0.6f, 0.85f),
            new Keyframe(0.8f, 1.1f),
            new Keyframe(1f, 1f)
        );

    [Header("Completion Settings")]
    [SerializeField] private float holdDuration = 1.5f;
    [SerializeField] private float completionFadeOut = 0.35f;

    [Header("Options")]
    [SerializeField] private bool useUnscaledTime = true;
    [SerializeField] private bool hideOnAwake = true;

    private Vector3 originalScale;
    private Quaternion originalRotation;
    private Coroutine animationCoroutine;

    private void Awake()
    {
        if (completionPanel != null)
        {
            originalScale = completionPanel.localScale;
            originalRotation = completionPanel.localRotation;
        }

        if (hideOnAwake)
            HideUI();
    }

    private void OnEnable()
    {
        if (playMode == PlayMode.OnEnable)
            PlayEffect();
    }

    public void PlayEffect()
    {
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        animationCoroutine = StartCoroutine(PlayAnimation());
    }

    public void StopEffect()
    {
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        animationCoroutine = null;
        HideUI();
    }

    private IEnumerator PlayAnimation()
    {
        backgroundCanvas.gameObject.SetActive(true);
        completionCanvas.gameObject.SetActive(true);

        backgroundCanvas.alpha = 0f;
        completionCanvas.alpha = 1f;

        completionPanel.localScale = Vector3.zero;
        completionPanel.localRotation =
            originalRotation * Quaternion.Euler(-flipAngle, 0f, 0f);

        // Background fades in.
        yield return FadeCanvas(backgroundCanvas, 0f, 1f,
            backgroundFadeIn);

        // Flip and scale happen simultaneously.
        yield return FlipAndPop();

        // Keep the completion panel visible.
        yield return Wait(holdDuration);

        // Fade everything out simultaneously.
        Coroutine bgFade = StartCoroutine(
            FadeCanvas(backgroundCanvas, 1f, 0f, backgroundFadeOut));

        Coroutine panelFade = StartCoroutine(
            FadeCanvas(completionCanvas, 1f, 0f, completionFadeOut));

        yield return bgFade;
        yield return panelFade;

        animationCoroutine = null;
        HideUI();
    }

    private IEnumerator FlipAndPop()
    {
        float elapsed = 0f;

        while (elapsed < animationDuration)
        {
            elapsed += DeltaTime();

            float t = Mathf.Clamp01(elapsed / animationDuration);

            // X-axis flip.
            float flipT = flipCurve.Evaluate(t);
            float angle = Mathf.Lerp(-flipAngle, 0f, flipT);

            completionPanel.localRotation =
                originalRotation * Quaternion.Euler(angle, 0f, 0f);

            // Simultaneous bouncing scale.
            float scale = scaleCurve.Evaluate(t);
            completionPanel.localScale = originalScale * scale;

            yield return null;
        }

        completionPanel.localRotation = originalRotation;
        completionPanel.localScale = originalScale;
    }

    private IEnumerator FadeCanvas(
        CanvasGroup canvas,
        float startAlpha,
        float endAlpha,
        float duration)
    {
        float elapsed = 0f;

        canvas.alpha = startAlpha;

        if (duration <= 0f)
        {
            canvas.alpha = endAlpha;
            yield break;
        }

        while (elapsed < duration)
        {
            elapsed += DeltaTime();

            float t = Mathf.Clamp01(elapsed / duration);
            canvas.alpha = Mathf.Lerp(startAlpha, endAlpha, t);

            yield return null;
        }

        canvas.alpha = endAlpha;
    }

    private IEnumerator Wait(float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += DeltaTime();
            yield return null;
        }
    }

    private float DeltaTime()
    {
        return useUnscaledTime
            ? Time.unscaledDeltaTime
            : Time.deltaTime;
    }

    private void HideUI()
    {
        if (backgroundCanvas != null)
        {
            backgroundCanvas.alpha = 0f;
            backgroundCanvas.gameObject.SetActive(false);
        }

        if (completionCanvas != null)
        {
            completionCanvas.alpha = 0f;
            completionCanvas.gameObject.SetActive(false);
        }

        if (completionPanel != null)
        {
            completionPanel.localScale = originalScale;
            completionPanel.localRotation = originalRotation;
        }

        gameObject.SetActive(false);
    }
}
