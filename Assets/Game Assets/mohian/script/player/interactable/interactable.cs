using UnityEngine;

public class interactableObject : MonoBehaviour, IInteractable
{
    public Transform interactingPoint { get ; set ; }
    
    public void Interact(GameObject player)
    {
        Debug.Log("Interacted with " + gameObject.name);
    }
}