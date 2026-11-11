using System.Collections;
using UnityEngine;

public class GearWheelRotation : MonoBehaviour
{
    public enum RotationDirection
    {
        Clockwise,
        CounterClockwise
    }

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private RotationDirection direction =
        RotationDirection.Clockwise;

    [SerializeField] private bool rotateOnEnable = true;

    private bool isRotating;

    private Coroutine coroutine;

    private void OnEnable()
    {
        isRotating = rotateOnEnable;
    }

    public void RotateOnCall()
    {
        isRotating = true;

        if(coroutine!=null) StopCoroutine(coroutine);

        coroutine = StartCoroutine(StopRotating());
    }

    private IEnumerator StopRotating()
    {
        yield return new WaitForSeconds(1);
        isRotating = false;
        coroutine = null;
    }

    private void Update()
    {
        if (!isRotating) return;

        float rotation = direction == RotationDirection.Clockwise
            ? -1f
            : 1f;

        transform.Rotate(
            0f,
            0f,
            rotation * rotationSpeed * Time.deltaTime
        );
    }

    public void StartRotation()
    {
        isRotating = true;
    }

    public void StopRotation()
    {
        isRotating = false;
    }
}
