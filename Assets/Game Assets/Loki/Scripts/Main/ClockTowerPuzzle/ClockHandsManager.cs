using System;
using UnityEngine;

public class ClockController : MonoBehaviour
{
    [Header("Clock Hands")]
    [SerializeField] private GameObject hourHand;
    [SerializeField] private GameObject minuteHand;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 180f;

    private Quaternion targetHourRotation;
    private Quaternion targetMinuteRotation;

    private void Awake()
    {
        targetHourRotation = hourHand.transform.localRotation;
        targetMinuteRotation = minuteHand.transform.localRotation;
    }

    private void Update()
    {
        hourHand.transform.localRotation = Quaternion.RotateTowards(
            hourHand.transform.localRotation,
            targetHourRotation,
            rotationSpeed * Time.deltaTime
        );

        minuteHand.transform.localRotation = Quaternion.RotateTowards(
            minuteHand.transform.localRotation,
            targetMinuteRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    public void TimeGotChanged(int hour, int minute)
    {
        hour %= 12;

        float minuteAngle = minute * 6f;
        float hourAngle = (hour * 30f) + (minute * 0.5f);

        targetMinuteRotation = Quaternion.Euler(0f, 0f, -minuteAngle);
        targetHourRotation = Quaternion.Euler(0f, 0f, -hourAngle);
    }
}