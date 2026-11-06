
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InputReader inputReader;

    [Header("Interaction")]
    [SerializeField] private Transform interactionPoint;
    [SerializeField] private float interactionRadius = 1.2f;
    [SerializeField] private LayerMask interactableLayers;

    private IInteractable currentInteractable;

    private void OnEnable()
    {
        if (inputReader != null)
            inputReader.InteractEvent += HandleInteractInput;
    }

    private void OnDisable()
    {
        if (inputReader != null)
            inputReader.InteractEvent -= HandleInteractInput;
    }

    private void Update()
    {
        FindInteractable();
    }

    private void FindInteractable()
    {
        currentInteractable = null;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            interactionPoint.position,
            interactionRadius,
            interactableLayers
        );

        float closestDistance = float.MaxValue;

        foreach (Collider2D hit in hits)
        {
            IInteractable interactable =
                hit.GetComponentInParent<IInteractable>();

            if (interactable == null)
                continue;

            float distance = Vector2.Distance(
                interactionPoint.position,
                hit.ClosestPoint(interactionPoint.position)
            );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                currentInteractable = interactable;
            }
        }
    }

    private void HandleInteractInput()
    {
        if (currentInteractable == null)
            return;

        currentInteractable.Interact(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        if (interactionPoint == null)
            return;

        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            interactionPoint.position,
            interactionRadius
        );
    }
}
