using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public static event Action<Vector2> OnMovementInput;
    public static event Action OnJumpInput;
    public static event Action OnAttackInput;
    public static event Action<bool> OnRunningInput;
    public static event Action<bool> OnCoverInput;
    public static event Action<bool> OnRollingInput;

    [SerializeField] private PlayerInput playerInput;

    private void OnEnable()
    {
        playerInput.actions["Movimiento"].performed += HandleMovement;
        playerInput.actions["Movimiento"].canceled += HandleMovement;
        playerInput.actions["Jump"].performed += HandleJump;
        playerInput.actions["Attack"].performed += HandleAttack;
        playerInput.actions["Running"].performed += HandleRunning;
        playerInput.actions["Running"].canceled += HandleRunning;
        playerInput.actions["Cover"].performed += HandleCover;
        playerInput.actions["Cover"].canceled += HandleCover;
        playerInput.actions["Rolling"].performed += HandleRolling;
    }

    private void OnDisable()
    {
        playerInput.actions["Movimiento"].performed -= HandleMovement;
        playerInput.actions["Movimiento"].canceled -= HandleMovement;
        playerInput.actions["Jump"].performed -= HandleJump;
        playerInput.actions["Attack"].performed -= HandleAttack;
        playerInput.actions["Running"].performed -= HandleRunning;
        playerInput.actions["Running"].canceled -= HandleRunning;
        playerInput.actions["Cover"].performed -= HandleCover;
        playerInput.actions["Cover"].canceled -= HandleCover;
        playerInput.actions["Rolling"].performed -= HandleRolling;
    }

    public void HandleMovement(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 movementInput = context.ReadValue<Vector2>();
            OnMovementInput?.Invoke(movementInput);
        }
        else if (context.canceled)
        {
            OnMovementInput?.Invoke(Vector2.zero);
        }
    }
    public void HandleJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnJumpInput?.Invoke();
        }
    }
    public void HandleAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnAttackInput?.Invoke();
        }
    }
    public void HandleRunning(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnRunningInput?.Invoke(true);
        }
        else if (context.canceled)
        {
            OnRunningInput?.Invoke(false);
        }
    }
    public void HandleCover(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnCoverInput?.Invoke(true);
        }
        else if (context.canceled)
        {
            OnCoverInput?.Invoke(false);
        }
    }
    public void HandleRolling(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnRollingInput?.Invoke(true);
        }
        else if (context.canceled)
        {
            OnRollingInput?.Invoke(false);
        }
    }
}
