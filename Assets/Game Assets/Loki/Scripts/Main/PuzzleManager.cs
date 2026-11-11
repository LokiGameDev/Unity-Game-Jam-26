using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    [SerializeField] private GameObject gearPuzzlePanel;
    [SerializeField] private GameObject townRecordPanel;
    [SerializeField] private GameObject luckynumberPanel;

    private void Start()
    {
        SetGearPuzzlePanel(false);
        SetTownRecordPanel(false);
        SetLuckyPanelStatus(false);
    } 

    public void SetGearPuzzlePanel(bool state)
    {
        gearPuzzlePanel.SetActive(state);
        UIManager.Instance.SetPanelStatus(state);
    }

    public void SetTownRecordPanel(bool state)
    {
        townRecordPanel.SetActive(state);
        UIManager.Instance.SetPanelStatus(state);
    }

    public void SetLuckyPanelStatus(bool state)
    {
        luckynumberPanel.SetActive(state);
        UIManager.Instance.SetPanelStatus(state);
    }
}
