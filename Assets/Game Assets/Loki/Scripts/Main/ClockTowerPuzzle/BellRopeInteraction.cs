
using UnityEngine;

public class BellRopeInteraction : MonoBehaviour, IInteractable
{
    public enum RopeType
    {
        Hour,
        Minute
    }

    [Header("Settings")]
    [SerializeField] private BellMechanismPuzzle puzzle;
    [SerializeField] private RopeType ropeType;

    [field:SerializeField] public Transform interactingPoint { get ; set ; }

    // Call this from your interaction system
    public void Interact(GameObject interactor)
    {
        if (puzzle == null)
        {
            Debug.LogWarning("Bell Mechanism Puzzle not assigned!");
            return;
        }

        if (ropeType == RopeType.Hour)
        {
            puzzle.PullHoursBell();
        }
        else
        {
            puzzle.PullMinutesBell();
        }
    }
}
