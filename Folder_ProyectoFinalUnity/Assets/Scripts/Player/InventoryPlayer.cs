using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InventoryPlayer : MonoBehaviour
{
    private DoubleCircularLinkedList<GameObject> weaponsInventory = new DoubleCircularLinkedList<GameObject>();

    [Header("Sword and Shield")]
    [SerializeField] private GameObject sword;
    [SerializeField] private GameObject shield;
    [SerializeField] private GameObject swordBack;
    [SerializeField] private GameObject shieldBack;
    [SerializeField] private GameObject swordGround;
    [SerializeField] private GameObject shieldGround;

    [Header("Bow, Quiver and Arrow")]
    [SerializeField] private GameObject bow;
    [SerializeField] private GameObject bowHand;
    [SerializeField] private GameObject quiver;
    [SerializeField] private GameObject groundBow;
    [SerializeField] private GameObject groundQuiver;

    [SerializeField] private PlayerController playerController;

    public bool isSwordEquipped = false;
    public bool isBowEquipped = false;
    public bool isShieldEquipped = false;

    // Eventos
    public static event Action<string> OnWeaponChanged;

    public Animator playerAnimator;
    private GameObject currentWeapon;
    private int currentWeaponIndex = 0;
    private bool isEquipping = false;

    private void Start()
    {
        ResetWeapons();
        Debug.Log("Capacidad inicial del inventario: " + weaponsInventory.Count);
    }
    private void ResetWeapons()
    {
        sword.SetActive(false);
        shield.SetActive(false);
        bowHand.SetActive(false);
        bow.SetActive(false);
        quiver.SetActive(false);
        swordBack.SetActive(false);
        shieldBack.SetActive(false);
    }

    public void AddWeapon(GameObject weapon)
    {
        weaponsInventory.InsertAtEnd(weapon);
        Debug.Log("Arma añadida al inventario: " + weapon.name);
        Debug.Log("Capacidad actual del inventario: " + weaponsInventory.Count);
    }

    public void OnSwitchWeaponPrevious(InputAction.CallbackContext context)
    {
        if (context.performed && weaponsInventory.Count > 0)
        {
            currentWeaponIndex = (currentWeaponIndex - 1 + weaponsInventory.Count) % weaponsInventory.Count;
            EquipWeapon(weaponsInventory.GetAtPosition(currentWeaponIndex));
        }
    }

    public void OnSwitchWeaponNext(InputAction.CallbackContext context)
    {
        if (context.performed && weaponsInventory.Count > 0)
        {
            currentWeaponIndex = (currentWeaponIndex + 1) % weaponsInventory.Count;
            EquipWeapon(weaponsInventory.GetAtPosition(currentWeaponIndex));
        }
    }

    private void EquipWeapon(GameObject newWeapon)
    {
        if (isEquipping || playerController == null)
        {
            return;
        }
        if (playerController.GetMovementMagnitude() > 0f)
        {
            return;
        }
        isEquipping = true;

        playerAnimator.SetBool("isEquipping", true);
        if (currentWeapon != null)
        {
            currentWeapon.SetActive(false);
        }
        currentWeapon = newWeapon;
        currentWeapon.SetActive(true);

        OnWeaponChanged?.Invoke(currentWeapon.name);

        if (currentWeapon == sword)
        {
            shield.SetActive(true);
            sword.SetActive(true);
            bow.SetActive(true);
            quiver.SetActive(true);
            swordBack.SetActive(false);
            shieldBack.SetActive(false);
            bowHand.SetActive(false);
            isSwordEquipped = true;
            isBowEquipped = false;
            isShieldEquipped = true;
        }
        else if (currentWeapon == bow)
        {
            shield.SetActive(false);
            sword.SetActive(false);
            bow.SetActive(false);
            bowHand.SetActive(true);
            quiver.SetActive(true);
            shieldBack.SetActive(true);
            swordBack.SetActive(true);
            isSwordEquipped = false;
            isBowEquipped = true;
            isShieldEquipped = false;
        }
        playerAnimator.SetBool("hasSword", currentWeapon == sword);
        playerAnimator.SetBool("hasBow", currentWeapon == bow);

        Debug.Log("Se ha equipado: " + currentWeapon.name);
    }

    public void OnEquipAnimationEnd()
    {
        playerAnimator.SetBool("isEquipping", false);
        isEquipping = false;
    }

    public void ActivateBow()
    {
        if (bow != null)
        {
            bow.SetActive(true);
            currentWeapon = bow;
        }
    }
    public void ActivateSword()
    {
        if (sword && shield != null)
        {
            swordBack.SetActive(true);
            shieldBack.SetActive(true);
            currentWeapon = sword;
        }
    }

    public GameObject GetBow()
    {
        return bow;
    }
    public GameObject GetSwordAndShield()
    {
        return sword;
    }
    public void ActivateQuiver()
    {
        if (quiver != null)
        {
            quiver.SetActive(true);
        }
    }
}
