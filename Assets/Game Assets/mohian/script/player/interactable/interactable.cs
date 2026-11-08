using UnityEngine;

public class interactableObject : MonoBehaviour, IInteractable
{
    public void Interact(GameObject player)
    {
        Debug.Log("Interacted with " + gameObject.name);
    }
}