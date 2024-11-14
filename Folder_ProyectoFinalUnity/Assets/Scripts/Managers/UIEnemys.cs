using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIEnemys : MonoBehaviour
{
    [SerializeField] private Image lifeBarEnemy;
    [SerializeField] private SkeletonController enemySkeleton;
    [SerializeField] private EnemyData enemyData;

    private void OnEnable()
    {
        enemySkeleton.OnHealthChanged += UpdateHealthBarEnemy;
    }
    private void OnDisable()
    {
        enemySkeleton.OnHealthChanged -= UpdateHealthBarEnemy;
    }
    private void UpdateHealthBarEnemy(int currentLife)
    {
        float targetFillAmount = (float)currentLife / (float)enemyData.GetMaxLife();

        lifeBarEnemy.DOFillAmount(targetFillAmount, 1f).SetEase(Ease.OutBounce);
    }
}
