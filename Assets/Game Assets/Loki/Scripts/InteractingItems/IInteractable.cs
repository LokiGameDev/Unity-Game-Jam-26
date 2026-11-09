
using UnityEngine;

public interface IInteractable
{
    void Interact(GameObject interactor);
    public Transform interactingPoint {get; set;}
}
