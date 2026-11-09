using UnityEngine;

public class NoticeBoard : MonoBehaviour, IInteractable
{
    [field: SerializeField] public Transform interactingPoint { get ; set ; }
    [SerializeField] private DialogueDisplayer dialogueDisplayer;

    public void Interact(GameObject interactor)
    {
        NoticeBoardInteracted();
    }

    private void NoticeBoardInteracted()
    {
        dialogueDisplayer.ShowDialogue("Nothing to show here!");
    }
}

