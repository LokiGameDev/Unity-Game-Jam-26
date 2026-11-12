using TMPro;
using UnityEngine;

public class Gear : MonoBehaviour
{
    [SerializeField] private int gearValue;
    [SerializeField] private TMP_Text gearValueText;
    public Transform interactingPoint { get ; set ; }

    public int GearValue { get => gearValue ;}

    public void Initialize(int value)
    {
        gearValue = value;

        if (gearValueText != null)
            gearValueText.text = value.ToString();
    }
}
