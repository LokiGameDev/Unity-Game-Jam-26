
using UnityEngine;
using UnityEngine.Events;

public class BellMechanismPuzzle : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent<int, int> onCountChanged;
    public UnityEvent onSolved;

    private int targetTens;
    private int targetUnits;

    private int currentTens;
    private int currentUnits;

    private bool solved;

    private void Start()
    {
        if (!LuckyNumberManager.Instance ||
            !LuckyNumberManager.Instance.HasLuckyNumber)
        {
            Debug.LogError("Lucky number has not been selected!");
            enabled = false;
            return;
        }

        LuckyNumberManager.Instance.GetDigits(
            out targetTens,
            out targetUnits
        );

        UpdateDisplay();
    }

    public void PullTensBell()
    {
        if (solved || currentTens >= 9)
            return;

        currentTens++;

        UpdateDisplay();
        CheckSolution();
    }

    public void PullUnitsBell()
    {
        if (solved || currentUnits >= 9)
            return;

        currentUnits++;

        UpdateDisplay();
        CheckSolution();
    }

    private void UpdateDisplay()
    {
        onCountChanged?.Invoke(currentTens, currentUnits);

        Debug.Log("Bell count: " +
                  currentTens + currentUnits);
    }

    private void CheckSolution()
    {
        if (currentTens == targetTens &&
            currentUnits == targetUnits)
        {
            SolvePuzzle();
        }
    }

    private void SolvePuzzle()
    {
        if (solved)
            return;

        solved = true;

        Debug.Log("CLOCK TOWER PUZZLE SOLVED!");
        onSolved?.Invoke();
    }

    public void ResetPuzzle()
    {
        if (solved)
            return;

        currentTens = 0;
        currentUnits = 0;

        UpdateDisplay();
    }
}
