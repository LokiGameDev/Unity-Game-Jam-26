
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    private bool isOpen;

    public void Interact(GameObject interactor)
    {
        isOpen = !isOpen;

        Debug.Log(isOpen ? "Door Opened" : "Door Closed");

        if(isOpen) DoorOpened();
    }

    private void DoorOpened()
    {
        Color color = spriteRenderer.color;
        color.a = 0;
        spriteRenderer.color = color;
    }
}
