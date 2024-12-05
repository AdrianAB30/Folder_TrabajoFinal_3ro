using DG.Tweening.Core.Easing;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AtributesManager : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    [SerializeField] private PlayerController player;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private GameManager gameManager;
    private int playerHitsReceived = 0;

    public event Action<int> OnPlayerDamage;

    public void DamageToPlayer(int damageAmount)
    {
        if (playerData.life <= 0) return;
        playerData.life -= damageAmount;
        if (playerHitsReceived < 3)
        {
            playerHitsReceived++;

            playerData.life = Math.Clamp(playerData.life, 0, 3);
            OnPlayerDamage?.Invoke(playerHitsReceived);
        }
    }
    public void IncreaseStamina(float amount)
    {
        playerData.Stamina = Mathf.Min(playerData.Stamina + amount, 100);
        uiManager.UpdateStaminaBar();
    }
    public void DecreaseStamina(float amount)
    {
        if (gameManager.IsOptionsMenuActive) return;
        playerData.Stamina = Mathf.Max(playerData.Stamina - amount, 0);
        uiManager.UpdateStaminaBar();
    }
}

   


