using System;
using System.Collections;
using UnityEngine;

public class SpawnArrows : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject arrowPrefab;  
    [SerializeField] private Transform bowTransform;  
    [SerializeField] private float launchForce = 15f;
    private int arrowCount = 3;

    public event Action<int> OnArrowCountChanged;
    public event Action OnArrowsEmpty;

    private void OnEnable()
    {
        PlayerController.OnAttackBowSpawner += HandleBowAttack;
    }
    private void OnDisable()
    {
        PlayerController.OnAttackBowSpawner -= HandleBowAttack;
    }
    private void HandleBowAttack(bool isAttacking)
    {
        if (isAttacking && arrowCount > 0)
        {
            SpawnAndShootArrow();
            arrowCount--;
            OnArrowCountChanged?.Invoke(arrowCount);
            ReloadingArrows();
        }
    }
    private void ReloadingArrows()
    {
        if (arrowCount <= 0)
        {
            OnArrowsEmpty?.Invoke();
        }
    }
    private void SpawnAndShootArrow()
    {
        GameObject arrow = Instantiate(arrowPrefab, bowTransform.position, bowTransform.rotation);

        Rigidbody arrowRb = arrow.GetComponent<Rigidbody>();

        arrowRb.useGravity = true;

        Vector3 launchDirection = bowTransform.forward + bowTransform.up * 0.05f; 

        arrowRb.AddForce(launchDirection.normalized * launchForce, ForceMode.Impulse);

        arrow.transform.rotation = Quaternion.LookRotation(launchDirection);
    }
    public void ReloadArrows()
    {
        int randomArrows = UnityEngine.Random.Range(1, 6);
        arrowCount = randomArrows;
        OnArrowCountChanged?.Invoke(arrowCount); 
    }
}
