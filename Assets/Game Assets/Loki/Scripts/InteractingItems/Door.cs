
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private SpawnAreaPoints spawnArea;

    [field: SerializeField] public Transform interactingPoint { get ; set ; }

    private bool isOpen;

    public void Interact(GameObject interactor)
    {
        isOpen = !isOpen;

        Debug.Log(isOpen ? "Door Opened" : "Door Closed");

        if(isOpen) DoorOpened();
    }

    private void DoorOpened()
    {
        UIManager.Instance.SceneSwapping(spawnArea);
    }
}
