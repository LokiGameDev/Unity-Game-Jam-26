
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class RecordDeskArea : MonoBehaviour
{
    public enum SlotType
    {
        Tens,
        Units
    }

    [Serializable]
    public class DeskSlot
    {
        public string slotName;
        public RectTransform slotTransform;
        public SlotType slotType;

        [NonSerialized]
        public List<DraggableRecord> records =
            new List<DraggableRecord>();

        [NonSerialized]
        public int currentTotal;
    }

    [Header("References")]
    [SerializeField] private RectTransform dragArea;

    [Header("Desk Slots")]
    [SerializeField] private DeskSlot[] slots;

    [Header("Snap Settings")]
    [SerializeField] private float snapDistance = 60f;
    [SerializeField] private float stackOffset = 8f;

    [Header("Events")]
    public UnityEvent<int> onCorrectRecordPlaced;
    public UnityEvent onPuzzleSolved;

    private int targetTens;
    private int targetUnits;
    private int targetNumber;

    private bool solved;

    private void Start()
    {
        if (LuckyNumberManager.Instance == null ||
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

        targetNumber = targetTens * 10 + targetUnits;

        foreach (DeskSlot slot in slots)
        {
            slot.records = new List<DraggableRecord>();
            slot.currentTotal = 0;
        }

        Debug.Log($"Required number: {targetNumber}");

        LuckyNumberManager.Instance.LuckyNumberSelected.AddListener(UpdateLuckyNumber);

        CheckPuzzleCompletion();
    }

    private void UpdateLuckyNumber()
    {
        LuckyNumberManager.Instance.GetDigits(
            out targetTens,
            out targetUnits
        );

        targetNumber = targetTens * 10 + targetUnits;
    }

    public void TryPlaceRecord(DraggableRecord record)
    {
        if (record == null || solved)
            return;

        RectTransform recordRect =
            record.GetComponent<RectTransform>();

        Vector2 recordPosition =
            dragArea.InverseTransformPoint(recordRect.position);

        DeskSlot nearestSlot = null;
        float nearestDistance = float.MaxValue;

        foreach (DeskSlot slot in slots)
        {
            if (slot.slotTransform == null)
                continue;

            Vector2 slotPosition =
                dragArea.InverseTransformPoint(
                    slot.slotTransform.position
                );

            float distance =
                Vector2.Distance(recordPosition, slotPosition);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestSlot = slot;
            }
        }

        if (nearestSlot == null ||
            nearestDistance > snapDistance)
        {
            Debug.Log($"Record {record.Digit} cannot be placed here.");
            return;
        }

        AddRecordToSlot(record, nearestSlot);
    }

    private void AddRecordToSlot(
        DraggableRecord record,
        DeskSlot slot)
    {
        if (slot.records.Contains(record))
            return;

        slot.records.Add(record);

        // Calculate place value.
        int multiplier = slot.slotType == SlotType.Tens ? 10 : 1;

        slot.currentTotal += record.Digit * multiplier;

        record.OnSnapped();

        UpdateStackPositions(slot);

        int currentNumber = GetCurrentTotal();

        Debug.Log(
            $"{slot.slotName}: {slot.currentTotal} | Current Number: {currentNumber}"
        );

        // Check the current number after every placement.
        onCorrectRecordPlaced?.Invoke(currentNumber);

        CheckPuzzleCompletion();
    }

    private void UpdateStackPositions(DeskSlot slot)
    {
        Vector3 basePosition =
            dragArea.InverseTransformPoint(
                slot.slotTransform.position
            );

        for (int i = 0; i < slot.records.Count; i++)
        {
            DraggableRecord record = slot.records[i];

            if (record == null)
                continue;

            RectTransform recordRect =
                record.GetComponent<RectTransform>();

            Vector3 stackPosition = basePosition +
                new Vector3(0, i * stackOffset, 0);

            recordRect.localPosition = stackPosition;

            recordRect.SetAsLastSibling();
        }
    }

    public void ReleaseRecord(DraggableRecord record)
    {
        foreach (DeskSlot slot in slots)
        {
            if (!slot.records.Remove(record))
                continue;

            record.OnDesnapped();

            RecalculateSlotTotal(slot);

            UpdateStackPositions(slot);

            Debug.Log(
                $"Removed record. {slot.slotName}: {slot.currentTotal}"
            );

            // Check again after removing a record.
            CheckPuzzleCompletion();

            return;
        }
    }

    private void RecalculateSlotTotal(DeskSlot slot)
    {
        slot.currentTotal = 0;

        int multiplier =
            slot.slotType == SlotType.Tens ? 10 : 1;

        foreach (DraggableRecord remaining in slot.records)
        {
            if (remaining != null)
                slot.currentTotal += remaining.Digit * multiplier;
        }
    }

    private void CheckPuzzleCompletion()
    {
        int currentNumber = GetCurrentTotal();

        if (currentNumber != targetNumber)
            return;

        if (solved)
            return;

        solved = true;

        Debug.Log("TOWN RECORDS PUZZLE SOLVED!");

        UIManager.Instance.PuzzleCompleted();

        onPuzzleSolved?.Invoke();
    }

    public int GetSlotTotal(SlotType type)
    {
        foreach (DeskSlot slot in slots)
        {
            if (slot.slotType == type)
                return slot.currentTotal;
        }

        return 0;
    }

    private int GetCurrentTotal()
    {
        int total = 0;

        foreach (DeskSlot slot in slots)
        {
            total += slot.currentTotal;
        }

        return total;
    }

    public bool IsSolved()
    {
        return solved;
    }
}
