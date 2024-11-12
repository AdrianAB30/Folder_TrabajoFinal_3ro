using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Rigidbody rb;
    public ArrowData arrowData;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        ShootArrow();
    }
    public void ShootArrow()
    {
        rb.AddForce(transform.forward * arrowData.shootForce, ForceMode.VelocityChange);
    }
    private void Update()
    {
        if (rb.velocity.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(rb.velocity);

            if (rb.velocity.y < 0)
            {
                Vector3 currentEulerRotation = transform.eulerAngles;
                Vector3 targetEulerRotation = targetRotation.eulerAngles;

                targetEulerRotation.x = Mathf.LerpAngle(currentEulerRotation.x, arrowData.maxFallRotationX, Time.deltaTime * 20f); 

                targetEulerRotation.z = Mathf.LerpAngle(currentEulerRotation.z, arrowData.maxFallRotationZ, Time.deltaTime * 20f);

                targetRotation = Quaternion.Euler(targetEulerRotation.x, targetEulerRotation.y, targetEulerRotation.z);
            }
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }
    public float GetArrowDamage()
    {
        return arrowData.damage;
    }
}

