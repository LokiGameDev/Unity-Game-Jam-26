
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GearPuttingArea : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform dragArea;

    [SerializeField] private RectTransform[] pivotPoints;
    [SerializeField] private GearBalancePuzzle puzzle;

    [Header("Snap Settings")]
    [SerializeField] private float snapDistance = 60f;

    [Header("Events")]
    public UnityEvent onGearSnapped;
    public UnityEvent onGearReleased;

    // Keeps track of which gear occupies which pivot.
    private Dictionary<RectTransform, DraggableUIItem>
        occupiedPivots = new Dictionary<RectTransform, DraggableUIItem>();

    public void TrySnapGear(DraggableUIItem gear)
    {
        if (gear == null || dragArea == null)
            return;

        RectTransform gearRect = gear.GetComponent<RectTransform>();

        RectTransform nearestPivot = null;
        float nearestDistance = float.MaxValue;

        // Convert gear position into drag area local coordinates.
        Vector2 gearPosition =
            dragArea.InverseTransformPoint(gearRect.position);

        foreach (RectTransform pivot in pivotPoints)
        {
            if (pivot == null)
                continue;

            // Don't allow an already occupied pivot.
            if (occupiedPivots.ContainsKey(pivot))
                continue;

            Vector2 pivotPosition =
                dragArea.InverseTransformPoint(pivot.position);

            float distance =
                Vector2.Distance(gearPosition, pivotPosition);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestPivot = pivot;
            }
        }

        // Check if the nearest pivot is close enough.
        if (nearestPivot != null &&
            nearestDistance <= snapDistance)
        {
            SnapGear(gear, nearestPivot);
        }
        else
        {
            Debug.Log("No pivot found nearby.");
            onGearReleased?.Invoke();
        }
    }

    private void SnapGear(
        DraggableUIItem gear,
        RectTransform pivot)
    {
        RectTransform gearRect = gear.GetComponent<RectTransform>();

        // Convert pivot's world position into the drag area's local space.
        Vector3 localPosition =
            dragArea.InverseTransformPoint(pivot.position);

        gear.OnSnapped();

        gearRect.localPosition = localPosition;

        // Reserve this pivot.
        occupiedPivots[pivot] = gear;

        puzzle.AddGear(gear.gameObject.GetComponent<Gear>().GearValue);

        Debug.Log($"Gear snapped to {pivot.name}");

        onGearSnapped?.Invoke();
    }

    public void ReleaseGear(DraggableUIItem gear)
    {
        RectTransform occupiedPivot = null;

        foreach (var pair in occupiedPivots)
        {
            if (pair.Value == gear)
            {
                occupiedPivot = pair.Key;
                break;
            }
        }

        if (occupiedPivot != null)
        {
            occupiedPivots.Remove(occupiedPivot);

            Debug.Log($"Released pivot: {occupiedPivot.name}");

            puzzle.RemoveGear(gear.gameObject.GetComponent<Gear>().GearValue);

            gear.OnDesnapped();
        }
    }

    public bool IsPivotOccupied(RectTransform pivot)
    {
        return occupiedPivots.ContainsKey(pivot);
    }
}
