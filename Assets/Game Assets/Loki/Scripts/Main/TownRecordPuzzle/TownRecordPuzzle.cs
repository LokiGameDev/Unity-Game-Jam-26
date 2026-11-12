
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TownRecordsPuzzle : MonoBehaviour
{
    public enum RecordSlot
    {
        Tens,
        Units
    }

    [Header("Events")]
    public UnityEvent onSolved;
    public UnityEvent<PuzzleType> onSolvedType;
    [SerializeField] private TMP_Text currentRecordValue;

    private int targetTens;
    private int targetUnits;

    private int placedTens = -1;
    private int placedUnits = -1;

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
    }

    // Call this when the player places a record.
    public void PlaceRecord(int digit, RecordSlot slot)
    {
        if (solved || digit < 0 || digit > 9)
            return;

        if (slot == RecordSlot.Tens)
        {
            placedTens = digit;
        }
        else
        {
            placedUnits = digit;
        }

        Debug.Log("Tens: " + placedTens +
                  " | Units: " + placedUnits);

        currentRecordValue.text = $"{placedTens}{placedUnits}";

        CheckSolution();
    }

    private void CheckSolution()
    {
        if (placedTens == targetTens &&
            placedUnits == targetUnits)
        {
            SolvePuzzle();
        }
    }

    private void SolvePuzzle()
    {
        if (solved)
            return;

        solved = true;

        Debug.Log("TOWN RECORDS PUZZLE SOLVED!");
        onSolved?.Invoke();
        onSolvedType?.Invoke(PuzzleType.MayorRecord);
    }

    public void RemoveRecord(RecordSlot slot)
    {
        if (solved)
            return;

        if (slot == RecordSlot.Tens)
            placedTens = -1;
        else
            placedUnits = -1;
    }

    public void CurrentValueUpdate(int value)
    {
        currentRecordValue.text = $"value";
    }

    public void SolveThePuzzle()
    {
        SolvePuzzle();
    }
}
