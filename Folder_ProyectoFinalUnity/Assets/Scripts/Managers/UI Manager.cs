using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;
using Cinemachine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Image swordBorder;
    [SerializeField] private Image bowBorder;
    [SerializeField] private GameObject bowUI;
    [SerializeField] private GameObject swordUI;
    [SerializeField] private TMP_Text arrowCountText;

    [Header("Stamina and Life Player")]
    [SerializeField] private Image[] staminaFills; 
    [SerializeField] private Image[] lifeFills;
    [SerializeField] private RectTransform lifeBar;
    [SerializeField] private RectTransform staminaBar;
    [SerializeField] private RectTransform timerArrows;
    [SerializeField] private Image timer;

    [Header("References")]
    [SerializeField] private LifeManager lifeManager;
    [SerializeField] private SpawnArrows countArrows;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private NPCData dialoguesHenry;
    [SerializeField] private NPCData dialoguesHerrera;
    [SerializeField] private PlayerData dataPlayer;
    [SerializeField] private PlayerController player;
    [SerializeField] private Stack<int> damagedLifeIndices = new Stack<int>();

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
    private Vector3 positionTimer = new Vector3(0,130,0);

    [Header("Life Regeneration Curve")]
    [SerializeField] private AnimationCurve lifeRegenerationCurve;
    private bool[] isLifeBarActive;
    private float[] regenerationTimers;

    private void Start()
    {
        StartCoroutine(MoveAtributesPlayer());
        originalPositions = new Vector3[dialoguePanels.Length];
        for (int i = 0; i < dialoguePanels.Length; i++)
        {
            originalPositions[i] = dialoguePanels[i].anchoredPosition;
        }

        isLifeBarActive = new bool[lifeFills.Length];
        regenerationTimers = new float[lifeFills.Length];
        for (int i = 0; i < lifeFills.Length; i++)
        {
            isLifeBarActive[i] = true;
            regenerationTimers[i] = 0f;  
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
        countArrows.OnArrowsEmpty += ShowTimerArrows;
        countArrows.OnArrowCountChanged += UpdateArrowCountUI;
        player.OnPlayerStand += ShowTutorial;
        lifeManager.OnPlayerDamage += UpdateLifeBar;
        InventoryPlayer.OnWeaponChanged += UpdateWeaponBorder;
        PlayerController.OnBowCollected += ActivateBowUI;
        PlayerController.OnSwordCollected += ActivateSwordUI;
        HerreraController.OnPlayerEnter += ShowDialogueNpc;
        HenryController.OnPlayerEnter += ShowDialogueNpc;
    }
    private void OnDisable()
    {
        countArrows.OnArrowsEmpty -= ShowTimerArrows;
        countArrows.OnArrowCountChanged -= UpdateArrowCountUI;
        player.OnPlayerStand -= ShowTutorial;
        lifeManager.OnPlayerDamage -= UpdateLifeBar;
        InventoryPlayer.OnWeaponChanged -= UpdateWeaponBorder;
        PlayerController.OnBowCollected -= ActivateBowUI;
        PlayerController.OnSwordCollected -= ActivateSwordUI;
        HerreraController.OnPlayerEnter -= ShowDialogueNpc;
        HenryController.OnPlayerEnter -= ShowDialogueNpc;
    }
    private void Update()
    {
        RegenerateLifeBar();
    }
    private void UpdateArrowCountUI(int currentArrows)
    {
        arrowCountText.text = "X" + currentArrows;
    }
    private void ActivateBowUI()
    {
        bowUI.gameObject.SetActive(true);
        DOTween.Sequence()
        .Append(dialoguePanels[4].DOAnchorPos(targetPositions[6], 0.5f).SetEase(Ease.InSine))
        .AppendInterval(5f)
        .Append(dialoguePanels[4].DOAnchorPos(originalPositions[4],0.8f).SetEase(Ease.InOutBack));
    }
    private void ActivateSwordUI()
    {
        swordUI.gameObject.SetActive(true);
        swordBorder.gameObject.SetActive(true);
        DOTween.Sequence()
        .Append(dialoguePanels[5].DOAnchorPos(targetPositions[7],0.5f).SetEase(Ease.InSine))
        .AppendInterval(5f)
        .Append(dialoguePanels[5].DOAnchorPos(originalPositions[5],0.8f).SetEase(Ease.InOutBack));
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
            int index = hitsReceived - 1;
            if (isLifeBarActive[index])
            {
                if (!damagedLifeIndices.Contains(index))
                {
                    damagedLifeIndices.Push(index);  
                }
                Image currentFill = lifeFills[index];
                StartCoroutine(FadeOutLifeBar(currentFill)); 
                regenerationTimers[index] = dataPlayer.regenerationLifeTime;  

                isLifeBarActive[index] = false; 

                if (hitsReceived == lifeFills.Length)
                {
                    Debug.Log("Sin vida");
                }
            }
            else
            {
                isLifeBarActive[index] = false;
                StartCoroutine(FadeOutLifeBar(lifeFills[index])); 
                regenerationTimers[index] = dataPlayer.regenerationLifeTime; 
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
    public void RegenerateLifeBar()
    {
        if (damagedLifeIndices.Count > 0)
        {
            int index = damagedLifeIndices.Peek();

            if (lifeFills[index].fillAmount < 1f)
            {
                if (regenerationTimers[index] > 0)
                {
                    regenerationTimers[index] -= Time.deltaTime;  
                }
                if (regenerationTimers[index] <= 0)
                {
                    isLifeBarActive[index] = true;  
                    damagedLifeIndices.Pop();  
                    StartCoroutine(RegenerateLifeBarAnimation(lifeFills[index])); 
                }
            }
        }
    }
    private IEnumerator RegenerateLifeBarAnimation(Image image)
    {
        float duration = 5f;
        float elapsedTime = 0f;
        float targetFillAmount = 1f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float lerpedAmount = Mathf.Lerp(0f, targetFillAmount, lifeRegenerationCurve.Evaluate(elapsedTime / duration));
            image.fillAmount = lerpedAmount;
            yield return null;
        }
        image.fillAmount = targetFillAmount;
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
        yield return new WaitForSeconds(3f);
        lifeBar.DOAnchorPos(targetPositions[2], 0.5f).SetEase(Ease.InOutQuad);
        staminaBar.DOAnchorPos(targetPositions[3], 0.8f).SetEase(Ease.OutQuad);
      
    }
    private void DisplayDialogueNPC(int npcIndex, NPCData dialoguesData)
    {
        npcNameTexts[npcIndex].text = dialoguesData.nameCharacter;
        dialogueTexts[npcIndex].text = dialoguesData.dialogue;
        npcImages[npcIndex].sprite = dialoguesData.imageCharacter;
    }
    private void ShowTutorial()
    {
     DOTween.Sequence()
    .AppendInterval(0.8f)
    .Append(dialoguePanels[2].DOAnchorPos(targetPositions[4],0.5f).SetEase(Ease.InSine))
    .AppendInterval(2f)
    .Append(dialoguePanels[2].DOAnchorPos(originalPositions[2], 0.8f).SetEase(Ease.InOutBack))
    .AppendInterval(2f)
    .Append(dialoguePanels[3].DOAnchorPos(targetPositions[5],0.5f).SetEase(Ease.InSine))
    .AppendInterval(2f)
    .Append(dialoguePanels[3].DOAnchorPos(originalPositions[3], 0.8f).SetEase(Ease.InOutBack));
    }
    private void ShowTimerArrows()
    {
        timerArrows.DOAnchorPos(targetPositions[8], 0.5f).SetEase(Ease.InOutBounce);

        if (timer != null)
        {
            DOTween.To(() => timer.fillAmount, x => timer.fillAmount = x, 1f, 5f).SetEase(Ease.InSine)
            .OnComplete(() =>
            {
                timerArrows.DOAnchorPos(positionTimer, 0.5f).SetEase(Ease.InOutBounce)
                .OnComplete(() =>
                {
                    timer.fillAmount = 0f;
                    countArrows.ReloadArrows();
                });
            });
        }
    }
}
