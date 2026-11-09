
using UnityEngine;

public class SkyMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 0.2f;
    [SerializeField] private Vector2 moveDirection = Vector2.left;

    [Header("Texture")]
    [SerializeField] private Renderer skyRenderer;

    private Vector2 offset;

    private void Awake()
    {
        if (skyRenderer == null)
            skyRenderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        offset += moveDirection.normalized * moveSpeed * Time.deltaTime;

        skyRenderer.material.mainTextureOffset = offset;
    }
}
