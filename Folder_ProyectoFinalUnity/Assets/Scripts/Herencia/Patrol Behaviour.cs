using System;
using System.Collections;
using UnityEngine;

public abstract class PatrolBehaviour : MonoBehaviour 
{
    protected SimpleLinkedList<GameObject> nodesRoutes;
    [SerializeField] protected int currentPatrolIndex;
    [SerializeField] protected float distanceToNode;
    [SerializeField] protected float movementForce;
    [SerializeField] protected NPCData npcData;
    [SerializeField] protected EnemyData enemyData;
    public Rigidbody myRBD;

    protected virtual void Awake()
    {
        myRBD = GetComponent<Rigidbody>();
    }
    protected virtual void Start()
    {
        currentPatrolIndex = 0;
    }

    protected virtual void FixedUpdate()
    {
        if (nodesRoutes != null && nodesRoutes.Length > 0)
        {
            Patrol();
        }
    }

    protected virtual void SetPatrolRoute(SimpleLinkedList<GameObject> patrolNodes)
    {
        nodesRoutes = patrolNodes;
        currentPatrolIndex = 0;
    }
    protected abstract void Patrol();

    protected void MoveTowards(Vector3 targetPosition)
    {
        if (myRBD != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

            if (distanceToTarget > distanceToNode)
            {
                Vector3 direction = (targetPosition - transform.position).normalized;
                myRBD.velocity = direction * movementForce * Time.deltaTime;
            }
            else
            {
                myRBD.velocity = Vector3.zero;
            }
        }
    }

}
