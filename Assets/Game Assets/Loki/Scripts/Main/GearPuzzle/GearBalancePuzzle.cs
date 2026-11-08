
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GearBalancePuzzle : MonoBehaviour
{
    [Header("Gear Values")]
    [SerializeField]
    private int[] gearValues = { 1, 2, 4, 8, 16, 32, 64 };

    [Header("Events")]
    public UnityEvent<int> onValueChanged;
    public UnityEvent onSolved;

    private HashSet<int> selectedGears = new HashSet<int>();

    private int targetNumber;
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

        targetNumber = LuckyNumberManager.Instance.LuckyNumber;

        Debug.Log("Gear puzzle target: " + targetNumber);
        UpdateValue();
    }

    // Connect this to each gear's interaction/button.
    public void ToggleGear(int gearValue)
    {
        if (solved || !System.Array.Exists(gearValues,
            value => value == gearValue))
            return;

        if (selectedGears.Contains(gearValue))
        {
            selectedGears.Remove(gearValue);
        }
        else
        {
            // Prevent the player from exceeding the target.
            if (currentValue + gearValue > targetNumber)
                return;

            selectedGears.Add(gearValue);
        }

        UpdateValue();
    }

    private void UpdateValue()
    {
        currentValue = 0;

        foreach (int value in selectedGears)
        {
            currentValue += value;
        }

        onValueChanged?.Invoke(currentValue);

        Debug.Log("Current gear value: " + currentValue);

        if (currentValue == targetNumber)
        {
            SolvePuzzle();
        }
    }

    private void SolvePuzzle()
    {
        if (solved)
            return;

        solved = true;

        Debug.Log("GEAR PUZZLE SOLVED!");
        onSolved?.Invoke();
    }

    public void ResetPuzzle()
    {
        if (solved)
            return;

        selectedGears.Clear();
        UpdateValue();
    }
}
