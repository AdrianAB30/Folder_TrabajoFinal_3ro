using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LifeManager : MonoBehaviour
{
    private int playerHitsReceived = 0;

    public event Action<int> OnPlayerDamage;

    public void DamageToPlayer(int damageAmount)
    {
        if (playerHitsReceived < 3)
        {
            playerHitsReceived++;
            OnPlayerDamage?.Invoke(playerHitsReceived);
        }
    }
}

   


