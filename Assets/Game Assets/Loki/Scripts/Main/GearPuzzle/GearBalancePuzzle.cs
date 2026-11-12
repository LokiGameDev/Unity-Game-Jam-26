
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GearBalancePuzzle : MonoBehaviour
{
    [Header("Gear Values")]
    [SerializeField]
    private int[] gearValues = { 1, 2, 4, 8, 16, 32, 64 };
    [SerializeField] private TMP_Text currentValueText;

    [Header("Events")]
    public UnityEvent<int> onValueChanged;
    public UnityEvent onSolved;
    public UnityEvent<PuzzleType> onSolvedType;

    private int currentValue;
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

        UpdateValue();
    }

    // Connect this to each gear's interaction/button.
    public void AddGear(int gearValue)
    {
        if (solved)
            return;

            // Prevent the player from exceeding the target.
        currentValue += gearValue;

        UpdateValue();
    }

    public void RemoveGear(int gearValue)
    {
        if (solved)
            return;

            // Prevent the player from exceeding the target.
        currentValue -= gearValue;

        UpdateValue();
    }

    private void UpdateValue()
    {
        onValueChanged?.Invoke(currentValue);

        if (currentValue == LuckyNumberManager.Instance.LuckyNumber)
        {
            SolvePuzzle();
        }

        currentValueText.text = $"{currentValue}";
    }

    private void SolvePuzzle()
    {
        if (solved)
            return;

        solved = true;

        Debug.Log("GEAR PUZZLE SOLVED!");
        onSolved?.Invoke();
        onSolvedType?.Invoke(PuzzleType.GearBalance);
    }

    public void ResetPuzzle()
    {
        if (solved)
            return;

        UpdateValue();
    }
}
