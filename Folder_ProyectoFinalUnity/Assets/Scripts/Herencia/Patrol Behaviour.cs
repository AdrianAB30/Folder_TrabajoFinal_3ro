using System;
using System.Collections;
using UnityEngine;

public abstract class PatrolBehaviour : MonoBehaviour 
{
    protected SimpleLinkedList<GameObject> nodesRoutes;
    [SerializeField] protected int currentPatrolIndex;
    [SerializeField] protected float speedPatrol;

    protected virtual void Start()
    {
        currentPatrolIndex = 0;
    }

    protected virtual void Update()
    {
        if (nodesRoutes != null && nodesRoutes.Length > 0)
        {
            Patrol();
        }
    }

    public void SetPatrolRoute(SimpleLinkedList<GameObject> patrolNodes)
    {
        nodesRoutes = patrolNodes;
        currentPatrolIndex = 0;
    }
    protected abstract void Patrol();

    protected void MoveTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * speedPatrol * Time.deltaTime;
    }
}
