using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Weapon Borders")]
    [SerializeField] private Image swordBorder;
    [SerializeField] private Image bowBorder;
    [SerializeField] private GameObject bowUI;

    [Header("Stamina and Life Player")]
    [SerializeField] private Image staminaBar1; 
    [SerializeField] private Image staminaBar2;
    [SerializeField] private Image lifeFill1;
    [SerializeField] private Image lifeFill2;
    [SerializeField] private Image lifeFill3;

    [Header("References")]
    [SerializeField] private LifeManager lifeManager;

    private void Start()
    {
        bowUI.gameObject.SetActive(false);
        swordBorder.gameObject.SetActive(true);
        bowBorder.gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        lifeManager.OnPlayerDamage += UpdateLifeBar;
        InventoryPlayer.OnWeaponChanged += UpdateWeaponBorder;
        PlayerController.OnBowCollected += ActivateBowUI;
    }

    private void OnDisable()
    {
        lifeManager.OnPlayerDamage -= UpdateLifeBar;
        InventoryPlayer.OnWeaponChanged -= UpdateWeaponBorder;
        PlayerController.OnBowCollected -= ActivateBowUI;
    }
    private void ActivateBowUI()
    {
        bowUI.gameObject.SetActive(true); 
    }

    private void UpdateWeaponBorder(string weaponName)
    {
        swordBorder.gameObject.SetActive(false);
        bowBorder.gameObject.SetActive(false);

        switch (weaponName)
        {
            case "sword":
                swordBorder.gameObject.SetActive(true);
                break;
            case "Arco":
                bowBorder.gameObject.SetActive(true);
                break;
            default:
                Debug.Log("Arma desconocida: " + weaponName);
                break;
        }
    }
    private void UpdateLifeBar(int hitsReceived)
    {
        Image currentFill = null;

        switch (hitsReceived)
        {
            case 1:
                currentFill = lifeFill1;
                break;
            case 2:
                currentFill = lifeFill2;
                break;
            case 3:
                currentFill = lifeFill3;
                Debug.Log("Sin vida");
                break;
        }

        if (currentFill != null)
        {
            StartCoroutine(FadeOutLifeBar(currentFill));
        }
    }
    private IEnumerator FadeOutLifeBar(Image image)
    {
        float duration = 0.5f;
        float elapsed = 0f;
        float startAmount = image.fillAmount;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            image.fillAmount = Mathf.Lerp(startAmount, 0, elapsed / duration);
            yield return null;
        }
        image.fillAmount = 0;
    }

}
