
using UnityEngine;
using TMPro;

public class BellCounterDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text counterText;

    public void UpdateCounter(int tens, int units)
    {
        counterText.text = $"{tens}{units}";
    }
}
