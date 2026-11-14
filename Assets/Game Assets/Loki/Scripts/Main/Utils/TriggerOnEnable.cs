using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TriggerOnEnable : MonoBehaviour
{
    [SerializeField] private float delayBeforeTrigger = 0;
    public UnityEvent triggerEvent;

    public void OnEnable()
    {
        StartCoroutine(TriggerTheEvent());
    }

    private IEnumerator TriggerTheEvent()
    {
        yield return new WaitForSeconds(delayBeforeTrigger);
        triggerEvent?.Invoke();
    }
}
