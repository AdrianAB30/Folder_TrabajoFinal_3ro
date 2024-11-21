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
    public bool canHandleInputJump = true;

    [Header("Referencias")]
    [SerializeField] private InventoryPlayer inventoryPlayer;
    [SerializeField] private PlayerController playerController;

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
        if (!canHandleInputJump) return;

        if (context.performed)
        {
            Debug.Log("Saltando");
            OnJumpInput?.Invoke();
        }
    }
    public void HandleAttackSword(InputAction.CallbackContext context)
    {
        if (context.performed && CanPerformAttackSword())
        {
            Debug.Log("Atacando con espada");
            OnAttackSwordInput?.Invoke();
        }

    }
    public void HandleAttackBow(InputAction.CallbackContext context)
    {
        if (CanPerformAttackBow())
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
        if (CanPerformCover())
        {
            if (context.started)
            {
                Debug.Log("Cubriéndose");
                OnCoverInput?.Invoke(true);
            }
            else if (context.canceled)
            {
                Debug.Log("Dejando de cubrirse");
                OnCoverInput?.Invoke(false);
            }
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
    private bool CanPerformAttackSword()
    {
        return inventoryPlayer.isSwordEquipped && inventoryPlayer.isShieldEquipped && !playerController.isRolling && !playerController.isJumping;
    }
    private bool CanPerformAttackBow()
    {
        return inventoryPlayer.isBowEquipped && !playerController.isRolling && !playerController.isJumping;
    }
    private bool CanPerformCover()
    {
        return inventoryPlayer.isShieldEquipped && !playerController.isRolling && !playerController.isJumping;
    }
}
