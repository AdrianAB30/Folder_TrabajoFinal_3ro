using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryPlayer : MonoBehaviour
{
    private DoubleLinkedList<GameObject> weaponsInventory = new DoubleLinkedList<GameObject>();

    [Header("Sword and Shield")]
    [SerializeField] private GameObject sword;
    [SerializeField] private GameObject shield;
    [SerializeField] private GameObject swordBack;
    [SerializeField] private GameObject shieldBack;
    [Header("Bow, Quiver and Arrow")]
    [SerializeField] private GameObject bow;
    [SerializeField] private GameObject bowHand;
    [SerializeField] private GameObject quiver;
    [SerializeField] private GameObject groundBow;
    [SerializeField] private GameObject groundQuiver;

    [SerializeField] private GameObject player;

    public Animator playerAnimator;
    private GameObject currentWeapon;
    private int currentWeaponIndex = 0;
    private bool isEquipping = false;

    private void Start()
    {
        weaponsInventory.AddEnd(sword);
        currentWeapon = sword;
        sword.SetActive(true);
        shield.SetActive(true);
        bowHand.SetActive(false);
        bow.SetActive(false);
        quiver.SetActive(false);
        swordBack.SetActive(false);
        shieldBack.SetActive(false);
        Debug.Log("Capacidad inicial del inventario: " + weaponsInventory.Length);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GroundBow"))
        {
            Debug.Log("Has recogido el arco del suelo.");
            AddWeapon(bow);
            groundBow.SetActive(false);
            groundQuiver.SetActive(false);
            EquipWeapon(bow); 
        }
    }
    public void AddWeapon(GameObject weapon)
    {
        weaponsInventory.AddEnd(weapon);
        Debug.Log("Arma añadida al inventario: " + weapon.name);
        Debug.Log("Capacidad actual del inventario: " + weaponsInventory.Length);
    }
    public void OnSwitchWeaponPrevious(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            currentWeaponIndex = (currentWeaponIndex - 1 + weaponsInventory.Length) % weaponsInventory.Length;
            EquipWeapon(weaponsInventory.GetAt(currentWeaponIndex));
        }
    }
    public void OnSwitchWeaponNext(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            currentWeaponIndex = (currentWeaponIndex + 1) % weaponsInventory.Length;
            EquipWeapon(weaponsInventory.GetAt(currentWeaponIndex));
        }
    }
    private void EquipWeapon(GameObject newWeapon)
    {
        if (isEquipping) return; 
        isEquipping = true;

        playerAnimator.SetBool("isEquipping", true);
        if (currentWeapon != null)
        {
            currentWeapon.SetActive(false);
        }
        currentWeapon = newWeapon;
        currentWeapon.SetActive(true);

        if (currentWeapon == sword)
        {
            shield.SetActive(true); 
            sword.SetActive(true);   
            bow.SetActive(true);  
            quiver.SetActive(true);
            swordBack.SetActive(false);
            shieldBack.SetActive(false);
            bowHand.SetActive(false);
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
        }
        playerAnimator.SetBool("hasSword", currentWeapon == sword);
        playerAnimator.SetBool("hasBow", currentWeapon == bow);

        Debug.Log("Se ha equipado: " + currentWeapon.name);
        StartCoroutine(StopEquippingAnimation());
    }
    private IEnumerator StopEquippingAnimation()
    {
        yield return new WaitForSeconds(0.5f);
        playerAnimator.SetBool("isEquipping", false);
        yield return new WaitForSeconds(2.5f);
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
    public GameObject GetBow()
    {
        return bow; 
    }
    public void ActivateQuiver()
    {
        if (quiver != null)
        {
            quiver.SetActive(true);
        }
    }
}
