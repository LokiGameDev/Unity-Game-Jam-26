using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Dialogue Elements")]
    [SerializeField] private TMP_Text speakerText;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Image portraitImage;
    [SerializeField] private Button continueButton;

    private Action continueCallback;

    private void Awake()
    {
        if (continueButton != null)
            continueButton.onClick.AddListener(HandleContinueClicked);

        Hide();
    }

    public void SetContinueCallback(Action callback)
    {
        continueCallback = callback;
    }

    public void ShowLine(DialogueLine line)
    {
        if (line == null)
            return;

        if (speakerText != null)
            speakerText.text = line.speakerName;

        if (dialogueText != null)
            dialogueText.text = line.text;

        if (portraitImage != null)
        {
            portraitImage.sprite = line.portrait;
            portraitImage.enabled = line.portrait != null;
        }

        SetVisible(true);
    }

    public void Hide()
    {
        SetVisible(false);
    }

    private void SetVisible(bool visible)
    {
        if (canvasGroup == null)
        {
            gameObject.SetActive(visible);
            return;
        }

        canvasGroup.alpha = visible ? 1f : 0f;
        canvasGroup.interactable = visible;
        canvasGroup.blocksRaycasts = visible;
    }

    private void HandleContinueClicked()
    {
        continueCallback?.Invoke();
    }

    private void OnDestroy()
    {
        if (continueButton != null)
            continueButton.onClick.RemoveListener(HandleContinueClicked);
    }
}
