
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(RectTransform))]
public class UIEffects : MonoBehaviour
{
    public enum PlayMode
    {
        OnEnable,
        Manual
    }

    public enum PopDirection
    {
        Center,
        Left,
        Right,
        Top,
        Bottom
    }

    public enum RotationDirection
    {
        Clockwise,
        CounterClockwise
    }

    [Header("Trigger")]
    [SerializeField] private PlayMode playMode = PlayMode.OnEnable;

    [Header("Effects")]
    [SerializeField] private bool useScale = true;
    [SerializeField] private bool usePop;
    [SerializeField] private bool useRotation;

    [Header("Scale Settings")]
    [SerializeField] private Vector3 startScale = Vector3.zero;
    [SerializeField] private Vector3 endScale = Vector3.one;
    [SerializeField] private bool scaleLoop;
    [SerializeField] private float scaleMultiplier = 1.15f;

    [Header("Pop Settings")]
    [SerializeField] private PopDirection popDirection =
        PopDirection.Center;

    [SerializeField] private float popDistance = 200f;

    [Header("Rotation Settings")]
    [SerializeField] private float startRotation = -90f;
    [SerializeField] private float endRotation = 0f;

    [Header("Animation Settings")]
    [SerializeField] private float duration = 0.35f;
    [SerializeField] private AnimationCurve animationCurve =
        AnimationCurve.EaseInOut(0, 0, 1, 1);

    [SerializeField] private bool rotateLoop;
    [SerializeField] private RotationDirection rotationDirection =
        RotationDirection.Clockwise;

    [SerializeField] private float rotationSpeed = 180f;

    private RectTransform rect;
    private Coroutine animationRoutine;

    private Vector3 originalScale;
    private Vector2 originalPosition;
    private Quaternion originalRotation;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();

        originalScale = rect.localScale;
        originalPosition = rect.anchoredPosition;
        originalRotation = rect.localRotation;
    }

    private void OnEnable()
    {
        if (playMode == PlayMode.OnEnable)
        {
            PlayEffect();
        }
    }

    public void PlayEffect()
    {
        if (animationRoutine != null)
            StopCoroutine(animationRoutine);

        animationRoutine = StartCoroutine(Animate());
    }

    public void StopEffect()
    {
        if (animationRoutine != null)
        {
            StopCoroutine(animationRoutine);
            animationRoutine = null;
        }
    }

    public void ResetEffect()
    {
        StopEffect();

        rect.localScale = originalScale;
        rect.anchoredPosition = originalPosition;
        rect.localRotation = originalRotation;
    }

    private IEnumerator Animate()
    {
        Vector3 fromScale = useScale
            ? Vector3.Scale(originalScale, startScale)
            : rect.localScale;

        Vector3 toScale = useScale
            ? Vector3.Scale(originalScale, endScale)
            : rect.localScale;

        Vector2 fromPosition = originalPosition;
        Vector2 toPosition = originalPosition;

        if (usePop)
        {
            fromPosition += GetPopOffset();
        }

        Quaternion fromRotation = useRotation
            ? originalRotation * Quaternion.Euler(0, 0, startRotation)
            : rect.localRotation;

        Quaternion toRotation = useRotation
            ? originalRotation * Quaternion.Euler(0, 0, endRotation)
            : rect.localRotation;

        rect.localScale = fromScale;
        rect.anchoredPosition = fromPosition;
        rect.localRotation = fromRotation;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / duration);
            float curveValue = animationCurve.Evaluate(t);

            if (useScale)
                rect.localScale = Vector3.LerpUnclamped(
                    fromScale, toScale, curveValue);

            if (usePop)
                rect.anchoredPosition = Vector2.LerpUnclamped(
                    fromPosition, toPosition, curveValue);

            if (useRotation)
                rect.localRotation = Quaternion.SlerpUnclamped(
                    fromRotation, toRotation, curveValue);

            yield return null;
        }

        rect.localScale = toScale;
        rect.anchoredPosition = toPosition;
        rect.localRotation = toRotation;

        animationRoutine = null;

        if (scaleLoop && useScale)
        {
            StartCoroutine(ScalePulse());
        }

        if (rotateLoop && useRotation)
        {
            StartCoroutine(RotateLoop());
        }
    }

    private IEnumerator ScalePulse()
    {
        Vector3 normalScale = rect.localScale;
        Vector3 enlargedScale = normalScale * scaleMultiplier;

        while (enabled && gameObject.activeInHierarchy)
        {
            yield return AnimateScale(normalScale, enlargedScale);
            yield return AnimateScale(enlargedScale, normalScale);
        }
    }

    private IEnumerator RotateLoop()
    {
        float direction = rotationDirection == RotationDirection.Clockwise
            ? -1f
            : 1f;

        while (enabled && gameObject.activeInHierarchy)
        {
            rect.Rotate(
                0f,
                0f,
                direction * rotationSpeed * Time.unscaledDeltaTime
            );

            yield return null;
        }
    }

    private IEnumerator AnimateScale(Vector3 from, Vector3 to)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / duration);
            float curveValue = animationCurve.Evaluate(t);

            rect.localScale = Vector3.LerpUnclamped(
                from, to, curveValue);

            yield return null;
        }

        rect.localScale = to;
    }

    private Vector2 GetPopOffset()
    {
        switch (popDirection)
        {
            case PopDirection.Left:
                return Vector2.left * popDistance;

            case PopDirection.Right:
                return Vector2.right * popDistance;

            case PopDirection.Top:
                return Vector2.up * popDistance;

            case PopDirection.Bottom:
                return Vector2.down * popDistance;

            case PopDirection.Center:
            default:
                return Vector2.zero;
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        animationRoutine = null;
    }
}
