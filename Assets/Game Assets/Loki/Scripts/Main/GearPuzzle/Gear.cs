using TMPro;
using UnityEngine;

public class Gear : MonoBehaviour, IInteractable
{
    [SerializeField] private GearBalancePuzzle gearBalancePuzzle;
    [SerializeField] private int gearValue;
    [SerializeField] private TMP_Text gearValueText;
    public Transform interactingPoint { get ; set ; }

    private void Start()
    {
        gearValueText.text = gearValue.ToString();
    }

    public void Interact(GameObject interactor)
    {
        gearBalancePuzzle.ToggleGear(gearValue);
    }
}
