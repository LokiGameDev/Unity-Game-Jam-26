
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static Controls;

[CreateAssetMenu(fileName = "PlayerInputReader", menuName = "Input/ Player Input Reader")]
public class InputReader : ScriptableObject, IPlayerActions
{
    private Controls controls;

    // Input events
    public event Action<Vector2> MoveEvent;
    public event Action JumpEvent;
    public event Action<bool> SprintEvent;
    public event Action InteractEvent;

    private void OnEnable()
    {
        if (controls == null)
        {
            controls = new Controls();
            controls.Player.SetCallbacks(this);
        }

        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls?.Player.Disable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveEvent?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
            JumpEvent?.Invoke();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.started)
            SprintEvent?.Invoke(true);

        if (context.canceled)
            SprintEvent?.Invoke(false);
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            InteractEvent?.Invoke();
        }
    }
}
