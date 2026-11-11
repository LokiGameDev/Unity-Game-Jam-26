
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class StairZone2D : MonoBehaviour
{
    [Header("Stair Settings")]
    [SerializeField] private float stairSpeed = 3f;

    private Collider2D stairCollider;

    private void Awake()
    {
        stairCollider = GetComponent<Collider2D>();
        stairCollider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();

        if (player != null)
        {
            player.EnterStairZone(stairSpeed);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();

        if (player != null)
        {
            player.ExitStairZone();
        }
    }
}
