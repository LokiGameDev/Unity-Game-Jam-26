
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RecordPuzzleMaker : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject recordPrefab;
    [SerializeField] private RectTransform dragArea;
    [SerializeField] private RecordDeskArea recordDeskArea;

    [Header("Spawn Area")]
    [SerializeField] private RectTransform spawnArea;
    [SerializeField] private Vector2 spawnPadding = new Vector2(40, 40);

    [Header("Generation Settings")]
    [SerializeField] private int minSolutionRecords = 5;
    [SerializeField] private int maxSolutionRecords = 6;
    [SerializeField] private int extraRecords = 5;

    private readonly List<GameObject> spawnedRecords = new();

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
        if (recordPrefab == null || spawnArea == null)
        {
            Debug.LogError("Record prefab or spawn area is missing!");
            return;
        }

        if (luckyNumber < 5 || luckyNumber > 719)
        {
            Debug.LogError(
                "Record puzzle requires a lucky number between 5 and 719."
            );
            return;
        }

        ClearPuzzle();

        targetNumber = luckyNumber;

        int minCount = Mathf.Clamp(
            minSolutionRecords, 1, luckyNumber
        );

        int maxCount = Mathf.Clamp(
            maxSolutionRecords, minCount, luckyNumber
        );

        int requiredCount = Random.Range(
            minCount, maxCount + 1
        );

        List<int> solution = GenerateSolution(
            luckyNumber,
            requiredCount
        );

        List<int> candidates = new List<int>(solution);

        for (int i = 0; i < extraRecords; i++)
        {
            candidates.Add(Random.Range(1, luckyNumber + 1));
        }

        Shuffle(candidates);

        foreach (int value in candidates)
        {
            SpawnRecord(value);
        }

        Debug.Log(
            $"Puzzle generated! Target: {targetNumber}, " +
            $"Required records: {requiredCount}, " +
            $"Total records: {candidates.Count}"
        );
    }
    
    private List<int> GenerateSolution(int total, int count)
    {
        // Front = Units (x1)
        // Back  = Tens  (x10)

        int maxBackCount = Mathf.Min(
            count,
            (total - count) / 9
        );

        // If the number isn't divisible by 10,
        // all records cannot be placed in the Tens slot.
        if (total % 10 != 0)
        {
            maxBackCount = Mathf.Min(
                maxBackCount,
                count - 1
            );
        }

        int backCount = Random.Range(
            0, maxBackCount + 1
        );

        int frontCount = count - backCount;

        int backUnits;

        if (frontCount == 0)
        {
            // All records go into Tens.
            backUnits = total / 10;
        }
        else
        {
            int minimumBackUnits = backCount;

            int maximumBackUnits =
                (total - frontCount) / 10;

            backUnits = backCount > 0
                ? Random.Range(
                    minimumBackUnits,
                    maximumBackUnits + 1
                )
                : 0;
        }

        int frontTotal = total - (backUnits * 10);

        List<int> frontSolution = new List<int>();
        List<int> backSolution = new List<int>();

        if (frontCount > 0)
        {
            frontSolution = SplitIntoPositiveParts(
                frontTotal,
                frontCount
            );
        }

        if (backCount > 0)
        {
            backSolution = SplitIntoPositiveParts(
                backUnits,
                backCount
            );
        }

        // Debug the solution before mixing the records.
        Debug.Log(
            $"Units Solution: {string.Join(" + ", frontSolution)}"
        );

        Debug.Log(
            $"Tens Solution: {string.Join(" + ", backSolution)}"
        );

        int calculatedTotal =
            frontSolution.Sum() + backSolution.Sum() * 10;

        Debug.Log(
            $"Generated Solution Total: {calculatedTotal}"
        );

        List<int> solution = new List<int>();

        solution.AddRange(frontSolution);
        solution.AddRange(backSolution);

        return solution;
    }


    private List<int> SplitIntoPositiveParts(int total, int count)
    {
        List<int> result = new List<int>();

        int remaining = total;

        for (int i = 0; i < count - 1; i++)
        {
            int maxValue = remaining - (count - i - 1);

            int value = Random.Range(1, maxValue + 1);

            result.Add(value);
            remaining -= value;
        }

        result.Add(remaining);

        Shuffle(result);

        return result;
    }

    private void SpawnRecord(int value)
    {
        GameObject instance = Instantiate(
            recordPrefab,
            spawnArea,
            false
        );

        RectTransform rect = instance.GetComponent<RectTransform>();

        if (rect == null)
        {
            Debug.LogError("Record prefab needs a RectTransform!");
            Destroy(instance);
            return;
        }

        rect.anchoredPosition = GetRandomPosition();

        DraggableRecord piece =
            instance.GetComponentInChildren<DraggableRecord>(true);

        if (piece == null)
        {
            Debug.LogError("Record prefab is missing PuzzlePieceValue!");
            Destroy(instance);
            return;
        }

        piece.Initialize(value, dragArea, recordDeskArea);

        spawnedRecords.Add(instance);
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

    public int CalculateTotal(
        int frontStackValue,
        int backStackValue)
    {
        return frontStackValue + (backStackValue * 10);
    }

    public bool IsSolved(
        int frontStackValue,
        int backStackValue)
    {
        return CalculateTotal(
            frontStackValue,
            backStackValue
        ) == targetNumber;
    }

    public void ClearPuzzle()
    {
        foreach (GameObject record in spawnedRecords)
        {
            if (record != null)
                Destroy(record);
        }

        spawnedRecords.Clear();
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
