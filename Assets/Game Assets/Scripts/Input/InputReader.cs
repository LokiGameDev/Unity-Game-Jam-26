using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static Controls;

[CreateAssetMenu(fileName = "Player Input Reader", menuName = "Input/Player Input Reader")]
/// <summary>Reads player input actions and exposes them as game events.</summary>
public class InputReader : ScriptableObject, IPlayerActions
{
    private Controls controls;

    /// <summary>Creates and enables the generated input actions.</summary>
    private void OnEnable()
    {
        if(controls == null)
        {
            controls = new Controls();
            controls.Player.SetCallbacks(this);
        }
        controls.Player.Enable();
    }
    /// <summary>Disables the input actions when the asset is unloaded.</summary>
    private void OnDisable()
    {
        controls.Player.Disable();
    }

    public event Action<Vector2> PlayerMovementEvent;
    public event Action OnPlayerJump;
    public Vector2 movementValue;

    /// <summary>Publishes movement input and stores its current value.</summary>
    public void OnMove(InputAction.CallbackContext context)
    {
        PlayerMovementEvent?.Invoke(context.ReadValue<Vector2>());
        movementValue = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if(context.performed) OnPlayerJump?.Invoke();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }
}