using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using DG.Tweening.Core.Easing;

public class UIManager : MonoBehaviour
{
    [Header("Weapon Borders")]
    [SerializeField] private Image swordBorder;
    [SerializeField] private Image bowBorder;
    [SerializeField] private GameObject bowUI;
    [SerializeField] private GameObject swordUI;

    [Header("Stamina and Life Player")]
    [SerializeField] private Image[] staminaFills; 
    [SerializeField] private Image[] lifeFills;
    [SerializeField] private RectTransform lifeBar;
    [SerializeField] private RectTransform staminaBar;

    [Header("References")]
    [SerializeField] private LifeManager lifeManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private NPCData dialoguesHenry;
    [SerializeField] private NPCData dialoguesHerrera;
    [SerializeField] private PlayerData dataPlayer;

    [Header("Npc UI")]
    [SerializeField] private RectTransform[] dialoguePanels;
    [SerializeField] private TMP_Text[] dialogueTexts; 
    [SerializeField] private TMP_Text[] npcNameTexts;
    [SerializeField] private Image[] npcImages;

    [Header("Dotween")]
    [SerializeField] Ease easeAnimation;
    [SerializeField] private float duration;
    [SerializeField] private Vector3[] targetPositions;
    private Vector3[] originalPositions;
    private void Start()
    {
        StartCoroutine(MoveAtributesPlayer());

        originalPositions = new Vector3[dialoguePanels.Length];
        for (int i = 0; i < dialoguePanels.Length; i++)
        {
            originalPositions[i] = dialoguePanels[i].anchoredPosition;
        }
        ResetWeaponUI();
    }

    private void ResetWeaponUI()
    {
        bowUI.gameObject.SetActive(false);
        swordUI.gameObject.SetActive(false);
        swordBorder.gameObject.SetActive(false);
        bowBorder.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        lifeManager.OnPlayerDamage += UpdateLifeBar;
        InventoryPlayer.OnWeaponChanged += UpdateWeaponBorder;
        PlayerController.OnBowCollected += ActivateBowUI;
        PlayerController.OnSwordCollected += ActivateSwordUI;
        HerreraController.OnPlayerEnter += ShowDialogueNpc;
        HenryController.OnPlayerEnter += ShowDialogueNpc;
    }

    private void OnDisable()
    {
        lifeManager.OnPlayerDamage -= UpdateLifeBar;
        InventoryPlayer.OnWeaponChanged -= UpdateWeaponBorder;
        PlayerController.OnBowCollected -= ActivateBowUI;
        PlayerController.OnSwordCollected -= ActivateSwordUI;
        HerreraController.OnPlayerEnter -= ShowDialogueNpc;
        HenryController.OnPlayerEnter -= ShowDialogueNpc;
    }
    private void ActivateBowUI()
    {
        bowUI.gameObject.SetActive(true); 
    }
    private void ActivateSwordUI()
    {
        swordUI.gameObject.SetActive(true);
        swordBorder.gameObject.SetActive(true);
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
    public void UpdateLifeBar(int hitsReceived)
    {
        if (hitsReceived > 0 && hitsReceived <= lifeFills.Length)
        {
            Image currentFill = lifeFills[hitsReceived - 1];
            StartCoroutine(FadeOutLifeBar(currentFill));

            if (hitsReceived == lifeFills.Length)
            {
                Debug.Log("Sin vida");
            }
        }
    }
    public void UpdateStaminaBar()
    {
        if (staminaFills != null && dataPlayer != null)
        {
            float maxStaminaPerBar = 50f;
            float currentStamina = dataPlayer.Stamina;

            if (currentStamina > maxStaminaPerBar)
            {
                staminaFills[1].fillAmount = 1f;
            }
            else
            {
                staminaFills[1].fillAmount = currentStamina / maxStaminaPerBar;
            }

            if (currentStamina > maxStaminaPerBar)
            {
                float excessStamina = currentStamina - maxStaminaPerBar;
                staminaFills[0].fillAmount = excessStamina / maxStaminaPerBar;
            }
            else
            {
                staminaFills[0].fillAmount = 0f;
            }
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
    private void ShowDialogueNpc(bool isPlayerInRange, string npcName)
    {
        int npcIndex = -1;
        NPCData dialoguesToShow = null;

        if (npcName == "Herrera")
        {
            npcIndex = 0;
            dialoguesToShow = dialoguesHerrera;
        }
        else if (npcName == "Henry")
        {
            npcIndex = 1;
            dialoguesToShow = dialoguesHenry;
        }

        if (npcIndex != -1 && dialoguesToShow != null)
        {
            if (isPlayerInRange)
            {
                DisplayDialogueNPC(npcIndex, dialoguesToShow);
                dialoguePanels[npcIndex].DOAnchorPos(targetPositions[npcIndex], duration).SetEase(easeAnimation);
            }
            else
            {
                dialoguePanels[npcIndex].DOAnchorPos(originalPositions[npcIndex], duration).SetEase(easeAnimation);
            }
        }
    }
    private IEnumerator MoveAtributesPlayer()
    {
        yield return new WaitForSeconds(1f);
        lifeBar.DOAnchorPos(targetPositions[2], duration).SetEase(Ease.InOutQuad);
        staminaBar.DOAnchorPos(targetPositions[3], duration).SetEase(Ease.InBounce);
    }
    private void DisplayDialogueNPC(int npcIndex, NPCData dialoguesData)
    {
        npcNameTexts[npcIndex].text = dialoguesData.nameCharacter;
        dialogueTexts[npcIndex].text = dialoguesData.dialogue;
        npcImages[npcIndex].sprite = dialoguesData.imageCharacter;
    }
}
