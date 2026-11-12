using UnityEngine;
using UnityEngine.UI;

public class ColorChangerForItems : MonoBehaviour
{
    [SerializeField] private Image image;

    [SerializeField] private Color normalColor;
    [SerializeField] private Color snappedColor;

    public void SetSnapStateColor(bool state)
    {
        if(state) image.color = snappedColor;
        else image.color = normalColor;
    }
}
