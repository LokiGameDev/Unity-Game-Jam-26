
using UnityEngine;
using TMPro;

public class BellCounterDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text counterText;

    public void UpdateCounter(int hour, int minute)
    {
        counterText.text = $"{hour:D2} : {minute:D2}";
    }
}
