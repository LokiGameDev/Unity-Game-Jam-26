using UnityEngine;

public class GearRotator : MonoBehaviour
{
    [SerializeField] private GearWheelRotation[] hourGears;
    [SerializeField] private GearWheelRotation[] minuteGears;

    private int prevHour = 0;
    private int prevMinute = 0;

    public void RotateHourGears()
    {
        foreach(var gear in hourGears)
        {
            gear.RotateOnCall();
        }
    }

    public void RotateMinuteGears()
    {
        foreach(var gear in minuteGears)
        {
            gear.RotateOnCall();
        }
    }

    public void TimevalueChanged(int hour, int minute)
    {
        if(hour != prevHour)
        {
            RotateHourGears();
            prevHour = hour;
        }

        if(minute != prevMinute)
        {
            RotateMinuteGears();
            prevMinute = minute;
        }
    }

    public void RotateAll()
    {
        foreach(var gear in hourGears)
        {
            gear.RotateInfinite();
        }
        foreach(var gear in minuteGears)
        {
            gear.RotateInfinite();
        }
    }
}
