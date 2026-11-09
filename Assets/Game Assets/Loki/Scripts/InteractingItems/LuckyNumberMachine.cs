using UnityEngine;

public class LuckyNumberMachine : MonoBehaviour, IInteractable
{
    [field: SerializeField] public Transform interactingPoint { get ; set ; }

    private bool isOpen;

    public void Interact(GameObject interactor)
    {
        isOpen = !isOpen;

        Debug.Log(isOpen ? "Door Opened" : "Door Closed");

        if(isOpen) MachineStarted();
    }

    private void MachineStarted()
    {
        
    }
}

