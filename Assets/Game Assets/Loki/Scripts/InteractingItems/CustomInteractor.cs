using UnityEngine;
using UnityEngine.Events;

public class CustomInteractor : MonoBehaviour, IInteractable
{
    [SerializeField] private BellMechanismPuzzle puzzle;
    [SerializeField] private DialogueDisplayer displayer;
    [field: SerializeField] public Transform interactingPoint { get ; set ; }

    public void Interact(GameObject interactor)
    {
        if(!puzzle.CheckSolution())
        {
            displayer.ShowDialogue("Try somethings else!");
        }
    }
}
