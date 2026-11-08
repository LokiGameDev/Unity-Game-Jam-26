using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    [SerializeField] private GameObject gearPuzzlePanel;
    [SerializeField] private GameObject townRecordPanel;

    private void Start()
    {
        SetGearPuzzlePanel(false);
        SetTownRecordPanel(false);
    } 

    public void SetGearPuzzlePanel(bool state)
    {
        gearPuzzlePanel.SetActive(state);
    }

    public void SetTownRecordPanel(bool state)
    {
        townRecordPanel.SetActive(state);
    }
}
