
using UnityEngine;

public class BellRopeInteraction : MonoBehaviour, IInteractable
{
    public enum RopeType
    {
        Tens,
        Units
    }

    [Header("Settings")]
    [SerializeField] private BellMechanismPuzzle puzzle;
    [SerializeField] private RopeType ropeType;

    public Transform interactingPoint { get ; set ; }

    // Call this from your interaction system
    public void Interact(GameObject interactor)
    {
        if (puzzle == null)
        {
            Debug.LogWarning("Bell Mechanism Puzzle not assigned!");
            return;
        }

        if (ropeType == RopeType.Tens)
        {
            puzzle.PullTensBell();
        }
        else
        {
            puzzle.PullUnitsBell();
        }
    }
}
