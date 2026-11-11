
using UnityEngine;
using UnityEngine.Events;

public class BellMechanismPuzzle : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent<int, int> onCountChanged;
    public UnityEvent onSolved;

    private int targetHour;
    private int targetMinute;

    private int currentHour;
    private int currentMinute;

    private bool solved;

    private void Start()
    {
        if (!LuckyNumberManager.Instance || !LuckyNumberManager.Instance.HasLuckyNumber)
        {
            Debug.LogError("Lucky number has not been selected!");
            enabled = false;
            return;
        }

        LuckyNumberManager.Instance.GetDigits(out targetHour, out targetMinute);

        UpdateDisplay();
    }

    public void PullHoursBell()
    {
        if (solved)
            return;

        if(currentHour >= 12)
        {
            currentHour = 0;
        }
        else
        {
            currentHour++;
        }

        UpdateDisplay();
    }

    public void PullMinutesBell()
    {
        if (solved)
            return;

        if(currentMinute >= 59)
        {
            PullHoursBell();
            currentMinute = 0;
        }
        else
        {
            currentMinute++;
        }

        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        onCountChanged?.Invoke(currentHour, currentMinute);

        Debug.Log("Bell count: " +
                  currentHour + currentMinute);
    }

    public bool CheckSolution()
    {
        if (currentHour == targetHour &&
            currentMinute == targetMinute)
        {
            return SolvePuzzle();
        }

        return false;
    }

    private bool SolvePuzzle()
    {
        if (solved)
            return true;

        solved = true;

        Debug.Log("CLOCK TOWER PUZZLE SOLVED!");
        onSolved?.Invoke();

        return true;
    }

    public void ResetPuzzle()
    {
        if (solved)
            return;

        currentHour = 0;
        currentMinute = 0;

        UpdateDisplay();
    }
}
