
using UnityEngine;
using UnityEngine.Events;

public class LuckyNumberManager : MonoBehaviour
{
    public static LuckyNumberManager Instance { get; private set; }

    public int LuckyNumber { get; private set; } = 7;
    public bool HasLuckyNumber { get; private set; } = false;

    public UnityEvent LuckyNumberSelected;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
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

        LuckyNumberSelected?.Invoke();

        GameManager.Instance.LuckyNumberSelected();
    }

    public void GetDigits(out int hours, out int minutes)
    {
        hours = LuckyNumber / 10;
        minutes = LuckyNumber % 10;
    }

    public void GetDigitsInTime(out int hours, out int minutes)
    {
        hours = LuckyNumber / 60;
        minutes = LuckyNumber % 60;
    }
}
