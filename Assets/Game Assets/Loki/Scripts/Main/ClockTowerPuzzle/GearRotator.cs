using System.Collections;
using UnityEngine;

public class GearRotator : MonoBehaviour
{
    [SerializeField] private GearWheelRotation[] hourGears;
    [SerializeField] private GearWheelRotation[] minuteGears;

    private int prevHour = 0;
    private int prevMinute = 0;

    private Coroutine coroutine;
    private AudioSource rotatingSoundEffect;

    private void OnDisable()
    {
        if(rotatingSoundEffect!=null) rotatingSoundEffect.Stop();
    }

    public void RotateHourGears()
    {
        foreach(var gear in hourGears)
        {
            gear.RotateOnCall();
        }

        if(rotatingSoundEffect!=null) rotatingSoundEffect.Stop();

        rotatingSoundEffect = AudioManager.Instance.PlayLoopSFX(AudioID.GearRotation);

        if(coroutine!=null) StopCoroutine(coroutine);

        coroutine = StartCoroutine(StopRotating());
    }

    public void RotateMinuteGears()
    {
        foreach(var gear in minuteGears)
        {
            gear.RotateOnCall();
        }

        if(rotatingSoundEffect!=null) rotatingSoundEffect.Stop();

        rotatingSoundEffect = AudioManager.Instance.PlayLoopSFX(AudioID.GearRotation);

        if(coroutine!=null) StopCoroutine(coroutine);

        coroutine = StartCoroutine(StopRotating());
    }

    private IEnumerator StopRotating()
    {
        yield return new WaitForSeconds(1);
        rotatingSoundEffect.Stop();
        coroutine = null;
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

        rotatingSoundEffect = AudioManager.Instance.PlayLoopSFX(AudioID.GearRotation);
    }
}
