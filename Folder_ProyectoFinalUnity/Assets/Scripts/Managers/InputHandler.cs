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
        if (!canHandleInputJump) return;

        if (context.performed)
        {
            OnJumpInput?.Invoke();
        }
    }
    public void HandleAttackSword(InputAction.CallbackContext context)
    {
        if (context.performed && CanPerformAttackSword())
        {
            OnAttackSwordInput?.Invoke();
        }

    }
    public void HandleAttackBow(InputAction.CallbackContext context)
    {
        if (CanPerformAttackBow())
        {
            if (context.started)
            {
                OnAttackBow?.Invoke(true);
            }
            else if (context.canceled)
            {
                OnAttackBow?.Invoke(false);
            }
        }
    }

    public void HandleRunning(InputAction.CallbackContext context)
    {
        if (context.started)
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
        if (CanPerformCover())
        {
            if (context.started)
            {
                OnCoverInput?.Invoke(true);
            }
            else if (context.canceled)
            {
                OnCoverInput?.Invoke(false);
            }
        }
    }
    public void HandleRolling(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
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
