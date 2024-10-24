using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LifeManager : MonoBehaviour
{
    [SerializeField] private Image fill1;
    [SerializeField] private Image fill2;
    [SerializeField] private Image fill3;
    private int hitsReceived = 0;
    private bool isUpdatingLifeBar = false;

    public static event Action OnPlayerDamage;

    public void DamageToPlayer()
    {
        if (hitsReceived < 3)
        {
            if (!isUpdatingLifeBar)
            {
                OnPlayerDamage?.Invoke();
                StartCoroutine(UpdateLifeBar());
                hitsReceived++;
            }
        }
    }

    private IEnumerator UpdateLifeBar()
    {
        Image currentFill = null;

        switch (hitsReceived)
        {
            case 0:
                currentFill = fill1;
                break;
            case 1:
                currentFill = fill2;
                break;
            case 2:
                currentFill = fill3;
                Debug.Log("Sin vida");
                break;
        }

        if (currentFill != null)
        {
            yield return StartCoroutine(FadeOut(currentFill));
        }
    }

    private IEnumerator FadeOut(Image image)
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

