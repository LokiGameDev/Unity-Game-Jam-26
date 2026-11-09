using UnityEngine;
using UnityEngine.Events;

public class ClockMakerTable : MonoBehaviour, IInteractable
{
    public UnityEvent OnInteractEvent;

    [field: SerializeField] public Transform interactingPoint { get ; set ; }

    public void Interact(GameObject interactor)
    {
        OnInteractEvent?.Invoke();
    }
}
