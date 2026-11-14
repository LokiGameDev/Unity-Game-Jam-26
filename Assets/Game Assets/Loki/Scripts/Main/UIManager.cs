
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Interaction UI")]
    [SerializeField] private RectTransform interactionPrompt;
    [SerializeField] private Image interactionImage;
    [SerializeField] private Canvas canvas;

    [Header("Scene Transition UI")]
    [SerializeField] private GameObject sceneTransitionObject;
    [SerializeField] private CanvasGroup sceneTransitionCanvas;

    [SerializeField] private GameObject puzzleCompletedObject;

    private Transform currentShowPoint;
    private MonoBehaviour currentOwner;

    private Camera mainCamera;

    private Coroutine coroutine;
    
    public bool isPanelOpen { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        mainCamera = Camera.main;

        interactionPrompt.gameObject.SetActive(false);
    }

    private void Start()
    {
        sceneTransitionObject.SetActive(false);
        sceneTransitionCanvas.alpha = 0;
        puzzleCompletedObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if (currentShowPoint == null)
            return;

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null)
            return;

        Vector3 screenPosition =
            mainCamera.WorldToScreenPoint(currentShowPoint.position);

        // Hide if the point is behind the camera
        if (screenPosition.z <= 0)
        {
            interactionPrompt.gameObject.SetActive(false);
            return;
        }

        interactionPrompt.gameObject.SetActive(true);

        RectTransform canvasRect =
            canvas.transform as RectTransform;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPosition,
            null,
            out Vector2 localPosition
        );

        interactionPrompt.localPosition = localPosition;
    }

    public void ShowInteraction(MonoBehaviour owner,Transform showPoint,Sprite icon = null)
    {
        if (showPoint == null) return;

        currentOwner = owner;
        currentShowPoint = showPoint;

        if (icon != null)
            interactionImage.sprite = icon;

        interactionPrompt.gameObject.SetActive(true);
    }

    public void HideInteraction(MonoBehaviour owner)
    {
        // Only the current interactable can hide its prompt
        if (currentOwner != owner)
            return;

        currentOwner = null;
        currentShowPoint = null;

        interactionPrompt.gameObject.SetActive(false);
    }

    public void HideInteraction()
    {
        currentOwner = null;
        currentShowPoint = null;

        interactionPrompt.gameObject.SetActive(false);
    }

    public void SceneSwapping(SpawnAreaPoints spawnPoint)
    {
        if(coroutine!=null) StopCoroutine(coroutine);

        coroutine = StartCoroutine(SceneTransition(spawnPoint));
    }

    private IEnumerator SceneTransition(SpawnAreaPoints spawnPoint)
    {
        sceneTransitionObject.SetActive(true);
        sceneTransitionCanvas.alpha = 0;

        float time = 0.5f;
        float elapsed = 0;

        while (elapsed < time)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / time);

            sceneTransitionCanvas.alpha = Mathf.Lerp(0,1,t);

            yield return null;
        }

        elapsed = 0;
        sceneTransitionCanvas.alpha = 1;

        GameManager.Instance.TeleportPlayerToArea(spawnPoint);

        yield return new WaitForSeconds(1);

        while (elapsed < time)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / time);

            sceneTransitionCanvas.alpha = Mathf.Lerp(1,0,t);

            yield return null;
        }

        sceneTransitionCanvas.alpha = 0;
        sceneTransitionObject.SetActive(false);
    }

    public void SetPanelStatus(bool state)
    {
        isPanelOpen = state;
    }

    public void PuzzleCompleted()
    {
        puzzleCompletedObject.SetActive(true);
    }
}
