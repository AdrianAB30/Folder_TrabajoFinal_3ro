using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public event Action<Vector2> OnMovementInput;
    public event Action OnJumpInput;
    public event Action OnAttackSwordInput;
    public event Action OnRollingInput;
    public event Action<bool> OnAttackBow;
    public event Action<bool> OnRunningInput;
    public event Action<bool> OnCoverInput;

    public bool canHandleInput = true;

    public void HandleMovement(InputAction.CallbackContext context)
    {
        if (!canHandleInput) return;

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
        if (!canHandleInput) return;

        if (context.performed)
        {
            Debug.Log("Saltando");
            OnJumpInput?.Invoke();
        }
    }
    public void HandleAttackSword(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Atacando con espada");
            OnAttackSwordInput?.Invoke();
        }

    }
    public void HandleAttackBow(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("Tensando el Arco");
            OnAttackBow?.Invoke(true);
        }
        else if (context.canceled)
        {
            Debug.Log("Disparando Flecha");
            OnAttackBow?.Invoke(false); 
        }
    }

    public void HandleRunning(InputAction.CallbackContext context)
    {
        if (context.started)
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
        if (context.started)
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
            OnRollingInput?.Invoke();
        }
    }
}
