using UnityEngine;
using UnityEngine.InputSystem;

public class player_interact : MonoBehaviour
{
    private IInteractable inRange = null;
    public GameObject InteractionIcon;

    void Start()
    {
        InteractionIcon.SetActive(false);
    }
    void Update()
    {
        if (inRange != null && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            inRange.Interact(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable))
        {
            inRange = interactable;
            InteractionIcon.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable) && interactable == inRange)
        {
            inRange = null;
            InteractionIcon.SetActive(false);
        }
    }
}