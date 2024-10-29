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
   

    public void HandleMovement(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Moviendose");
            Vector2 movementInput = context.ReadValue<Vector2>();
            OnMovementInput?.Invoke(movementInput);
        }
        else if (context.canceled)
        {
            Debug.Log("Ya no se mueve");
            OnMovementInput?.Invoke(Vector2.zero);
        }
    }
    public void HandleJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Saltando");
            OnJumpInput?.Invoke();
        }
    }
    public void HandleAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Atacando");
            OnAttackInput?.Invoke();
        }
    }
    public void HandleRunning(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Corriendo");
            OnRunningInput?.Invoke(true);
        }
        else if (context.canceled)
        {
            Debug.Log("Ya no corre");
            OnRunningInput?.Invoke(false);
        }
    }
    public void HandleCover(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Cubriendose");
            OnCoverInput?.Invoke(true);
        }
        else if (context.canceled)
        {
            Debug.Log("Ya no se cubre");
            OnCoverInput?.Invoke(false);
        }
    }
    public void HandleRolling(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Rolleando");
            OnRollingInput?.Invoke(true);
        }
        else if (context.canceled)
        {
            Debug.Log("Ya no rollea");
            OnRollingInput?.Invoke(false);
        }
    }
}
