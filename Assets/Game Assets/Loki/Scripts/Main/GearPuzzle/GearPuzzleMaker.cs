
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GearPuzzleMaker : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject gearPrefab;

    [SerializeField] private RectTransform dragArea;
    [SerializeField] private GearPuttingArea gearPuttingArea;

    [Header("Spawn Area")]
    [SerializeField] private RectTransform spawnArea;
    [SerializeField] private Vector2 spawnPadding = new Vector2(40, 40);

    [Header("Generation Settings")]
    [SerializeField] private int minSolutionGears = 5;
    [SerializeField] private int maxSolutionGears = 6;
    [SerializeField] private int extraGears = 5;

    private readonly List<GameObject> spawnedGears = new();

    private int targetNumber;

    public int TargetNumber => targetNumber;

    public void Start()
    {
        LuckyNumberManager.Instance.LuckyNumberSelected.AddListener(SelectedLucky);
    }

    private void SelectedLucky()
    {
        GeneratePuzzle(LuckyNumberManager.Instance.LuckyNumber);
    }

    public void GeneratePuzzle(int luckyNumber)
    {
        if (gearPrefab == null || spawnArea == null)
        {
            Debug.LogError("Gear prefab or spawn area is missing!");
            return;
        }

        if (luckyNumber < 5 || luckyNumber > 719)
        {
            Debug.LogError(
                "Gear puzzle requires a lucky number between 5 and 719."
            );
            return;
        }

        ClearPuzzle();

        targetNumber = luckyNumber;

        int minCount = Mathf.Max(1, minSolutionGears);
        int maxCount = Mathf.Max(minCount, maxSolutionGears);

        // A positive integer is needed for every solution gear.
        maxCount = Mathf.Min(maxCount, luckyNumber);

        int requiredCount = Random.Range(minCount, maxCount + 1);

        // Generate the actual solution values.
        List<int> solution = SplitIntoPositiveParts(
            luckyNumber,
            requiredCount
        );

        // Add five random distractor gears.
        List<int> candidates = new List<int>(solution);

        for (int i = 0; i < extraGears; i++)
        {
            candidates.Add(Random.Range(1, luckyNumber + 1));
        }

        Shuffle(candidates);

        foreach (int value in candidates)
        {
            SpawnGear(value);
        }

        Debug.Log(
            $"Gear puzzle generated! Target: {targetNumber}, " +
            $"Required gears: {requiredCount}, " +
            $"Total gears: {candidates.Count}"
        );

        Debug.Log(
            "Solution: " + string.Join(" + ", solution)
        );
    }

    private List<int> SplitIntoPositiveParts(int total, int count)
    {
        List<int> result = new List<int>();

        int remaining = total;

        for (int i = 0; i < count - 1; i++)
        {
            // Reserve at least 1 for every remaining gear.
            int maxValue = remaining - (count - i - 1);

            int value = Random.Range(1, maxValue + 1);

            result.Add(value);
            remaining -= value;
        }

        result.Add(remaining);

        Shuffle(result);

        return result;
    }

    private void SpawnGear(int value)
    {
        GameObject instance = Instantiate(
            gearPrefab,
            spawnArea,
            false
        );

        RectTransform rect = instance.GetComponent<RectTransform>();

        if (rect == null)
        {
            Debug.LogError("Gear prefab needs a RectTransform!");
            Destroy(instance);
            return;
        }

        rect.anchoredPosition = GetRandomPosition();

        Gear piece =
            instance.GetComponentInChildren<Gear>(true);

        DraggableUIItem item = instance.GetComponentInChildren<DraggableUIItem>();

        if (piece == null || item == null)
        {
            Debug.LogError("Gear prefab is missing Gear!");
            Destroy(instance);
            return;
        }

        piece.Initialize(value);
        item.Initialize(dragArea, gearPuttingArea);

        spawnedGears.Add(instance);
    }

    private Vector2 GetRandomPosition()
    {
        float halfWidth = Mathf.Max(
            0, spawnArea.rect.width * 0.5f - spawnPadding.x
        );

        float halfHeight = Mathf.Max(
            0, spawnArea.rect.height * 0.5f - spawnPadding.y
        );

        return new Vector2(
            Random.Range(-halfWidth, halfWidth),
            Random.Range(-halfHeight, halfHeight)
        );
    }

    public bool IsSolved(List<Gear> placedGears)
    {
        if (placedGears == null ||
            placedGears.Count < 1 ||
            placedGears.Count > maxSolutionGears)
            return false;

        int total = 0;

        foreach (Gear gear in placedGears)
        {
            if (gear == null)
                return false;

            total += gear.GearValue;
        }

        return total == targetNumber;
    }

    public void ClearPuzzle()
    {
        foreach (GameObject gear in spawnedGears)
        {
            if (gear != null)
                Destroy(gear);
        }

        spawnedGears.Clear();
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);

            (list[i], list[randomIndex]) =
                (list[randomIndex], list[i]);
        }
    }
}
