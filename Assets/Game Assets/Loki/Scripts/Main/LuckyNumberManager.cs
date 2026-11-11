
using UnityEngine;

public class LuckyNumberManager : MonoBehaviour
{
    public static LuckyNumberManager Instance { get; private set; }

    public int LuckyNumber { get; private set; }
    public bool HasLuckyNumber { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SetLuckyNumber(7);
    }

    public void SetLuckyNumber(int number)
    {
        if (number < 0 || number > 720)
        {
            Debug.LogWarning("Lucky number must be between 1 and 99.");
            return;
        }

        LuckyNumber = number;
        HasLuckyNumber = true;

        Debug.Log("Lucky Number selected: " + LuckyNumber);
    }

    public void GetDigits(out int hours, out int minutes)
    {
        hours = LuckyNumber / 10;
        minutes = LuckyNumber % 10;
    }
}
