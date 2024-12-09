using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIEnemys : MonoBehaviour
{
    [SerializeField] private Image lifeBarEnemy;
    [SerializeField] private SkeletonController enemySkeleton;
    [SerializeField] private FrogController enemyFrog;
    [SerializeField] private EnemyData enemyData;

    private void OnEnable()
    {
        if (enemySkeleton != null)
        {
            enemySkeleton.OnHealthChanged += UpdateHealthBarEnemy;
        }
        if (enemyFrog != null)
        {
            enemyFrog.OnHealthChanged += UpdateHealthBarEnemy;
        }
    }
    private void OnDisable()
    {

        if (enemySkeleton != null)
        {
            enemySkeleton.OnHealthChanged -= UpdateHealthBarEnemy;
        }
        if (enemyFrog != null)
        {
            enemyFrog.OnHealthChanged -= UpdateHealthBarEnemy;
        }
    }
    private void UpdateHealthBarEnemy(int currentLife)
    {
        float targetFillAmount = (float)currentLife / (float)enemyData.GetMaxLife();

        lifeBarEnemy.DOFillAmount(targetFillAmount, 1f).SetEase(Ease.OutBounce);
    }
}
