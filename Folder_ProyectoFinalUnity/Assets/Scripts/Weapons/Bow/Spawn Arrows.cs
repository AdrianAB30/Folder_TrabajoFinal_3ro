using System;
using System.Collections;
using UnityEngine;

public class SpawnArrows : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject arrowPrefab;  
    [SerializeField] private Transform bowTransform;  
    [SerializeField] private float launchForce = 15f; 

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
        if (isAttacking)
        {
            SpawnAndShootArrow();
        }
    }

    private void SpawnAndShootArrow()
    {
        GameObject arrow = Instantiate(arrowPrefab, bowTransform.position, bowTransform.rotation);

        Rigidbody arrowRb = arrow.GetComponent<Rigidbody>();

        arrowRb.useGravity = true;

        Vector3 launchDirection = bowTransform.forward + bowTransform.up * 0.2f; 

        arrowRb.AddForce(launchDirection.normalized * launchForce, ForceMode.Impulse);

        arrow.transform.rotation = Quaternion.LookRotation(launchDirection);
    }
}
