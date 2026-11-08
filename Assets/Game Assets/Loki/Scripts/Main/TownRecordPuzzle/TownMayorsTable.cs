using UnityEngine;
using UnityEngine.Events;

public class TownMayorsTable : MonoBehaviour, IInteractable
{
    public UnityEvent OnInteractEvent;

    public void Interact(GameObject interactor)
    {
        OnInteractEvent?.Invoke();
    }
}
