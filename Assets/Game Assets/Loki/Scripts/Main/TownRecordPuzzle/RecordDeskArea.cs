
using System;
using System.Collections.Generic;
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
    public UnityEvent onCorrectRecordPlaced;
    public UnityEvent onPuzzleSolved;

    private int targetTens;
    private int targetUnits;

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

        foreach (DeskSlot slot in slots)
        {
            slot.records = new List<DraggableRecord>();
            slot.currentTotal = 0;
        }

        Debug.Log(
            $"Required number: {targetTens}{targetUnits}"
        );

        CheckPuzzleCompletion();
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

            int requiredDigit = GetRequiredDigit(slot.slotType);

            // No records are needed for a zero digit.
            if (requiredDigit == 0)
                continue;

            // Don't allow the total to exceed the required digit.
            if (slot.currentTotal + record.Digit > requiredDigit)
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
            Debug.Log(
                $"Record {record.Digit} cannot be placed here."
            );
            return;
        }

        AddRecordToSlot(record, nearestSlot);
    }

    private void AddRecordToSlot(
        DraggableRecord record,
        DeskSlot slot)
    {
        slot.records.Add(record);

        slot.currentTotal += record.Digit;

        UpdateStackPositions(slot);

        Debug.Log(
            $"{slot.slotName}: {slot.currentTotal}"
        );

        onCorrectRecordPlaced?.Invoke();

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

            // Keep the top record visible.
            recordRect.SetAsLastSibling();
        }
    }

    public void ReleaseRecord(DraggableRecord record)
    {
        foreach (DeskSlot slot in slots)
        {
            if (!slot.records.Remove(record))
                continue;

            slot.currentTotal = 0;

            // Recalculate the total after removing the record.
            foreach (DraggableRecord remaining in slot.records)
            {
                if (remaining != null)
                    slot.currentTotal += remaining.Digit;
            }

            UpdateStackPositions(slot);

            Debug.Log(
                $"Removed record. {slot.slotName}: {slot.currentTotal}"
            );

            return;
        }
    }

    private int GetRequiredDigit(SlotType type)
    {
        return type == SlotType.Tens
            ? targetTens
            : targetUnits;
    }

    private void CheckPuzzleCompletion()
    {
        foreach (DeskSlot slot in slots)
        {
            int requiredDigit = GetRequiredDigit(slot.slotType);

            if (slot.currentTotal != requiredDigit)
                return;
        }

        if (solved)
            return;

        solved = true;

        Debug.Log("TOWN RECORDS PUZZLE SOLVED!");

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

    public bool IsSolved()
    {
        return solved;
    }
}
